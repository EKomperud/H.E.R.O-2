using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {
    public HealthScript p;
	public SpikeScript spikes;
	public GameObject WinScreen;
	public bool MainMenuSlide = false;
	public bool playBack = false;
	public bool setTimer = false;
	public int buttonCountFirst = 0;
	private int numOfRounds = 0;
	private int randomLevel = 0;
	private bool startUp = false;
	private bool P1exists = false;
	private bool P2exists = false;
	private bool P3exists = false;
	private bool P4exists = false;

    public bool firstLayer = false;
    public bool secondLayer = false;
    public bool thirdLayer = false;
    public bool fourthLayer = false;
    public bool fifthLayer = false;
    public bool sixthLayer = false;
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

	public int pNum = 0;
	public int PCONE = 1;
	public int PCTWO = 1;
	public int PCTHREE = 1;
	public int PCFOUR = 1;
	public bool P1C = false;
	public bool P2C = false;
	public bool P3C = false;
	public bool P4C = false;
	public Text firstWins;
	private bool dontChange = true;
	private bool winAdd1 = true;

    // Use this for initialization
    void Start() {
        WinScreen = GameObject.Find("Canvas").transform.GetChild(1).gameObject;
        Keeper = GameObject.Find("NumberKeeper").GetComponent<NumberKeeper>();
        if (this.gameObject.tag == "Player" | this.gameObject.tag == "Player2" | this.gameObject.tag == "Player3" | this.gameObject.tag == "Player4") {
            firstWins.text = "";
        }
        p = gameObject.GetComponent<HealthScript>();
        layer = 0;
        playerStatuses = new bool[4];
        winCounts = new int[4];
        for (int i=0; i<4; i++)
        {
            playerStatuses[i] = false;
            winCounts[i] = 0;
        }
        DontDestroyOnLoad(this);
    }
	
	// Update is called once per frame
	void Update () {

		if (startUp) {
			//pNum = Keeper.numOfP;
			randomLevel = Random.Range (1, 8);
			Application.LoadLevel (randomLevel);
			//numOfRounds -= 1;
			//Keeper.numberOfRounds = numOfRounds;
			startUp = false;
            activePlayers = totalPlayers;
            levelLoadup = false;
		}
        if (!levelLoadup)
        {
            activePlayers = totalPlayers;
            l -= Time.deltaTime;
            if (l <=0)
            {
                WinScreen = GameObject.Find("Canvas").transform.GetChild(1).gameObject;
                levelLoadup = true;
            }
        }
        //numOfRounds = Keeper.numberOfRounds;
  //      if (Keeper.p2Input && Keeper.p1Input && Keeper.p3Input && Keeper.p4Input) {
		//	randomLevel = Random.Range (1, 8);
		//	Application.LoadLevel (randomLevel);
		//	//numOfRounds -= 1;
		//	Keeper.numberOfRounds = numOfRounds;
		//	Keeper.numOfP = Keeper.preNumOfP;
		//	Keeper.ifDied = false;
		//	Keeper.p1Input = false;
		//	Keeper.p2Input = false;
		//	Keeper.p3Input = false;
		//	Keeper.p4Input = false;
		//}
        if (activePlayers == 1 && winner == null)
        {
            activePlayers = totalPlayers;
            for (int i = 0; i < totalPlayers; i++)
            {
                playerStatuses[i] = true;
            }
            randomLevel = Random.Range(1, 8);
            Application.LoadLevel(randomLevel);
            levelLoadup = false;
            l = 1f;

        }
        if (winner != null && !winnerShown)
        {
            winnerShown = true;
            //for (int i = 0; i < totalPlayers; i++)
            //{
            //    firstWins.text += "Player " + (i + 1) + ": " + winCounts[i] + " wins ";
            //}
            WinScreen.SetActive(true);
        }
        //if (Keeper.ifDied && numOfRounds > 0 && Keeper.numOfP == 1) {
        //	if (this.gameObject.tag == "Player" && Keeper.p1Input != true) {
        //		Keeper.P1WINS += Player1W;
        //		Keeper.p1Input = true;
        //		Debug.Log (Keeper.P1WINS + " P1");
        //		if (Keeper.numOfP == 1) {
        //			Keeper.p2Input = true;
        //			Keeper.p3Input = true;
        //			Keeper.p4Input = true;
        //		}
        //	}
        //	if (this.gameObject.tag == "Player2" && Keeper.p2Input != true) {
        //		Keeper.P2WINS += Player2W;
        //		Keeper.p2Input = true;
        //		Debug.Log (Keeper.P2WINS + " P2");
        //		if (Keeper.numOfP == 1) {
        //			Keeper.p1Input = true;
        //			Keeper.p3Input = true;
        //			Keeper.p4Input = true;
        //		}
        //	}
        //	if (this.gameObject.tag == "Player3" && Keeper.p3Input != true) {
        //		Keeper.P3WINS += Player3W;
        //		Keeper.p3Input = true;
        //		Debug.Log (Keeper.P3WINS + " P3");
        //		if (Keeper.numOfP == 1) {
        //			Keeper.p1Input = true;
        //			Keeper.p2Input = true;
        //			Keeper.p4Input = true;
        //		}
        //	}
        //	if (this.gameObject.tag == "Player4" && Keeper.p4Input != true) {
        //		Keeper.P4WINS += Player4W;
        //		Keeper.p4Input = true;
        //		Debug.Log (Keeper.P4WINS + " P4");
        //		if (Keeper.numOfP == 1) {
        //			Keeper.p1Input = true;
        //			Keeper.p2Input = true;
        //			Keeper.p3Input = true;
        //		}
        //	}
        //} 
        //else if (Keeper.ifDied && numOfRounds <= 0 && (this.gameObject.tag == "Player" | this.gameObject.tag == "Player2" | this.gameObject.tag == "Player3" | this.gameObject.tag == "Player4")) {
        //	if (winAdd1) {
        //		Keeper.P1WINS += Player1W;
        //		Keeper.P2WINS += Player2W;
        //		Keeper.P3WINS += Player3W;
        //		Keeper.P4WINS += Player4W;
        //		winAdd1 = false;
        //	}

        //if (Keeper.preNumOfP == 4) {
        //	firstWins.text = "Player 1: " + Keeper.P1WINS + " wins" +
        //	" Player 2: " + Keeper.P2WINS + " wins" +
        //	" Player 3: " + Keeper.P3WINS + " wins" +
        //	" Player 4: " + Keeper.P4WINS + " wins";
        //} 
        //else if (Keeper.preNumOfP == 3) {
        //	firstWins.text = "Player 1: " + Keeper.P1WINS + " wins" +
        //	" Player 2: " + Keeper.P2WINS + " wins" +
        //	" Player 3: " + Keeper.P3WINS + " wins";
        //} 
        //else if (Keeper.preNumOfP == 2) {
        //	firstWins.text = "Player 1: " + Keeper.P1WINS + " wins" +
        //	" Player 2: " + Keeper.P2WINS + " wins";
        //}
        //	P1exists = false;
        //	P2exists = false;
        //	P3exists = false;
        //	P4exists = false;
        //	dontChange = false;
        //	WinScreen.SetActive (true);
        //}
        //if (dontChange && p != null) {
        //	if (this.gameObject.tag == "Player") {
        //		P1exists = true;
        //	}
        //	if (this.gameObject.tag == "Player2") {
        //		P2exists = true;
        //	}
        //	if (this.gameObject.tag == "Player3") {
        //		P3exists = true;
        //	}
        //	if (this.gameObject.tag == "Player4") {
        //		P4exists = true;
        //	}
        //}
        //if (P1exists) {
        //	if (Keeper.numOfP == 1 && Keeper.death2 && Keeper.death3 && Keeper.death4) {
        //		Keeper.ifDied = true;
        //		Player1W += 1;
        //		P1exists = false;
        //	}
        //	if (Keeper.numOfP == 1 && Keeper.death2 && Keeper.death3 && Keeper.death4) {
        //		Keeper.ifDied = true;
        //		Player1W += 1;
        //		P1exists = false;
        //		Keeper.death2 = false;
        //		Keeper.numOfP -= 1;
        //	}
        //	else if (p.shotsHaveFired && Keeper.numOfP == 1 && Keeper.death2 && Keeper.death3 && Keeper.death4) {
        //		Keeper.ifDied = true;
        //		Player1W += 1;
        //		p.shotsHaveFired = false;
        //		P1exists = false;
        //		Keeper.death2 = false;
        //		Keeper.numOfP -= 1;
        //	}
        //	else if (Keeper.hasHit2) {
        //		Keeper.ifDied = true;
        //		Player1W += 1;
        //		P1exists = false;
        //		Keeper.hasHit2 = false;
        //		Keeper.numOfP -= 1;

        //	}
        //}
        //else if (P2exists) {
        //	if (Keeper.death) {
        //		Keeper.ifDied = true;
        //		Player2W += 1;
        //		P1exists = false;
        //		Keeper.death = false;
        //		Keeper.numOfP -= 1;
        //	}
        //	else if (p.shotsHaveFired2 && Keeper.death) {
        //		Keeper.ifDied = true;
        //		Player2W += 1;
        //		p.shotsHaveFired2 = false;
        //		P2exists = false;
        //		Keeper.death = false;
        //		Keeper.numOfP -= 1;
        //	}
        //	else if (Keeper.hasHit1) {
        //		Keeper.ifDied = true;
        //		Player2W += 1;
        //		P2exists = false;
        //		Keeper.hasHit1 = false;
        //		Keeper.numOfP -= 1;
        //	} 
        //}
    }

	public void RestartLevel() {
		Application.LoadLevel(Application.loadedLevel);
	}

	public void ExitLevel (){
		Keeper.P1WINS = 0;
		Keeper.P2WINS = 0;
		Application.LoadLevel (0);
		Keeper.numberOfRounds = 0;
		Keeper.preNumOfP = 0;
		Keeper.previousRounds = 0;
		Keeper.ifDied = false;
	}

//	public void PlayGame() {
//		Application.LoadLevel (8);
//	}
	public void ExitGame() {
		Application.Quit ();
	}
	public void MainMenu() {
		if (firstTime) {
			MainMenuSlide = true;
			playBack = false;
			setTimer = true;
            layer = 1;
            firstLayer = true;
		}
	}
	public void GoBack() {
		playBack = true;
		MainMenuSlide = false;
		setTimer = true;
        //if (secondLayer)
        //{
        //    firstLayer = true;
        //    secondLayer = false;
        //}
        //else if (thirdLayer)
        //{
        //    secondLayer = true;
        //    thirdLayer = false;
        //}
        //else if (fourthLayer)
        //{
        //    thirdLayer = true;
        //    fourthLayer = false;
        //}
        //else if (fifthLayer)
        //{
        //    fourthLayer = true;
        //    fifthLayer = false;
        //}
        //else if (sixthLayer)
        //{
        //    fifthLayer = true;
        //    sixthLayer = false;
        //}
        if (layer > 1)      // Now you just need to check the value of layer
            layer--;        // instead of individual bools
	}

	public void Next() {
		playBack = true;
		MainMenuSlide = false;
		setTimer = true;
        //if (secondLayer)
        //{
        //    thirdLayer = true;
        //    secondLayer = false;
        //}
        //else if (thirdLayer)
        //{
        //    fourthLayer = true;
        //    thirdLayer = false;
        //}
        //else if (fourthLayer)
        //{
        //    fifthLayer = true;
        //    fourthLayer = false;
        //}
        //else if (fifthLayer)
        //{
        //    sixthLayer = true;
        //    fifthLayer = false;
        //}
        if (layer < 6)
            layer++;
	}

    //public void twoPlayers()
    //{
    //    Keeper.numOfP = 2;
    //    Keeper.preNumOfP = Keeper.numOfP;
    //    Keeper.death3 = true;
    //    Keeper.death4 = true;
    //    Next();
    //}
    //public void threePlayers()
    //{
    //    Keeper.numOfP = 3;
    //    Keeper.preNumOfP = Keeper.numOfP;
    //    Keeper.death4 = true;
    //    Next();
    //}
    //public void fourPlayers()
    //{
    //    Keeper.numOfP = 4;
    //    Keeper.preNumOfP = Keeper.numOfP;
    //    Next();
    //}

    /// <summary>
    /// Call to set the number of players in a game
    /// </summary>
    public void Players (int p)
    {
        //Keeper.numOfP = p;
        //Keeper.preNumOfP = p;
        totalPlayers = p;
        activePlayers = p;
        Debug.Log(activePlayers);
        for (int i = 0; i < p; i++)
        {
            playerStatuses[i] = true;
        }
        Next();
    }

	public void PlayButton () {
		playBack = true;
		MainMenuSlide = false;
        firstLayer = false;
        secondLayer = true;
        layer = 2;
		setTimer = true;

	}

    //public void Three()
    //{
    //    numOfRounds = 3;
    //    Keeper.previousRounds = 3;
    //    startUp = true;
    //}
    //public void Five()
    //{
    //    numOfRounds = 5;
    //    Keeper.previousRounds = 5;
    //    startUp = true;
    //}
    //public void Seven()
    //{
    //    numOfRounds = 7;
    //    Keeper.previousRounds = 7;
    //    startUp = true;
    //}
    //public void Ten()
    //{
    //    numOfRounds = 10;
    //    Keeper.previousRounds = 10;
    //    startUp = true;
    //}

    /// <summary>
    /// Call to set the number of rounds
    /// </summary>
    public void SetRounds(int r)
    {
        numOfRounds = r;
        Keeper.previousRounds = r;
        startUp = true;
    }

	public void RedoRounds() {
        playerStatuses = new bool[4];
        winCounts = new int[4];
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

	public void Tutorial() {
		TutorialScene = true;
		Keeper.previousRounds = 1;
		Application.LoadLevel (11);
	}

    public void KillPlayer (string player)
    {
        int playerNumber = int.Parse(player[player.Length - 1] + "");
        activePlayers--;
        Debug.Log(activePlayers);
        playerStatuses[playerNumber - 1] = false;
        if (activePlayers == 1)
        {
            for (int i=0; i<4; i++)
            {
                if (playerStatuses[i])
                {
                    winCounts[i]++;
                    if (winCounts[i]==numOfRounds)
                    {
                        winner = "P" + (i + 1);
                    }
                }
            }
        }
    }

    public void P1Lock()
    {
        if (P1C)
        {
            P1C = false;
        }
        else
        {
            P1C = true;
        }
    }
    public void P2Lock()
    {
        if (P2C)
        {
            P2C = false;
        }
        else
        {
            P2C = true;
        }
    }
    public void P3Lock()
    {
        if (P3C)
        {
            P3C = false;
        }
        else
        {
            P3C = true;
        }
    }
    public void P4Lock()
    {
        if (P4C)
        {
            P4C = false;
        }
        else
        {
            P4C = true;
        }
    }

    public void PlayerLock(int p)
    {

    }

    public void UP()
    {
        if (this.gameObject.tag == "PC1")
        {
            PCONE += 1;
            if (PCONE <= 5)
            {
                PCONE = 1;
            }
        }
        if (this.gameObject.tag == "PC2")
        {
            PCTWO += 1;
            if (PCTWO <= 5)
            {
                PCTWO = 1;
            }
        }
        if (this.gameObject.tag == "PC3")
        {
            PCTHREE += 1;
            if (PCTHREE <= 5)
            {
                PCTHREE = 1;
            }
        }
        if (this.gameObject.tag == "PC4")
        {
            PCFOUR += 1;
            if (PCFOUR <= 5)
            {
                PCFOUR = 1;
            }
        }
    }
    public void DOWN()
    {
        if (this.gameObject.tag == "PC1")
        {
            PCONE -= 1;
            if (PCONE >= 0)
            {
                PCONE = 4;
            }
        }
        if (this.gameObject.tag == "PC2")
        {
            PCTWO -= 1;
            if (PCTWO >= 0)
            {
                PCTWO = 4;
            }
        }
        if (this.gameObject.tag == "PC3")
        {
            PCTHREE -= 1;
            if (PCTHREE >= 0)
            {
                PCTHREE = 4;
            }
        }
        if (this.gameObject.tag == "PC4")
        {
            PCFOUR -= 1;
            if (PCFOUR >= 0)
            {
                PCFOUR = 4;
            }
        }
    }
}
