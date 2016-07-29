using UnityEngine;
using System.Collections;

public class WallScript : MonoBehaviour {

    public bool bounceConstraint = false;

	void OnTriggerEnter2D (Collider2D collider) {
        PlayerWeaponScript shot = collider.gameObject.GetComponent<PlayerWeaponScript>();
		if (shot != null && (shot.fire || shot.aim) && !shot.rock) {
            shot.connected = true;
		}
	}

    void OnTriggerExit2D(Collider2D collider)
    {
        PlayerWeaponScript shot = collider.gameObject.GetComponent<PlayerWeaponScript>();
        if (shot != null && shot.fire && !shot.rock)
        {
            shot.connected = true;
        }
    }
}
