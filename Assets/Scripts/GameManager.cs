using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {
	public bool MainMenuSlide = false;
	public bool playBack = false;
	public bool setTimer = false;
	public int buttonCountFirst = 0;
	private RoundKeeper Keeper;

	// Use this for initialization
	void Start () {
		Keeper = GameObject.Find("RoundKeeper").GetComponent<RoundKeeper> ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void RestartLevel() {
		Application.LoadLevel(Application.loadedLevel);
	}

	public void ExitLevel (){
		Application.LoadLevel (0);
	}

	public void PlayGame() {
		Application.LoadLevel (1);
	}

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
		Keeper.numOfRounds = 3;
		Keeper.newRound = true;
	}
	public void Five() {
		Keeper.numOfRounds = 5;
		Keeper.newRound = true;
	}
	public void Seven() {
		Keeper.numOfRounds = 7;
		Keeper.newRound = true;
	}
	public void Ten() {
		Keeper.numOfRounds = 10;
		Keeper.newRound = true;
	}
}
