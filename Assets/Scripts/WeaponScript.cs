using UnityEngine;
using System.Collections;

public class WeaponScript : MonoBehaviour {

	public Transform shotPrefab;

	public float shootingRate = 0.25f;

	private float shootCooldown;

	// ShotScript code
//	public PlayerScript caster;
//	private bool atCaster = false;
//
//	public int damage = 1;
//
//	public Vector2 speed = new Vector2 (5, 5);
//	public Vector2 direction = new Vector2(-1, 0);
//
//	public bool hasShot { get; set; }

	void Start() {
		shootCooldown = 0f;
		//hasShot = false;
	}

	void Update() {
		if (shootCooldown > 0) {
			shootCooldown -= Time.deltaTime;
		}
//		float X = caster.transform.position.x - this.transform.position.x;
//		float Y = caster.transform.position.y - this.transform.position.y;
//		if ((X > 1 || X < -1) || (Y > 1 || Y < -1)) {
//			Vector3 movement = new Vector3 (speed.x * X, speed.y * Y, 0);
//			movement *= Time.deltaTime;
//			this.transform.Translate (movement);
//		} else
//			hasShot = true;
	}

	public void Attack(bool isEnemy) {
		if (CanAttack) {
			shootCooldown = shootingRate;

			var shotTransform = Instantiate (shotPrefab) as Transform;

			shotTransform.position = transform.position;

			ShurikenScript shot = shotTransform.gameObject.GetComponent<ShurikenScript> ();

			MoveScript move = shotTransform.gameObject.GetComponent<MoveScript> ();

			if (move != null) {
				if (transform.localScale.x > 0) {
					move.direction.x = -1;
				} else if (transform.localScale.x < 0) {
					move.direction.x = 1;
				}
			}

		}
	}

	public bool CanAttack {
		get {
			//if (shootCooldown <= 0f)
			return shootCooldown <= 0f;
			//return false;
		}
	}

//	public void MoveToCaster () {
//		float X = caster.transform.position.x - this.transform.position.x;
//		float Y = caster.transform.position.y - this.transform.position.y;
//		Vector3 movement = new Vector3 (speed.x * X, speed.y * Y, 0);
//		movement *= Time.deltaTime;
//		this.transform.Translate (movement);
//	}
}
