using UnityEngine;
using System.Collections;

public class SpikeScript : MonoBehaviour {

	public NumberKeeper hasHit;

	// Use this for initialization
	void OnTriggerEnter2D (Collider2D collider) {
		//hasHit = GameObject.Find("NumberKeeper").GetComponent<NumberKeeper> ();
		//HealthScript Player = collider.gameObject.GetComponent<HealthScript> ();
		//if (Player != null) {
		//	if (Player.gameObject.tag == "Player") {
		//		hasHit.hasHit1 = true;
		//		Destroy (Player.gameObject);
		//	} 
		//	if (Player.gameObject.tag == "Player2") {
		//		hasHit.hasHit2 = true;
		//		Destroy (Player.gameObject);
		//	} 
		//	if (Player.gameObject.tag == "Player3") {
		//		hasHit.hasHit3 = true;
		//		Destroy (Player.gameObject);
		//	}
		//	if (Player.gameObject.tag == "Player4") {
		//		hasHit.hasHit4 = true;
		//		Destroy (Player.gameObject);
		//	}
		//}
	}
}
