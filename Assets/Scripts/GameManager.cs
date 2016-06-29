using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {
    public HealthScript p1;
    public Health2Script p2;
	public SpikeScript spikes;
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
	private int Player1W = 0;
	private int Player2W = 0;

	// Use this for initialization
	void Start () {
        p1 = gameObject.GetComponent<HealthScript>();
        p2 = gameObject.GetComponent<Health2Script>();
		Keeper = GameObject.Find("NumberKeeper").GetComponent<NumberKeeper> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (startUp) {
			randomLevel = Random.Range (1, 9);
			Application.LoadLevel (randomLevel);
			numOfRounds -= 1;
			Keeper.numberOfRounds = numOfRounds;
			startUp = false;
		}
		numOfRounds = Keeper.numberOfRounds;
		if (ifDied && numOfRounds > 0) {
			randomLevel = Random.Range (1, 9);
			Application.LoadLevel (randomLevel);
			Player1W = Keeper.P1WINS;
			Player2W = Keeper.P2WINS;
			numOfRounds -= 1;
			Keeper.numberOfRounds = numOfRounds;
			ifDied = false;
		} else if (ifDied && numOfRounds <= 0) {
			//setActive winscreen
		}
		if (p1 != null) {
			P1exists = true;
		}
		if (p2 != null) {
			P2exists = true;
		}
		if (P1exists) {
			if (p1 != null && p1.hp == 0) {
				ifDied = true;
				if (this.gameObject.tag == "Player") {
					Player2W += 1;
					Keeper.P2WINS = Player2W;
					P1exists = false;
				}
			}
			if (p2 == null) {
				if (p1.shotsHaveFired) {
					ifDied = true;
					Player1W += 1;
					Keeper.P1WINS = Player1W;
					p1.shotsHaveFired = false;
					P1exists = false;
				}
				if (Keeper.hasHit) {
					ifDied = true;
					Player1W += 1;
					Keeper.P1WINS = Player1W;
					P1exists = false;
					Keeper.hasHit = false;

				}
			}
		}
		else if (P2exists) {
			if (p2 != null && p2.hp == 0) {
				ifDied = true;
				if (this.gameObject.tag == "Player2") {
					Player1W += 1;
					Keeper.P1WINS = Player1W;
					P2exists = false;
				}
				P2exists = false;
			}
			if (p1 == null) {
				if (p2.shotsHaveFired) {
					ifDied = true;
					Player2W += 1;
					Keeper.P2WINS = Player2W;
					p2.shotsHaveFired = false;
					P2exists = false;
				}
				if (Keeper.hasHit) {
					ifDied = true;
					Player2W += 1;
					Keeper.P2WINS = Player2W;
					P2exists = false;
					Keeper.hasHit = false;
					Debug.Log ("this is Player2 wins " + Keeper.P2WINS);
				} 
			}
		}
	}

	public void RestartLevel() {
		Application.LoadLevel(Application.loadedLevel);
	}

	public void ExitLevel (){
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
		startUp = true;
	}
	public void Five() {
		numOfRounds = 5;
		startUp = true;
	}
	public void Seven() {
		numOfRounds = 7;
		startUp = true;
	}
	public void Ten() {
		numOfRounds = 10;
		startUp = true;
	}
}
