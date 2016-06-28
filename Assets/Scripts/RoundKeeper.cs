using UnityEngine;
using System.Collections;

public class RoundKeeper : MonoBehaviour {

	public int numOfRounds = 0;
	public int P1WINS = 0;
	public int P2WINS = 0;
	private HealthScript P1Loss;
	private Health2Script P2Loss;
	private PlayerController player1;
	private Player2Controller player2;
	public bool newRound = false;
	public bool winScreen = false;
	private int randomLevel = 0;
	private bool endRoundP1 = false;
	private bool endRoundP2 = false;
	void Start () {
		DontDestroyOnLoad (this.gameObject);
	}
	
	// Update is called once per frame
	void Update () {
		if (numOfRounds > 0 && newRound) {
			randomLevel = Random.Range (1, 5);
			Application.LoadLevel (randomLevel);
			Debug.Log ("numberCount");
			if (randomLevel > 0 && randomLevel < 9) {
				P1Loss = GameObject.Find ("Player").GetComponent<HealthScript> ();
				P2Loss = GameObject.Find ("Player2").GetComponent<Health2Script> ();
				player1 = GameObject.Find ("Player").GetComponent<PlayerController> ();
				player2 = GameObject.Find ("Player2").GetComponent<Player2Controller> ();
				P1Loss.hp = 5;
				P2Loss.hp = 5;
				Debug.Log (P1Loss.hp);
			}
			newRound = false;
		} 
		else if (numOfRounds == 0) {
			winScreen = true;
		} 
		else {
			winScreen = false;
		}
		if (winScreen) {
			
		}
		if (randomLevel > 0 && randomLevel < 9) {
			if (P1Loss.hp == 0 | player1.fallDeath) {
				endRoundP1 = true;
			}
			if (P2Loss.hp == 0 | player2.fallDeath) {
				endRoundP2 = true;
			}
			if (endRoundP1) {
				P2WINS += 1;
				endRoundP1 = false;
				newRound = true;
			}
			if (endRoundP2) {
				P1WINS += 1;
				endRoundP2 = false;
				newRound = true;
			}
		}
	}
}
