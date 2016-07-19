using UnityEngine;
using System.Collections;

public class PickCharacter : MonoBehaviour {
	public GameManager Menu;
	public MenuChange Change;
	public GameObject P1UP;
	public GameObject P2UP;
	public GameObject P3UP;
	public GameObject P4UP;
	public GameObject P1DOWN;
	public GameObject P2DOWN;
	public GameObject P3DOWN;
	public GameObject P4DOWN;

	// Use this for initialization
	void Start () {
		Menu = GameObject.Find ("GameManager").GetComponent<GameManager> ();
		Change = GameObject.Find ("MenuChange").GetComponent<MenuChange> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (Menu.P1C) {
			P1UP.SetActive (false);
			P1DOWN.SetActive (false);
		} 
		else {
			P1UP.SetActive (true);
			P1DOWN.SetActive (true);
		}
		if (Menu.P2C) {
			P2UP.SetActive (false);
			P2DOWN.SetActive (false);
		} 
		else {
			P2UP.SetActive (true);
			P2DOWN.SetActive (true);
		}
		if (Menu.P3C) {
			P3UP.SetActive (false);
			P3DOWN.SetActive (false);
		} 
		else {
			P3UP.SetActive (true);
			P3DOWN.SetActive (true);
		}
		if (Menu.P4C) {
			P4UP.SetActive (false);
			P4DOWN.SetActive (false);
		} 
		else {
			P4UP.SetActive (true);
			P4DOWN.SetActive (true);
		}
	}
}
