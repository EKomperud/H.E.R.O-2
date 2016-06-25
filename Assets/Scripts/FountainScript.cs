using UnityEngine;
using System.Collections;

public class FountainScript : MonoBehaviour {

	public Transform shotPrefab;

	public ArrayList Players = new ArrayList();

	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter2D (Collider2D collider) {
		PlayerController player = collider.gameObject.GetComponent<PlayerController> ();
		if (player != null) {
			player.fountain = this;
			player.inFountain = true;
			Players.Add (player);
		}
	}

	void OnTriggerExit2D (Collider2D collider) {
		PlayerController player = collider.gameObject.GetComponent<PlayerController> ();
		if (player != null) {
            if (player.fountain.Equals(this))
            {
                player.fountain = null;
                player.inFountain = false;
            }
			Players.Remove (player);
		}
	}

	public void CreateShot (PlayerController player) {
		var shot = Instantiate (shotPrefab) as Transform;
		shot.position = transform.position;

		PlayerWeaponScript shotScript = shot.gameObject.GetComponent<PlayerWeaponScript> ();
		player.weapon = shotScript;
		shotScript.caster = player;
		shotScript.MoveToCaster ();
	}
		
}
