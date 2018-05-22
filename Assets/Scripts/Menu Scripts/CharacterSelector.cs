using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Rewired;

public class CharacterSelector : MonoBehaviour {

    public enum SelectState
    {
        inactive = 0,
        selecting = 1,
        chosen = 2
    }

    public bool active;
    private Player joystick;
    public SelectState state;
    private int characterIndex;
    private float inputRepeatDelay;
    private bool inputAvailable;
    [SerializeField] private int playerIndex;
    [SerializeField] private CharacterSelectManager selectManager;

    void Start ()
    {
        joystick = ReInput.players.GetPlayer(playerIndex);
        characterIndex = playerIndex;
        active = false;
        inputRepeatDelay = 0.175f;
        inputAvailable = true;
    }

    void Update ()
    {
        if (active)
        {
            switch (state)
            {
                case SelectState.inactive:
                    UpdateInactive();
                    break;
                case SelectState.selecting:
                    UpdateSelecting();
                    break;
                case SelectState.chosen:
                    UpdateChosen();
                    break;
            }
        }
    }

    void UpdateInactive()
    {
        if (joystick.GetButtonDown("UISubmit"))
        {
            selectManager.ActivatePlayer(playerIndex);
            state = SelectState.selecting;
        }
        else if (joystick.GetButton("UICancel"))
        {
            selectManager.HoldingBack();
        }
    }

    void UpdateSelecting()
    {
        if (joystick.GetButtonDown("UISubmit"))
        {
            if (selectManager.LockInCharacter(playerIndex, characterIndex))
                state = SelectState.chosen;
        }
        else if (joystick.GetAxis("Move Horizontal") != 0 && inputAvailable)
        {
            StartCoroutine("InputRepeatDelay");
            float h = joystick.GetAxis("Move Horizontal");
            characterIndex = h > 0 ? (characterIndex = ++characterIndex > 3 ? 0 : characterIndex) : (characterIndex = --characterIndex < 0 ? 3 : characterIndex);
            selectManager.ChangeCharacter(playerIndex, characterIndex);
        }
        else if (joystick.GetButtonDown("UICancel"))
        {
            selectManager.DeactivatePlayer(playerIndex);
            state = SelectState.inactive;
        }
    }

    void UpdateChosen()
    {
        if (joystick.GetButtonDown("UICancel"))
        {
            selectManager.UnlockCharacter(playerIndex);
            state = SelectState.selecting;
        }
        if (joystick.GetButtonDown("UISubmit"))
        {
            selectManager.TryStageSelect();
        }
    }

    public void SetActive(bool a)
    {
        active = a;
        state = SelectState.inactive;
    }

    public void SwitchActive()
    {
        active = !active;
        state = SelectState.inactive;
    }

    private IEnumerator InputRepeatDelay()
    {
        inputAvailable = false;
        float d = 0;
        while (d < inputRepeatDelay)
        {
            d += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        inputAvailable = true;
    }
}
