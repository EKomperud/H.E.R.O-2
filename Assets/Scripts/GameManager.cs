using UnityEngine;
using System.Collections;

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

	// Use this for initialization
	void Start () {
		//Keeper = GameObject.Find("RoundKeeper").GetComponent<RoundKeeper> ();
        p1 = gameObject.GetComponent<HealthScript>();
        p2 = gameObject.GetComponent<Health2Script>();
		spikes = gameObject.GetComponent<SpikeScript> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (startUp) {
			randomLevel = Random.Range (1, 8);
			Application.LoadLevel (randomLevel);
			numOfRounds -= 1;
			startUp = false;
		}
		if (ifDied && numOfRounds > 0) {
			randomLevel = Random.Range (1, 8);
			Application.LoadLevel (randomLevel);
			numOfRounds -= 1;
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
		if (p1 != null && p1.hp == 0) {
			ifDied = true;
		} 
		else if (p2 != null && p2.hp == 0) {
			ifDied = true;
		}
		if (P1exists) {
			if (p1 == null) {
				Debug.Log ("the code gets here");
				if (p2.shotsHaveFired) {
					ifDied = true;
					p2.shotsHaveFired = false;
					P1exists = false;
				}
				if (spikes != null) {
					if (spikes.hasHit) {
						ifDied = true;
						P1exists = false;
					}
				}
			}
		}
		else if (P2exists) {
			if (p2 == null) {
				if (p1.shotsHaveFired) {
					ifDied = true;
					p1.shotsHaveFired = false;
					P2exists = false;
				}
				if (spikes != null) {
					if (spikes.hasHit) {
						ifDied = true;
						P2exists = false;
					}
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
