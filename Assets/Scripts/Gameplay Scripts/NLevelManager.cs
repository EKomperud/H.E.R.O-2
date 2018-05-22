using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class NLevelManager : MonoBehaviour {

    private enum LevelType
    {
        menu = 0,
        gameplay = 1
    }

    [SerializeField] private LevelType levelType;
    [SerializeField] private Transform spawnPointsObject;
    [SerializeField] private Transform playersObject;
    [SerializeField] private Transform winMenu;
    [SerializeField] private Transform pauseMenu;
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
	}
	
    public void Initialize(NPersistentGameDataSO gameData)
    {
        this.gameData = gameData;
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
                if (player.GetLivingStatus())
                {
                    if (++gameData.playerWins[player.GetPlayerNumber()] >= gameData.neededWins)
                    {
                        winMenu.gameObject.SetActive(true);
                        EventSystem.current.SetSelectedGameObject(winMenu.GetChild(1).gameObject);
                        winMenu.GetChild(3).GetComponent<Text>().text = "shit";
                    }
                    else
                    {
                        if (gameManager != null || NGameManager.TryGetInstance(out gameManager))
                            gameManager.LoadRandomLevel();
                    }
                }
            }
        }
    }
}
