using UnityEngine;
using System.Collections;

public class WallScript : MonoBehaviour {

	void OnTriggerEnter2D (Collider2D collider) {
		PlayerWeaponScript shot = collider.gameObject.GetComponent<PlayerWeaponScript> ();
		Player2WeaponScript enemyShot = collider.gameObject.GetComponent<Player2WeaponScript> ();
		if (shot != null) {			
			shot.Destroy ();
		}
		if (enemyShot != null) {
			enemyShot.Destroy ();
		}
	}
}
