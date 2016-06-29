﻿using UnityEngine;
using System.Collections;

public class SpikeScript : MonoBehaviour {

	public bool hasHit = false;

	// Use this for initialization
	void OnTriggerEnter2D (Collider2D collider) {
		Health2Script Player2 = collider.gameObject.GetComponent<Health2Script> ();
		HealthScript Player1 = collider.gameObject.GetComponent<HealthScript> ();
		if (Player1 != null) {
			hasHit = true;
			Destroy (Player1.gameObject);
		}
		if (Player2 != null) {
			hasHit = true;
			Destroy	(Player2.gameObject);
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
