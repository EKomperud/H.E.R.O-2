using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {
    private static GameManager managerInstance;
    public HealthScript p;
	public SpikeScript spikes;
	public GameObject WinScreen;
    public Transform CyborgPrefab;
    public Sprite CyborgSelect;
    public Sprite CyborgLock;
    public Transform NinjaPrefab;
    public Sprite NinjaSelect;
    public Sprite NinjaLock;
    public Transform WitchHunterPrefab;
    public Sprite WitchHunterSelect;
    public Sprite WitchHunterLock;
    public Transform PiratePrefab;
    public Sprite PirateSelect;
    public Sprite PirateLock;

    public AudioClip MenuMusic;
    private AudioClip[] battleMusic;
    public AudioClip battle1;
    public AudioClip battle2;
    public AudioClip battle3;
    public AudioClip battle4;
    public AudioClip battle5;

    /// <summary>
    /// Images of the characters
    /// Cyborg = 0
    /// Ninja = 1
    /// Pirate = 2
    /// Witch Hunter = 3
    /// </summary>
    private Sprite[] selectImages;

    /// <summary>
    /// Maps players to their selected characters
    /// P1 = 0
    /// P2 = 1
    /// P3 = 2
    /// P4 = 3
    /// </summary>
    private List<int> selectedCharacters;

    private string[] characterNames;
    public Transform[] characterPrefabs;

    private SpawnPoint[] spawnPoints;

	public bool MainMenuSlide = false;
	public bool playBack = false;
	public bool setTimer = false;
	public int buttonCountFirst = 0;
    public bool[] OpenDoor;
	private int numOfRounds = 0;
	private int randomLevel = 0;
	private bool startUp = false;

    public int layer;   // This should achieve the same function as having any number of layers

	public bool doorDown = false;
	public bool doorUp = false;
	public bool firstTime = true;
	public bool TutorialScene = false;
	public NumberKeeper Keeper;
	public int Player1W = 0;
	public int Player2W = 0;
	public int Player3W = 0;
	public int Player4W = 0;
    public int totalPlayers;
    public int activePlayers;
    private bool[] playerStatuses;
    private int[] winCounts;
    private string winner = null;
    private bool winnerShown = false;
    private bool levelLoadup = true;
    private float l;

    public float EndOfMatchTime = 2f;
    private float EndOfMatchTimer;
	public Text firstWins;
	private bool dontChange = true;
	private bool winAdd1 = true;

    // Use this for initialization
    void Start() {
        try
        {
            WinScreen = GameObject.Find("Canvas").transform.GetChild(1).gameObject;
        }
        finally
        {
            Keeper = GameObject.Find("NumberKeeper").GetComponent<NumberKeeper>();
            //if (this.gameObject.tag == "Player" | this.gameObject.tag == "Player2" | this.gameObject.tag == "Player3" | this.gameObject.tag == "Player4") {
            //    firstWins.text = "";
            //}
            p = gameObject.GetComponent<HealthScript>();
            layer = 0;
            battleMusic = new AudioClip[5];
            playerStatuses = new bool[4];
            winCounts = new int[4];
            selectedCharacters = new List<int>(4);
            characterNames = new string[4];
            characterPrefabs = new Transform[4];
            selectImages = new Sprite[4];
            for (int i = 0; i < 4; i++)
            {
                playerStatuses[i] = false;
                winCounts[i] = 0;
                selectedCharacters.Add(i);;
            }
            selectImages = new Sprite[8];
            battleMusic[0] = battle1;
            battleMusic[1] = battle2;
            battleMusic[2] = battle3;
            battleMusic[3] = battle4;
            battleMusic[4] = battle5;
            selectImages[0] = CyborgSelect;
            selectImages[1] = NinjaSelect;
            selectImages[2] = PirateSelect;
            selectImages[3] = WitchHunterSelect;
            selectImages[4] = CyborgLock;
            selectImages[5] = NinjaLock;
            selectImages[6] = PirateLock;
            selectImages[7] = WitchHunterLock;
            characterNames[0] = "Cyborg";
            characterNames[1] = "Ninja";
            characterNames[2] = "Pirate";
            characterNames[3] = "WitchHunter";
            characterPrefabs[0] = CyborgPrefab;
            characterPrefabs[1] = NinjaPrefab;
            characterPrefabs[2] = PiratePrefab;
            characterPrefabs[3] = WitchHunterPrefab;
            DontDestroyOnLoad(this);
            if (managerInstance == null)
            {
                managerInstance = this;
            }
            else
                Destroy(gameObject);
        }
    }
	
	// Update is called once per frame
	void Update () {

		if (startUp) {
            //pNum = Keeper.numOfP;
            //randomLevel = Random.Range (1, 8);
            EndOfMatchTimer = EndOfMatchTime;
            randomLevel = Random.Range (1, 16);
			Application.LoadLevel (randomLevel);
			//numOfRounds -= 1;
			//Keeper.numberOfRounds = numOfRounds;
			startUp = false;
            Debug.Log("total players upon startup: " + totalPlayers);
            activePlayers = totalPlayers;
            levelLoadup = false;
            Transform canvas2 = GameObject.Find("Canvas2").transform.GetChild(0);
            canvas2.gameObject.SetActive(false);
		}
        if (!levelLoadup)
        {
            activePlayers = totalPlayers;
            l -= Time.deltaTime;
            
            if (l <= 0)
            {
                try
                {
                    WinScreen = GameObject.Find("Canvas").transform.GetChild(1).gameObject;
                    System.Random playerRandomizer = new System.Random();
                    int[] playerPositions = new int[totalPlayers + 1];
                    playerPositions[0] = 0;
                    for (int i = 1; i < totalPlayers + 1; i++)
                    {
                        int position = playerRandomizer.Next(1, totalPlayers + 1);
                        while (position == playerPositions[i - 1])
                            position = playerRandomizer.Next(1, totalPlayers + 1);
                        playerPositions[i] = position;
                    }
                    for (int i = 1; i < totalPlayers + 1; i++)
                    {
                        SpawnPoint spawn1 = GameObject.Find("Spawn" + i).GetComponent<SpawnPoint>();
                        spawn1.SpawnPlayer("_P" + playerPositions[i], characterNames[selectedCharacters[playerPositions[i] - 1] - selectedCharacters.Count]
                            , characterPrefabs[selectedCharacters[playerPositions[i] - 1] - selectedCharacters.Count]);
                    }
                    levelLoadup = true; ;
                }
                finally { }
            }

            //levelLoadup = true;
        }

        if (activePlayers == 1 && winner == null)
        {
            if (EndOfMatchTimer >= 0)
            {
                EndOfMatchTimer -= Time.deltaTime;
            }
            else
            {
                EndOfMatchTimer = EndOfMatchTime;
                activePlayers = totalPlayers;
                for (int i = 0; i < totalPlayers; i++)
                {
                    playerStatuses[i] = true;
                }
                randomLevel = Random.Range(1, 16);
                Application.LoadLevel(randomLevel);
                levelLoadup = false;
                l = 3.0f;
            }
        }
        if (winner != null && !winnerShown)
        {
            winnerShown = true;
            //for (int i = 0; i < totalPlayers; i++)
            //{
            //    firstWins.text += "Player " + (i + 1) + ": " + winCounts[i] + " wins ";
            //}
            Transform text = WinScreen.transform.GetChild(3);
            Text actualText = text.GetComponent<Text>();
            actualText.text = winner + " is better than everyone else";
            WinScreen.SetActive(true);
            Button playAgainButton = WinScreen.transform.GetChild(2).gameObject.GetComponent<Button>();
            playAgainButton.onClick.AddListener(() => RedoRounds());
            Button menuButton = WinScreen.transform.GetChild(1).gameObject.GetComponent<Button>();
            menuButton.onClick.AddListener(() => ExitLevel());
        }
    }

    /// <summary>
    /// Restarts the level you are on
    /// </summary>
	public void RestartLevel() {
		Application.LoadLevel(Application.loadedLevel);
	}

    /// <summary>
    /// Exits to the main menu from the win screen
    /// </summary>
	public void ExitLevel (){
        MainMenuSlide = false;
        activePlayers = 0;
        doorUp = false;
        layer = 0;
        numOfRounds = 0;
        totalPlayers = 0;
        winner = null;
        firstTime = true;
        layer = 1;
		Application.LoadLevel (0);
        MainMenu();
	}

    /// <summary>
    /// Quits the game entirely
    /// </summary>
	public void ExitGame() {
		Application.Quit ();
	}

    /// <summary>
    /// Plays on the first menu click
    /// </summary>
	public void MainMenu() {
		if (firstTime) {
            firstTime = false;
            AudioSource source = GetComponent<AudioSource>();
            source.loop = true;
            source.PlayOneShot(MenuMusic);
			MainMenuSlide = true;
			playBack = false;
			setTimer = true;
            layer = 1;
		}
	}

    /// <summary>
    /// Operation when menu goes back on layer
    /// </summary>
	public void GoBack() {
		playBack = true;
		MainMenuSlide = false;
		setTimer = true;
        if (layer > 1)      
            layer--;

        if (layer == 2)
        {
            for (int i = 0; i < 4; i++)
            {
                GameObject.Find("Canvas2").transform.GetChild(3).transform.GetChild(i).gameObject.SetActive(true);
            }
        }
	}

    /// <summary>
    /// Operation when menu goes forward one layer
    /// </summary>
	public void Next() {
		playBack = true;
		MainMenuSlide = false;
		setTimer = true;
        if (layer < 7)
            layer++;
	}

    /// <summary>
    /// Call to set the number of players in a game
    /// </summary>
    public void Players (int p)
    {
        totalPlayers = p;
        activePlayers = p;
        for (int i = 0; i < p; i++)
        {
            playerStatuses[i] = true;
        }
        for (int i = p; i<4; i++)
        {
            GameObject.Find("Canvas2").transform.GetChild(3).transform.GetChild(i).gameObject.SetActive(false);
        }
        Next();
    }

    /// <summary>
    /// Clicks the play button
    /// </summary>
	public void PlayButton () {
		playBack = true;
		MainMenuSlide = false;
        layer = 2;
		setTimer = true;
	}

    /// <summary>
    /// Call to set the number of rounds
    /// </summary>
    public void SetRounds(int r)
    {
        System.Random rand = new System.Random();
        AudioSource source = GetComponent<AudioSource>();
        source.Stop();
        source.PlayOneShot(battleMusic[rand.Next(battleMusic.Length-1)]);
        numOfRounds = r;
        Keeper.previousRounds = r;
        startUp = true;
        layer++;
    }

    /// <summary>
    /// Resets the game to where it was on the first round from the win screen
    /// </summary>
	public void RedoRounds() {
        System.Random rand = new System.Random();
        AudioSource source = GetComponent<AudioSource>();
        source.Stop();
        source.PlayOneShot(battleMusic[rand.Next(battleMusic.Length-1)]);
        playerStatuses = new bool[4];
        winCounts = new int[4];
        Debug.Log("total players upon RedoRoundsCall: " + totalPlayers);
        for (int i = 0; i < totalPlayers; i++)
        {
            playerStatuses[i] = true;
            winCounts[i] = 0;
        }
        winner = null;
        winnerShown = false;
        startUp = true;
        l = 1f;
	}

    /// <summary>
    /// Plays the tutorial level
    /// </summary>
	public void Tutorial() {
		TutorialScene = true;
		Keeper.previousRounds = 1;
		Application.LoadLevel (16);
	}

    /// <summary>
    /// Kills the parameter player and updates all relevatnt data
    /// </summary>
    /// <param name="player"></param>
    public void KillPlayer (string player)
    {
        int playerNumber = int.Parse(player[player.Length - 1] + "");
        if (playerNumber <= totalPlayers)
            activePlayers--;
        playerStatuses[playerNumber - 1] = false;
        if (activePlayers == 1)
        {
            for (int i=0; i<4; i++)
            {
                if (playerStatuses[i])
                {
                    winCounts[i]++;
                    Debug.Log("player win count: " + winCounts[i]);
                    if (winCounts[i]==numOfRounds)
                    {
                        winner = "P" + (i + 1);
                    }
                }
            }
        }
    }

    public void PlayerLock(int p)
    {
        if (!selectedCharacters.Contains(selectedCharacters[p-1]+selectedCharacters.Count))
        {
            Image select = GameObject.Find(p + "Select").GetComponent<Image>();
            if (selectedCharacters[p-1]<selectedCharacters.Count) {
                select.sprite = selectImages[selectedCharacters[p - 1] + (selectImages.Length / 2)];
                selectedCharacters[p - 1] += selectedCharacters.Count;           
            }
            else
            {
                select.sprite = selectImages[selectedCharacters[p - 1] - (selectImages.Length / 2)];
                selectedCharacters[p - 1] -= selectedCharacters.Count;
            }
        }
        int locked = 0;
        foreach (int i in selectedCharacters)
        {
            if (i >= selectedCharacters.Count)
                locked++;
        }
        Button next = GameObject.Find("Canvas2").transform.GetChild(3).transform.GetChild(5).GetComponent<Button>();
        if (locked == totalPlayers)
            next.interactable = true;
        else
            next.interactable = false;

    }

    public void ScrollUp (int player)
    {
        Image select = GameObject.Find(player + "Select").GetComponent<Image>();
        if (selectedCharacters[player - 1] == (selectImages.Length/2)-1)
            selectedCharacters[player - 1] = 0;
        else
            selectedCharacters[player - 1]++;

        select.sprite = selectImages[selectedCharacters[player - 1]];
    }

    public void ScrollDown (int player)
    {
        Image select = GameObject.Find(player + "Select").GetComponent<Image>();
        if (selectedCharacters[player - 1] == 0)
            selectedCharacters[player - 1] = (selectImages.Length/2)-1;
        else
            selectedCharacters[player - 1] --;

        select.sprite = selectImages[selectedCharacters[player - 1]];
    }

}
