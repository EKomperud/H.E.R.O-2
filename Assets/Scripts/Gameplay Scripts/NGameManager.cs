using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class NGameManager : MonoBehaviour {

    Dictionary<GameObject, NPlayerController> playerDictionary = new Dictionary<GameObject, NPlayerController>();
    private static NGameManager instance = null;

    [SerializeField] NPersistentGameDataSO gameData;
    [SerializeField] Transform winMenu;
    [SerializeField] Transform pauseMenu;
    NLevelManager levelManager;

    void Start ()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            gameData.playerWins = new int[4] { 0, 0, 0, 0 };
        }
        DontDestroyOnLoad(gameObject);
        instance = this;
    }

    public void SetRounds(int r)
    {
        gameData.neededWins = r;
    }

    public void SetPlayerCharacterChoices(int[] c)
    {
        gameData.playerCharacterChoices = c;
    }

    public void Play()
    {
        winMenu.gameObject.SetActive(false);
        gameData.playerWins = new int[4] { 0, 0, 0, 0 };
        SceneManager.LoadSceneAsync(Random.Range(1, 16));
    }

    public void Menu()
    {
        winMenu.gameObject.SetActive(false);
        SceneManager.LoadSceneAsync(17);
    }

    public void LoadRandomLevel()
    {
        SceneManager.LoadSceneAsync(Random.Range(1, 16));
    }

    public NPersistentGameDataSO SetLevelManager(NLevelManager lm)
    {
        levelManager = lm;
        return gameData;
    }

    public void WinSequence(int winner)
    {
        winMenu.gameObject.SetActive(true);
        EventSystem.current.SetSelectedGameObject(winMenu.GetChild(1).gameObject);
        winMenu.GetChild(3).GetComponent<Text>().text = "P" + (winner + 1) + " is better than everyone else";
    }

    public void SetEventSystemSelected(GameObject g)
    {
        EventSystem.current.SetSelectedGameObject(g);
    }

    public static bool TryGetInstance(out NGameManager gm)
    {
        gm = instance;
        if (instance == null)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
}
