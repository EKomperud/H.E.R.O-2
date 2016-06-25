 using UnityEngine;
using System.Collections;

public class EnemyScript : MonoBehaviour {

	private EnemyWeaponScript[] weapons;
	private EnemyHealthScript health;
	bool goingUp = true;

	// Use this for initialization
	void Awake () {
	
		weapons = GetComponents<EnemyWeaponScript> ();
		health = GetComponent<EnemyHealthScript> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (health.hp <= 30) {
			if (weapons[0] != null) {
				weapons[0].Attack (true);
			}
		}
		if (health.hp <= 20) {
			foreach (EnemyWeaponScript weapon in weapons) {
				if (weapon != null) {
					weapon.Attack (true);
				}
			}
		}
		if (health.hp <= 10) {
			if (transform.position.y > 4.75) {
				goingUp = false;
			} else if (transform.position.y < -2) {
				goingUp = true;
			}
			if (goingUp) {
				Vector3 movement = new Vector3 (0f, 3f, 0f);
				movement *= Time.deltaTime;
				transform.Translate (movement);
			} else {
				Vector3 movement = new Vector3 (0f, -3f, 0f);
				movement *= Time.deltaTime;
				transform.Translate (movement);
			}
		}
	}
}
