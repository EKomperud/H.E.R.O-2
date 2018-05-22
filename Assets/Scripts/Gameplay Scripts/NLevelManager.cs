using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NLevelManager : MonoBehaviour {

    private enum LevelType
    {
        menu = 0,
        gameplay = 1
    }

    [SerializeField] private LevelType levelType;
    [SerializeField] private Transform spawnPointsObject;
    [SerializeField] private Transform playersObject;
    private NGameManager gameManager;
    private LinkedList<Transform> spawnPoints;
    private Dictionary<GameObject, NPlayerController> players;
    private NPersistentGameDataSO gameData;
    private int livingPlayers;

	void Start ()
    {
        players = new Dictionary<GameObject, NPlayerController>();
        for (int i = 0; i < 4; i++)
        {
            players.Add(playersObject.GetChild(i).gameObject, playersObject.GetChild(i).GetComponent<NPlayerController>());
        }
        spawnPoints = new LinkedList<Transform>();
        for (int i = 0; i < spawnPointsObject.childCount; i++)
        {
            spawnPoints.AddLast(spawnPointsObject.GetChild(i));
        }
        if (NGameManager.TryGetInstance(out gameManager))
        {
            gameData = gameManager.SetLevelManager(this);
        }
        Initialize();
	}
	
    public void Initialize()
    {
        if (levelType == LevelType.gameplay)
        {
            int i = 0;
            foreach (NPlayerController player in players.Values)
            {
                if (gameData.playerCharacterChoices[i] != 4)
                {
                    player.GetComponent<Animator>().runtimeAnimatorController = gameData.characterTemplates[gameData.playerCharacterChoices[i]].animator;
                    LinkedList<Transform>.Enumerator e = spawnPoints.GetEnumerator();
                    int r = Random.Range(1, spawnPoints.Count);
                    for (int j = 0; j < r; j++)
                        e.MoveNext();
                    player.transform.position = e.Current.position;
                    spawnPoints.Remove(e.Current);
                    player.AddLevelManager(this);
                    livingPlayers++;
                }
                else
                {
                    player.gameObject.SetActive(false);
                }
                i++;
            }
        }
    }

    public void RemovePlayer(GameObject go)
    {
        livingPlayers--;
        if (livingPlayers == 1)
        {
            foreach (NPlayerController player in players.Values)
            {
                if (player.gameObject.activeSelf && player.GetLivingStatus())
                {
                    if (++gameData.playerWins[player.GetPlayerNumber()] >= gameData.neededWins)
                    {
                        gameManager.WinSequence(player.GetPlayerNumber());
                    }
                    else
                    {
                        StartCoroutine("EndSequence");
                    }
                }
            }
        }
    }

    private IEnumerator EndSequence()
    {
        Debug.Log("end sequence");
        yield return new WaitForSeconds(2.5f);
        if (gameManager != null || NGameManager.TryGetInstance(out gameManager))
            gameManager.LoadRandomLevel();
    }
}
