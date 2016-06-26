using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {
	public bool MainMenuSlide = false;
	public bool Back = false;

	// Use this for initialization
	void Start () {
	
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
	}
	public void GoBack() {
		Back = true;
	}
}
