using UnityEngine;
using System.Collections;

public class MenuChange : MonoBehaviour {
	public GameObject MainScreen;
	public GameObject NumOFP;
	public GameObject CharSel;
	public GameObject Selection;
	public GameObject Rounds;
	public GameObject picOne;
	public GameObject picTwo;
	public GameObject picThree;
	public GameObject picFour;
	//public GameObject LevelSelect;
	public GameObject door;
	private GameManager Menu;


	// Use this for initialization
	void Start () {
		Menu = GameObject.Find ("GameManager").GetComponent<GameManager> ();
	}
	
	// Update is called once per frame
	void Update () {

		if (Menu.layer == 1 && Menu.doorDown) {
            Debug.Log("option one");
			MainScreen.SetActive (true);
			NumOFP.SetActive (false);
			CharSel.SetActive (false);
			Selection.SetActive (false);
			Rounds.SetActive (false);
			//LevelSelect.SetActive (false);
		}
        
		else if (Menu.layer == 1 && Menu.firstTime) {
            Debug.Log("option two");
            MainScreen.SetActive (true);
			NumOFP.SetActive (false);
			CharSel.SetActive (false);
			Selection.SetActive (false);
			Rounds.SetActive (false);
			//LevelSelect.SetActive (false);
			Menu.firstTime = false;
		}
		else if (Menu.layer == 2 && Menu.doorDown) {
			MainScreen.SetActive (false);
			NumOFP.SetActive (true);
			CharSel.SetActive (false);
			Selection.SetActive (false);
			Rounds.SetActive (false);
			//LevelSelect.SetActive (false);
		}
		else if (Menu.layer == 3 && Menu.doorDown) {
			MainScreen.SetActive (false);
			NumOFP.SetActive (false);
			CharSel.SetActive (true);
			Selection.SetActive (false);
			Rounds.SetActive (false);
			if (Menu.Keeper.numOfP == 2) {
				picThree.SetActive (false);
				picFour.SetActive (false);
			}
			else if (Menu.Keeper.numOfP == 3) {
				picThree.SetActive (true);
				picFour.SetActive (false);
			} 
			else {
				picThree.SetActive (true);
				picFour.SetActive (true);
			}
			//LevelSelect.SetActive (false);
		}
		else if (Menu.layer == 4 && Menu.doorDown) {
			MainScreen.SetActive (false);
			NumOFP.SetActive (false);
			CharSel.SetActive (false);
			Selection.SetActive (true);
			Rounds.SetActive (false);
			//LevelSelect.SetActive (false);
		}
		else if (Menu.layer == 5 && Menu.doorDown) {
			MainScreen.SetActive (false);
			NumOFP.SetActive (false);
			CharSel.SetActive (false);
			Selection.SetActive (false);
			Rounds.SetActive (true);
			//LevelSelect.SetActive (false);
		}
		else if (Menu.layer == 6 && Menu.doorDown) {
			MainScreen.SetActive (false);
			NumOFP.SetActive (false);
			CharSel.SetActive (false);
			Selection.SetActive (false);
			Rounds.SetActive (false);
			//LevelSelect.SetActive (true);
		}
		
	}
}
