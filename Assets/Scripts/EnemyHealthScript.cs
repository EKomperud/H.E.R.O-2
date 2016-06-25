using UnityEngine;
using System.Collections;

public class EnemyHealthScript : MonoBehaviour {

	public int hp = 2;
	private EnemyScript enemy; 

	void Awake () {
		enemy = gameObject.GetComponent<EnemyScript> ();
	}

	void OnTriggerEnter2D (Collider2D collider) {
		//player = gameObject.GetComponent<PlayerScript>();
		PlayerWeaponScript shot = collider.gameObject.GetComponent<PlayerWeaponScript> ();
		if (shot != null) {			
			hp -= shot.damage;
			Destroy (shot.gameObject);
			if (hp <= 0) {
				Destroy (gameObject);
			}			
		}
	}
}
