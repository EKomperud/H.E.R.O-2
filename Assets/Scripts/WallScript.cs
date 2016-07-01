using UnityEngine;
using System.Collections;

public class WallScript : MonoBehaviour {

	void OnTriggerEnter2D (Collider2D collider) {
        PlayerWeaponScript shot = collider.gameObject.GetComponent<PlayerWeaponScript>();
		if (shot != null && shot.fire && !shot.rock) {			
			shot.Destroy ();
		}
	}
}
