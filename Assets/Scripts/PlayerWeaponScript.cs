using UnityEngine;
using System.Collections;

public class PlayerWeaponScript : MonoBehaviour {

    private AnimationController2D animator;
    public RockScript rockScript;

	// ShotScript code
	public PlayerController caster;
	private bool atCaster = false;
    private bool right = true;
	public string shotType = "default";

	public int damage = 1;
	private float sizeX;

	public Vector2 speed = new Vector2 (0f, 0f);
	public Vector2 direction = new Vector2(1, 0);

	public bool hasShot { get; set; }
	public bool fire = false;
	private bool connected = false;
	public bool rock = false;

	void Start() {
        rockScript = gameObject.GetComponent<RockScript>();
        animator = gameObject.GetComponent<AnimationController2D>();
		sizeX = transform.localScale.x;
		hasShot = false;
	}

	void Update() {
		// Regular weapons
		if (!rock) {
			// Weapon is following player
			if (!fire) {
				if (caster.faceRight) {
					direction.x = 1;
				} else {
					direction.x = -1;
				}

                
				float X = caster.transform.position.x - this.transform.position.x;
				float Y = caster.transform.position.y - this.transform.position.y;
				if ((X > 2 || X < -2) || (Y > 0.1 || Y < -0.1)) {
                    atCaster = false;
					Vector3 movement = new Vector3 ((speed.x / 2) * X, (speed.y / 2) * Y, 0);
					movement *= Time.deltaTime;
					this.transform.Translate (movement);
				} else {
					hasShot = true;
                    atCaster = true;
				}
			}

			// Weapon has been fired
			if (fire && !connected) {

                if (shotType.Equals("fire"))
                    animator.setAnimation("Fireball Shoot");

                if (shotType.Equals("plasma"))
                    animator.setAnimation("plasmaShoot");

				hasShot = false;

				Vector3 movement = new Vector3 (speed.x * direction.x, 0, 0);
				if (shotType.Equals ("rock")) {
					movement = new Vector3 (speed.x * direction.x * 1.5f, 0, 0);
					Destroy (gameObject, 0.25f);
				}
				movement *= Time.deltaTime;
				transform.Translate (movement);

			}

			// Shot connected
			if (connected) {
                if (shotType.Equals("fire"))
                {
                    animator.setAnimation("Fireball Explosion");
                    if (transform.localScale.x < (4 * sizeX))
                    {
                        transform.localScale = new Vector3(transform.localScale.x * 1.05f, transform.localScale.y * 1.05f, 1);
                    }
                    else
                        Destroy(gameObject);
                }
                else if (shotType.Equals("plasma"))
                {
                    animator.setAnimation("plasmaExplosion");
                    if (transform.localScale.x < (4 * sizeX))
                    {
                        transform.localScale = new Vector3(transform.localScale.x * 1.05f, transform.localScale.y * 1.05f, 1);
                    }
                }
			}
		} 

		// Rock
		else {
			if (caster != null) {
				if (!fire) {
					if (caster.faceRight) {
						direction.x = 1;
					} else {
						direction.x = -1;
					}


					float X = caster.transform.position.x - this.transform.position.x;
					float Y = caster.transform.position.y - this.transform.position.y;
					if ((X > 1 || X < -1) || (Y > 0.3 || Y < -0.3)) {
						Vector3 movement = new Vector3 ((speed.x / 2) * X, (speed.y / 2) * Y, 0);
						movement *= Time.deltaTime;
						this.transform.Translate (movement);
					} else {
						hasShot = true;
					}
				}

				// Weapon has been fired
				if (fire) {

					hasShot = false;

					Vector3 movement = new Vector3 (speed.x * direction.x, 0, 0);
					if (shotType.Equals ("rock")) {
						movement = new Vector3 (speed.x * direction.x * 1.5f, 0, 0);
						Destroy (gameObject, 0.25f);
					}
					movement *= Time.deltaTime;
					transform.Translate (movement);

				}

				// Shot connected
				if (connected) {
					if (shotType.Equals ("fire")) {
						if (transform.localScale.x < (4 * sizeX)) { 
							transform.localScale = new Vector3 (transform.localScale.x * 1.05f, transform.localScale.y * 1.05f, 1);
						} else
							Destroy (gameObject);
					}
				}
			}
		}
	}

	void OnTriggerEnter2D (Collider2D collider) {
		Player2WeaponScript projectile = collider.gameObject.GetComponent<Player2WeaponScript> ();
		if (projectile != null && shotType.Equals ("rock")) {
			Destroy (collider.gameObject);
		}
	}

	public void Attack() {
		if (hasShot) {
			fire = true;
		}
	}

	public bool CanAttack {
		get {
			return hasShot;
		}
	}

	public void MoveToCaster () {
		float X = caster.transform.position.x - this.transform.position.x;
		float Y = caster.transform.position.y - this.transform.position.y;
		Vector3 movement = new Vector3 (speed.x * X, speed.y * Y, 0);
		movement *= Time.deltaTime;
		this.transform.Translate (movement);
	}

	public void Flip() {
		Vector3 weaponScale = transform.localScale;
		weaponScale.x *= -1;
		transform.localScale = weaponScale;
	}

	public void Destroy() {
		speed = new Vector2 (2, 2);	
		connected = true;
	}
	
	public void Burn() {
		//wtf;	
	}
}
