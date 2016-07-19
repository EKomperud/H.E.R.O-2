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
	public bool doorDown = false;
	public bool doorUp = false;
	public bool firstTime = true;
	public bool TutorialScene = false;
	public NumberKeeper Keeper;
	public int Player1W = 0;
	public int Player2W = 0;
	public int Player3W = 0;
	public int Player4W = 0;
	public int pNum = 0;
	public Text firstWins;
	private bool dontChange = true;
	private bool winAdd1 = true;

	// Use this for initialization
	void Start () {
		Keeper = GameObject.Find("NumberKeeper").GetComponent<NumberKeeper> ();
		if (this.gameObject.tag == "Player" | this.gameObject.tag == "Player2" | this.gameObject.tag == "Player3" | this.gameObject.tag == "Player4") {
			firstWins.text = "";
		}
        p = gameObject.GetComponent<HealthScript>();
	}
	
	// Update is called once per frame
	void Update () {
		if (startUp) {
			pNum = Keeper.numOfP;
			randomLevel = Random.Range (1, 8);
			Application.LoadLevel (randomLevel);
			numOfRounds -= 1;
			Keeper.numberOfRounds = numOfRounds;
			startUp = false;
		}
		numOfRounds = Keeper.numberOfRounds;
		if (Keeper.p2Input && Keeper.p1Input && Keeper.p3Input && Keeper.p4Input) {
			randomLevel = Random.Range (1, 8);
			Application.LoadLevel (randomLevel);
			numOfRounds -= 1;
			Keeper.numberOfRounds = numOfRounds;
			Keeper.numOfP = Keeper.preNumOfP;
			Keeper.ifDied = false;
			Keeper.p1Input = false;
			Keeper.p2Input = false;
			Keeper.p3Input = false;
			Keeper.p4Input = false;
		}
		if (Keeper.ifDied && numOfRounds > 0 && Keeper.numOfP == 1) {
			if (this.gameObject.tag == "Player" && Keeper.p1Input != true) {
				Keeper.P1WINS += Player1W;
				Keeper.p1Input = true;
				Debug.Log (Keeper.P1WINS + " P1");
				if (Keeper.numOfP == 1) {
					Keeper.p2Input = true;
					Keeper.p3Input = true;
					Keeper.p4Input = true;
				}
			}
			if (this.gameObject.tag == "Player2" && Keeper.p2Input != true) {
				Keeper.P2WINS += Player2W;
				Keeper.p2Input = true;
				Debug.Log (Keeper.P2WINS + " P2");
				if (Keeper.numOfP == 1) {
					Keeper.p1Input = true;
					Keeper.p3Input = true;
					Keeper.p4Input = true;
				}
			}
			if (this.gameObject.tag == "Player3" && Keeper.p3Input != true) {
				Keeper.P3WINS += Player3W;
				Keeper.p3Input = true;
				Debug.Log (Keeper.P3WINS + " P3");
				if (Keeper.numOfP == 1) {
					Keeper.p1Input = true;
					Keeper.p2Input = true;
					Keeper.p4Input = true;
				}
			}
			if (this.gameObject.tag == "Player4" && Keeper.p4Input != true) {
				Keeper.P4WINS += Player4W;
				Keeper.p4Input = true;
				Debug.Log (Keeper.P4WINS + " P4");
				if (Keeper.numOfP == 1) {
					Keeper.p1Input = true;
					Keeper.p2Input = true;
					Keeper.p3Input = true;
				}
			}
		} 
		else if (Keeper.ifDied && numOfRounds <= 0 && (this.gameObject.tag == "Player" | this.gameObject.tag == "Player2" | this.gameObject.tag == "Player3" | this.gameObject.tag == "Player4")) {
			if (winAdd1) {
				Keeper.P1WINS += Player1W;
				Keeper.P2WINS += Player2W;
				Keeper.P3WINS += Player3W;
				Keeper.P4WINS += Player4W;
				winAdd1 = false;
			}
			if (Keeper.preNumOfP == 4) {
				firstWins.text = "Player 1: " + Keeper.P1WINS + " wins" +
				" Player 2: " + Keeper.P2WINS + " wins" +
				" Player 3: " + Keeper.P3WINS + " wins" +
				" Player 4: " + Keeper.P4WINS + " wins";
			} 
			else if (Keeper.preNumOfP == 3) {
				firstWins.text = "Player 1: " + Keeper.P1WINS + " wins" +
				" Player 2: " + Keeper.P2WINS + " wins" +
				" Player 3: " + Keeper.P3WINS + " wins";
			} 
			else if (Keeper.preNumOfP == 2) {
				firstWins.text = "Player 1: " + Keeper.P1WINS + " wins" +
				" Player 2: " + Keeper.P2WINS + " wins";
			}
			P1exists = false;
			P2exists = false;
			P3exists = false;
			P4exists = false;
			dontChange = false;
			WinScreen.SetActive (true);
		}
		if (dontChange && p != null) {
			if (this.gameObject.tag == "Player") {
				P1exists = true;
			}
			if (this.gameObject.tag == "Player2") {
				P2exists = true;
			}
			if (this.gameObject.tag == "Player3") {
				P3exists = true;
			}
			if (this.gameObject.tag == "Player4") {
				P4exists = true;
			}
		}
		if (P1exists) {
			if (Keeper.numOfP == 1 && Keeper.death2 && Keeper.death3 && Keeper.death4) {
				Keeper.ifDied = true;
				Player1W += 1;
				P1exists = false;
			}
			if (Keeper.numOfP == 1 && Keeper.death2 && Keeper.death3 && Keeper.death4) {
				Keeper.ifDied = true;
				Player1W += 1;
				P1exists = false;
				Keeper.death2 = false;
				Keeper.numOfP -= 1;
			}
			else if (p.shotsHaveFired && Keeper.numOfP == 1 && Keeper.death2 && Keeper.death3 && Keeper.death4) {
				Keeper.ifDied = true;
				Player1W += 1;
				p.shotsHaveFired = false;
				P1exists = false;
				Keeper.death2 = false;
				Keeper.numOfP -= 1;
			}
			else if (Keeper.hasHit2) {
				Keeper.ifDied = true;
				Player1W += 1;
				P1exists = false;
				Keeper.hasHit2 = false;
				Keeper.numOfP -= 1;

			}
		}
		else if (P2exists) {
			if (Keeper.death) {
				Keeper.ifDied = true;
				Player2W += 1;
				P1exists = false;
				Keeper.death = false;
				Keeper.numOfP -= 1;
			}
			else if (p.shotsHaveFired2 && Keeper.death) {
				Keeper.ifDied = true;
				Player2W += 1;
				p.shotsHaveFired2 = false;
				P2exists = false;
				Keeper.death = false;
				Keeper.numOfP -= 1;
			}
			else if (Keeper.hasHit1) {
				Keeper.ifDied = true;
				Player2W += 1;
				P2exists = false;
				Keeper.hasHit1 = false;
				Keeper.numOfP -= 1;
			} 
		}
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
			firstLayer = true;
		}
	}
	public void GoBack() {
		playBack = true;
		MainMenuSlide = false;
		setTimer = true;
		if (secondLayer) {
			firstLayer = true;
			secondLayer = false;
		} 
		else if (thirdLayer) {
			secondLayer = true;
			thirdLayer = false;
		} 
		else if (fourthLayer) {
			thirdLayer = true;
			fourthLayer = false;
		} 
		else if (fifthLayer) {
			fourthLayer = true;
			fifthLayer = false;
		} 
		else if (sixthLayer) {
			fifthLayer = true;
			sixthLayer = false;
		}
	}
	public void Next() {
		playBack = true;
		MainMenuSlide = false;
		setTimer = true;
		if (secondLayer) {
			thirdLayer = true;
			secondLayer = false;
		} 
		else if (thirdLayer) {
			fourthLayer = true;
			thirdLayer = false;
		} 
		else if (fourthLayer) {
			fifthLayer = true;
			fourthLayer = false;
		} 
		else if (fifthLayer) {
			sixthLayer = true;
			fifthLayer = false;
		} 
	}
	public void twoPlayers () {
		Keeper.numOfP = 2;
		Keeper.preNumOfP = Keeper.numOfP;
		Keeper.death3 = true;
		Keeper.death4 = true;
		Next ();
	}
	public void threePlayers () {
		Keeper.numOfP = 3;
		Keeper.preNumOfP = Keeper.numOfP;
		Keeper.death4 = true;
		Next ();
	}
	public void fourPlayers () {
		Keeper.numOfP = 4;
		Keeper.preNumOfP = Keeper.numOfP;
		Next ();
	}
	public void PlayButton () {
		playBack = true;
		MainMenuSlide = false;
		firstLayer = false;
		secondLayer = true;
		setTimer = true;

	}
	public void Three() {
		numOfRounds = 3;
		Keeper.previousRounds = 3;
		startUp = true;
	}
	public void Five() {
		numOfRounds = 5;
		Keeper.previousRounds = 5;
		startUp = true;
	}
	public void Seven() {
		numOfRounds = 7;
		Keeper.previousRounds = 7;
		startUp = true;
	}
	public void Ten() {
		numOfRounds = 10;
		Keeper.previousRounds = 10;
		startUp = true;
	}
	public void RedoRounds() {
		Keeper.P1WINS = 0;
		Keeper.P2WINS = 0;
		numOfRounds = Keeper.previousRounds;
		Keeper.numOfP = Keeper.preNumOfP;
		startUp = true;
		Keeper.ifDied = false;
	}
	public void Tutorial() {
		TutorialScene = true;
		Keeper.previousRounds = 1;
		Application.LoadLevel (11);
	}
}
