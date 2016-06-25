using UnityEngine;
using System.Collections;

public class Health2Script : MonoBehaviour {

	public int hp = 5;
	private Player2Controller player; 

	void Awake () {
		player = gameObject.GetComponent<Player2Controller> ();
	}

	void OnTriggerEnter2D (Collider2D collider) {
		//player = gameObject.GetComponent<PlayerScript>();
		PlayerWeaponScript shot = collider.gameObject.GetComponent<PlayerWeaponScript> ();
		if (shot != null) {			
			hp -= shot.damage;
			System.Threading.Thread d = new System.Threading.Thread (() => shot.Destroy ());
			d.Start ();
            //Destroy(shot.gameObject);
			if (hp <= 0) {
				if (player.weapon != null) {
					Destroy (player.weapon);
				}
				Destroy (gameObject);
			}			
		}
	}
}
