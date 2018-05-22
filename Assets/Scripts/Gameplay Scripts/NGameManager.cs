using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NGameManager : MonoBehaviour {

    Dictionary<GameObject, NPlayerController> playerDictionary = new Dictionary<GameObject, NPlayerController>();
    private static NGameManager instance = null;

    [SerializeField] NPersistentGameDataSO gameData;
    NLevelManager levelManager;

    void Start ()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
        instance = this;

        GameObject lm = GameObject.Find("Players, Spawns, and Level Manager");
        if (lm != null)
        {
            levelManager = lm.GetComponent<NLevelManager>();
            levelManager.Initialize(gameData);
        }
    }

    public void SetRounds(int r)
    {
        gameData.neededWins = r;
    }

    public void SetPlayerCharacterChoices(int[] c)
    {
        gameData.playerCharacterChoices = c;
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        GameObject lm = GameObject.Find("Players, Spawns, and Level Manager");
        if (lm != null)
        {
            levelManager = lm.GetComponent<NLevelManager>();
            levelManager.Initialize(gameData);
        }
    }

    public void LoadMenu()
    {
        SceneManager.LoadSceneAsync(17);
    }

    public void LoadLevel(int level)
    {
        if (level >= 1 && level <= 15)
            SceneManager.LoadSceneAsync(level);
    }

    public void LoadRandomLevel()
    {
        SceneManager.LoadSceneAsync(Random.Range(1, 16));
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
