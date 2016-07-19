using UnityEngine;
using System.Collections;

public class RockScript : MonoBehaviour {

	public PlayerController player;
    private CircleCollider2D pickupCollider;

    void Start()
    {
        GetComponent<Rigidbody2D>().collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        pickupCollider = gameObject.GetComponent<CircleCollider2D>();
    }

	void OnTriggerEnter2D (Collider2D collider) {
        PlayerWeaponScript projectile = collider.gameObject.GetComponent<PlayerWeaponScript>();
        if (player == null)
        {
            player = collider.gameObject.GetComponent<PlayerController>();
            if (player != null)
            {
                player.nearRock = true;
                player.rock = this;
            }
        }
	}

	void OnTriggerExit2D (Collider2D collider) {
		PlayerController p = collider.gameObject.GetComponent<PlayerController> ();

		if (player != null) {

            if (p.Equals(player))
            {
                player.nearRock = false;
                player.rock = null;
                player = null;
            }
        }
	}
}
