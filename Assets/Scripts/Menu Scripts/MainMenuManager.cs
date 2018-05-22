using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MainMenuManager : MonoBehaviour {

    [SerializeField] Button fightButton;
    [SerializeField] Slider winsSlider;
    [SerializeField] Transform matchSettingsPanel;
    [SerializeField] CharacterSelectManager characterSelectManager;
    EventSystem eventSystem;
    NGameManager gameManager;

    public delegate void Transition();
    public static event Transition CharSelectTransition;

	void Start ()
    {
        eventSystem = EventSystem.current;
        fightButton.onClick.AddListener(ToCharacterSelect);
        characterSelectManager.SetActive(false);
        NGameManager.TryGetInstance(out gameManager);
    }

    public void ToCharacterSelect()
    {
        SetRounds((int)winsSlider.value);
        eventSystem.SetSelectedGameObject(characterSelectManager.gameObject);
        characterSelectManager.DeactivatePlayers();
        if (CharSelectTransition != null)
            CharSelectTransition();
    }

    public void ToMainMenu()
    {
        eventSystem.SetSelectedGameObject(fightButton.gameObject);
        if (CharSelectTransition != null)
            CharSelectTransition();
    }

    public void SetRounds(int r)
    {
        if (gameManager != null || NGameManager.TryGetInstance(out gameManager))
            gameManager.SetRounds(r);
    }

    public void SetPlayerCharacterChoices(int[] c)
    {
        if (gameManager != null || NGameManager.TryGetInstance(out gameManager))
            gameManager.SetPlayerCharacterChoices(c);
    }

    public void LoadStageSelect()
    {
        if (gameManager != null || NGameManager.TryGetInstance(out gameManager))
            gameManager.LoadRandomLevel();
    }
}
