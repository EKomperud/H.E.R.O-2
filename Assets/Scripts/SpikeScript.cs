using UnityEngine;
using System.Collections;

public class SpikeScript : MonoBehaviour {

	// Use this for initialization
	void OnTriggerEnter2D (Collider2D collider) {
		Health2Script Player2 = collider.gameObject.GetComponent<Health2Script> ();
		HealthScript Player1 = collider.gameObject.GetComponent<HealthScript> ();
		if (Player1 != null) {
			Destroy (Player1.gameObject);
		}
		if (Player2 != null) {
			Destroy	(Player2.gameObject);
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
