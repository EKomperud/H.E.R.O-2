using UnityEngine;
using System.Collections;

public class RockScript : MonoBehaviour {

	public PlayerController caster;

    void Start()
    {
        GetComponent<Rigidbody2D>().collisionDetectionMode = CollisionDetectionMode2D.Continuous;
    }

	void OnTriggerEnter2D (Collider2D collider) {
		caster = collider.gameObject.GetComponent<PlayerController> ();
		if (caster != null) {
			caster.nearRock = true;
			caster.rock = this;
		}
	}

	void OnTriggerExit2D (Collider2D collider) {
		PlayerController player = collider.gameObject.GetComponent<PlayerController> ();
		if (player != null) {
			player.nearRock = false;
			player.rock = this;

		}
	}
}
