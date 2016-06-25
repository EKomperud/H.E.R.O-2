using UnityEngine;
using System.Collections;

public class EnemyWeaponScript : MonoBehaviour {

	public Transform shotPrefab;

	public float shootingRate = 0.25f;
	public int damage = 1;



	private float shootCooldown;

	void Start() {
		shootCooldown = 0f;
		//hasShot = false;
	}

	void Update() {
		if (shootCooldown > 0) {
			shootCooldown -= Time.deltaTime;
		}
	}

	public void Attack(bool isEnemy) {
		if (CanAttack) {
			shootCooldown = shootingRate;

			var shotTransform = Instantiate (shotPrefab) as Transform;

			shotTransform.position = transform.position;

			//ShurikenScript shot = shotTransform.gameObject.GetComponent<ShurikenScript> ();
		}
	}

	public bool CanAttack {
		get {
			return shootCooldown <= 0f;
		}
	}
}
