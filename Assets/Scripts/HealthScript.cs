using UnityEngine;
using System.Collections;

public class HealthScript : MonoBehaviour {

	public int hp = 5;
	public bool shotsHaveFired = false;
	private PlayerController player; 

	void Awake () {
		player = gameObject.GetComponent<PlayerController> ();
	}

	void OnTriggerEnter2D (Collider2D collider) {
		//player = gameObject.GetComponent<PlayerScript>();
		Player2WeaponScript shot = collider.gameObject.GetComponent<Player2WeaponScript> ();
		if (shot != null) {	
			shotsHaveFired = true;
			hp -= shot.damage;
            System.Threading.Thread d = new System.Threading.Thread(() => shot.Destroy());
            d.Start();
			//Destroy (shot.gameObject);
			if (hp <= 0) {
				if (player.weapon != null) {
					Destroy (player.weapon);
				}
				Destroy (gameObject);
			}			
		}
	}
}
