//using UnityEngine;
//using System.Collections;
//
//public class currentMenu : MonoBehaviour {
//	public GameObject MainScreen;
//	public GameObject CharSel;
//	public GameObject Selection;
//	public GameObject Rounds;
//	public GameObject LevelSelect;
//	
//	public enum whichMenu {
//		firstLayer,
//		secondLayer,
//		thirdLayer,
//		fourthLayer,
//		fifthLayer,
//		none
//	}
//	public StateMachineBehaviour state;
//
//	IEnumerator firstLayerS () {
//		MainScreen.SetActive (true);
//		CharSel.SetActive (false);
//		Selection.SetActive (false);
//		Rounds.SetActive (false);
//		LevelSelect.SetActive (false);
//	}
//
//	IEnumerator secondLayerS () {
//		MainScreen.SetActive (false);
//		CharSel.SetActive (true);
//		Selection.SetActive (false);
//		Rounds.SetActive (false);
//		LevelSelect.SetActive (false);
//	}
//
//	IEnumerator thirdLayerS () {
//		MainScreen.SetActive (false);
//		CharSel.SetActive (false);
//		Selection.SetActive (true);
//		Rounds.SetActive (false);
//		LevelSelect.SetActive (false);
//	}
//
//	IEnumerator fourthLayerS () {
//		MainScreen.SetActive (false);
//		CharSel.SetActive (false);
//		Selection.SetActive (false);
//		Rounds.SetActive (true);
//		LevelSelect.SetActive (false);
//	}
//
//	IEnumerator fifthLayerS () {
//		MainScreen.SetActive (false);
//		CharSel.SetActive (false);
//		Selection.SetActive (false);
//		Rounds.SetActive (false);
//		LevelSelect.SetActive (true);
//	}
//	
//	// Update is called once per frame
//	void changeState (int newState) {
//		if (newState == 1) {
//			StartCoroutine ((IEnumerator)Invoke (firstLayerS, null));
//		}
//		else if (newState == 2) {
//			StartCoroutine ((IEnumerator)Invoke (secondLayerS, null));
//		}
//		else if (newState == 3) {
//			StartCoroutine ((IEnumerator)Invoke (thirdLayerS, null));
//		}
//		else if (newState == 4) {
//			StartCoroutine ((IEnumerator)Invoke (fourthLayerS, null));
//		}
//		else if (newState == 5) {
//			StartCoroutine ((IEnumerator)Invoke (fifthLayerS, null));
//		}
//	}
//}
