using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {
    public HealthScript p1;
    public Health2Script p2;
	public bool MainMenuSlide = false;
	public bool playBack = false;
	public bool setTimer = false;
    public bool ifDied = false;
	public int buttonCountFirst = 0;
	private RoundKeeper Keeper;

	// Use this for initialization
	void Start () {
		//Keeper = GameObject.Find("RoundKeeper").GetComponent<RoundKeeper> ();
        p1 = gameObject.GetComponent<HealthScript>();
        p2 = gameObject.GetComponent<Health2Script>();
	}
	
	// Update is called once per frame
	void Update () {
	    if(ifDied)
        {
            Application.LoadLevel(8);
            ifDied = false;
        }
        if (p1 != null && p1.hp == 0)
        {
            ifDied = true;
        }
        else if (p2 != null && p2.hp == 0)
        {
            ifDied = true;
        }
	}

	public void RestartLevel() {
		Application.LoadLevel(Application.loadedLevel);
	}

	public void ExitLevel (){
		Application.LoadLevel (8);
	}

	public void PlayGame() {
		Application.LoadLevel (8);
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
