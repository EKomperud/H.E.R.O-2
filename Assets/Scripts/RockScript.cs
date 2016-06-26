using UnityEngine;
using System.Collections;

public class RockScript : MonoBehaviour {

	public PlayerWeaponScript caster;
	public Player2WeaponScript caster2;

	void OnTriggerEnter2D (Collider2D collider) {
		PlayerController player = collider.gameObject.GetComponent<PlayerController> ();
		Player2Controller player2 = collider.gameObject.GetComponent<Player2Controller> ();
		if (player != null) {
			player.nearRock = true;
			player.rock = this;
		}
		if (player2 != null) {

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
