using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Rewired;

public class JoystickArray
{
    private List<bool> activeArray;
    //private List<int> playersArray;
    private int activePlayers;
    //private int openSpot;

    public JoystickArray(int size)
    {
        activeArray = new List<bool>(size);
        activePlayers = 0;
        //openSpot = 0;
    }

    public bool ActivatePlayer(int p)
    {
        if (activePlayers != activeArray.Capacity)
        {
            activeArray[p] = true;
            activePlayers++;
            //int openSpotSeeker = openSpot + 1;
            //while (openSpotSeeker < activeArray.Capacity && activeArray[openSpotSeeker])
            //    openSpotSeeker++;
            return true;
        }
        return false;
    }

    public bool DropPlayer(int p)
    {
        if (activeArray[p])
        {
            activeArray[p] = false;
            activePlayers--;
            //openSpot = p < openSpot ? p : openSpot;
            return true;
        }
        return false;
    }
}

public class CharacterSelectManager : MonoBehaviour {

    [SerializeField] MenuDataSO menuData;
    [SerializeField] MainMenuManager mainMenuManager;
    [SerializeField] Slider backSlider;
    [SerializeField] CharacterSelector[] selectors;

    private Image[] playerPortraits;
    private Sprite[] characterPortraits;
    private int[] playerCharacterChoices;
    private int activePlayers;
    private int lockedPlayers;

    void Start ()
    {
        characterPortraits = new Sprite[5];
        characterPortraits[0] = menuData.GetCharacterPortrait(0);
        characterPortraits[1] = menuData.GetCharacterPortrait(1);
        characterPortraits[2] = menuData.GetCharacterPortrait(2);
        characterPortraits[3] = menuData.GetCharacterPortrait(3);
        characterPortraits[4] = menuData.GetCharacterPortrait(4);
        playerPortraits = new Image[selectors.Length];
        for (int i = 0; i < selectors.Length; i++)
        {
            playerPortraits[i] = selectors[i].GetComponent<Image>();
            playerPortraits[i].sprite = characterPortraits[0];
            playerPortraits[i].color = new Color(0f, 0f, 0f, 1f);
        }
        playerCharacterChoices = new int[4] { 4, 4, 4, 4 };
        activePlayers = 0;
        lockedPlayers = 0;
    }

    void Update ()
    {
        backSlider.value -= Time.deltaTime;
	}

    public void ActivatePlayer(int p)
    {
        playerPortraits[p].sprite = characterPortraits[p];
        playerPortraits[p].color = new Color(1f, 1f, 1f, 0.5f);
        activePlayers++;
    }

    public void DeactivatePlayer(int p)
    {
        playerPortraits[p].sprite = characterPortraits[0];
        playerPortraits[p].color = new Color(0f, 0f, 0f, 1f);
        activePlayers--;
    }

    public void DeactivatePlayers()
    {
        for (int i = 0; i < playerPortraits.Length; i++)
        {
            playerPortraits[i].sprite = characterPortraits[0];
            playerPortraits[i].color = new Color(0f, 0f, 0f, 1f);
        }
        activePlayers = 0;
        lockedPlayers = 0;
    }

    public void ChangeCharacter(int p, int c)
    {
        playerPortraits[p].sprite = characterPortraits[c];
    }

    public bool LockInCharacter(int p, int c)
    {
        bool validLock = true;
        foreach (int ch in playerCharacterChoices)
            if (ch == c)
                validLock = false;
        if (validLock)
        {
            playerPortraits[p].color = new Color(1f, 1f, 1f, 1f);
            playerCharacterChoices[p] = c;
            lockedPlayers++;
            return true;
        }
        else
            return false;
    }

    public void UnlockCharacter(int p)
    {
        playerCharacterChoices[p] = 0;
        playerPortraits[p].color = new Color(1f, 1f, 1f, 0.5f);
        lockedPlayers--;
    }

    public void TryStageSelect()
    {
        if (lockedPlayers == activePlayers && lockedPlayers >= 2)
        {
            mainMenuManager.SetPlayerCharacterChoices(playerCharacterChoices);
            mainMenuManager.LoadStageSelect();
        }
    }

    public void HoldingBack()
    {
        backSlider.value += (2 * Time.deltaTime);
        if (backSlider.value == backSlider.maxValue)
        {
            backSlider.value = backSlider.minValue;
            mainMenuManager.ToMainMenu();
        }
    }

    public void SetActive(bool a)
    {
        foreach (CharacterSelector cs in selectors)
            cs.SetActive(a);
    }
}
