using UnityEngine;
using System.Collections;

public class RockScript : MonoBehaviour {

	public PlayerController caster;
	public Player2Controller caster2;

	void OnTriggerEnter2D (Collider2D collider) {
		caster = collider.gameObject.GetComponent<PlayerController> ();
		caster2 = collider.gameObject.GetComponent<Player2Controller> ();
		if (caster != null) {
			caster.nearRock = true;
			caster.rock = this;
		}
		if (caster2 != null) {

		}
	}

	void OnTriggerExit2D (Collider2D collider) {
		PlayerController player = collider.gameObject.GetComponent<PlayerController> ();
		Player2Controller player2 = collider.gameObject.GetComponent<Player2Controller> ();
		if (player != null) {
			player.nearRock = false;
			player.rock = this;

		}
		if (player2 != null) {

		}
	}
}
