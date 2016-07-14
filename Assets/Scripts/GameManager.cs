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
    public bool ifDied = false;
	public int buttonCountFirst = 0;
	private int numOfRounds = 0;
	private int randomLevel = 0;
	private bool startUp = false;
	private bool P1exists = false;
	private bool P2exists = false;
	public NumberKeeper Keeper;
	public int Player1W = 0;
	public int Player2W = 0;
	public Text firstWins;
	private bool dontChange = true;

	// Use this for initialization
	void Start () {
		Keeper = GameObject.Find("NumberKeeper").GetComponent<NumberKeeper> ();
		if (this.gameObject.tag == "Player" | this.gameObject.tag == "Player2") {
			firstWins.text = "";
		}
        p = gameObject.GetComponent<HealthScript>();
	}
	
	// Update is called once per frame
	void Update () {
		if (startUp) {
			randomLevel = Random.Range (1, 8);
			Application.LoadLevel (randomLevel);
			numOfRounds -= 1;
			Keeper.numberOfRounds = numOfRounds;
			startUp = false;
		}
		numOfRounds = Keeper.numberOfRounds;
		if (ifDied && numOfRounds > 0) {
			Debug.Log (Keeper.P1WINS);
			randomLevel = Random.Range (1, 8);
			Application.LoadLevel (randomLevel);
			Debug.Log (Keeper.P1WINS);
			Player1W = Keeper.P1WINS;
			Player2W = Keeper.P2WINS;
			numOfRounds -= 1;
			Keeper.numberOfRounds = numOfRounds;
			ifDied = false;
		} 
		else if (ifDied && numOfRounds <= 0) {
			if (Keeper.P2WINS == 1) {
				firstWins.text = "Player 1: " + Keeper.P1WINS + " wins" +
				" Player 2: " + Keeper.P2WINS + " win";
			} 
			else if (Keeper.P1WINS == 1) {
				firstWins.text = "Player 1: " + Keeper.P1WINS + " win" +
				" Player 2: " + Keeper.P2WINS + " wins";
			} 
			else {
				firstWins.text = "Player 1: " + Keeper.P1WINS + " wins" +
				" Player 2: " + Keeper.P2WINS + " wins";
			}
			P1exists = false;
			P2exists = false;
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
		}
		if (P1exists) {
			if (Keeper.death2) {
				ifDied = true;
				Player1W += 1;
				Keeper.P1WINS = Player1W;
				P1exists = false;
				Keeper.death2 = false;
			}
			else if (p.shotsHaveFired && Keeper.death2) {
				ifDied = true;
				Player1W += 1;
				Keeper.P1WINS = Player1W;
				p.shotsHaveFired = false;
				P1exists = false;
				Keeper.death2 = false;
			}
			else if (Keeper.hasHit2) {
				ifDied = true;
				Player1W += 1;
				Keeper.P1WINS = Player1W;
				P1exists = false;
				Keeper.hasHit2 = false;

			}
		}
		else if (P2exists) {
			if (Keeper.death) {
				ifDied = true;
				Player2W += 1;
				Keeper.P2WINS = Player2W;
				P1exists = false;
				Keeper.death = false;
			}
			else if (p.shotsHaveFired2 && Keeper.death) {
				ifDied = true;
				Player2W += 1;
				Keeper.P2WINS = Player2W;
				p.shotsHaveFired2 = false;
				P2exists = false;
				Keeper.death = false;
			}
			else if (Keeper.hasHit1) {
				ifDied = true;
				Player2W += 1;
				Keeper.P2WINS = Player2W;
				P2exists = false;
				Keeper.hasHit1 = false;
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
	}

//	public void PlayGame() {
//		Application.LoadLevel (8);
//	}

	public void ExitGame() {
		Application.Quit ();
	}
	public void MainMenu() {
		MainMenuSlide = true;
		playBack = false;
		setTimer = true;
	}
	public void GoBack() {
		playBack = true;
		MainMenuSlide = false;
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
		Keeper.numberOfRounds = Keeper.previousRounds;
		randomLevel = Random.Range (1, 8);
		Application.LoadLevel (randomLevel);

	}
}
