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
    private Camera cam;
    private IEnumerator cameraCoroutine;
    private Queue<IEnumerator> cameraQueue;
    private int livingPlayers;

	void Start ()
    {
        cam = Camera.main;
        cameraQueue = new Queue<IEnumerator>();
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
        StartSequence();
	}
	
    public void Initialize()
    {
        if (levelType == LevelType.gameplay)
        {
            int playerNumber = 0;
            foreach (NPlayerController player in players.Values)
            {
                playerNumber = player.GetPlayerNumber();
                if (gameData.playerCharacterChoices[playerNumber] != 4)
                {
                    player.SetAnimators(gameData.characterTemplates[gameData.playerCharacterChoices[playerNumber]].GetAnimatorControllers());
                    player.SetPants(gameData.characterTemplates[gameData.playerCharacterChoices[playerNumber]].GetPants());
                    LinkedList<Transform>.Enumerator e = spawnPoints.GetEnumerator();
                    int r = Random.Range(1, spawnPoints.Count);
                    for (int j = 0; j < r; j++)
                        e.MoveNext();
                    player.transform.position = e.Current.position;
                    spawnPoints.Remove(e.Current);
                    player.LevelManagerInitialize(this);
                    livingPlayers++;
                }
                else
                {
                    player.gameObject.SetActive(false);
                }
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

    private void StartSequence()
    {
        int playerNumber;
        foreach (NPlayerController player in players.Values)
        {
            playerNumber = player.GetPlayerNumber();
            if (gameData.playerCharacterChoices[playerNumber] != 4)
            {
                cameraQueue.Enqueue(CamToPosition(new Vector3(player.transform.position.x, player.transform.position.y, cam.transform.position.z), 4.5f, player));
            }
        }
        cameraQueue.Enqueue(CamToPosition(cam.transform.position, cam.orthographicSize, null));
        cameraCoroutine = cameraQueue.Dequeue();
        StartCoroutine(cameraCoroutine);
        //BeginMatch();
    }

    private IEnumerator CamToPosition(Vector3 dest, float size, NPlayerController player)
    {
        if (player != null)
            player.Vibrate(1, 0.5f, 0.5f);
        while (Vector2.Distance(cam.transform.position, dest) > 0.05f)
        {
            cam.transform.position += (dest - cam.transform.position) * 0.125f;
            if (Mathf.Abs(cam.orthographicSize - size) > 0.05f)
                cam.orthographicSize += (size - cam.orthographicSize) * 0.75f;
            else
                cam.orthographicSize = size;
            yield return new WaitForEndOfFrame();
        }
        cam.transform.position = dest;
        yield return new WaitForSeconds(0.6f);
        if (cameraQueue.Count > 1)
        {
            cameraCoroutine = cameraQueue.Dequeue();
            StartCoroutine(cameraCoroutine);
        }
        else if (cameraQueue.Count == 1)
        {
            cameraCoroutine = cameraQueue.Dequeue();
            StartCoroutine(cameraCoroutine);
        }
        else
        {
            BeginMatch();
        }
    }

    private void BeginMatch()
    {
        int playerNumber;
        foreach (NPlayerController player in players.Values)
        {
            playerNumber = player.GetPlayerNumber();
            if (gameData.playerCharacterChoices[playerNumber] != 4)
            {
                player.SetMovementBool("active",true);
            }
        }
    }

    private IEnumerator EndSequence()
    {
        yield return new WaitForSeconds(2.5f);
        if (gameManager != null || NGameManager.TryGetInstance(out gameManager))
            gameManager.LoadRandomLevel();
    }
}
