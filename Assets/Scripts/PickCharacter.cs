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

}
