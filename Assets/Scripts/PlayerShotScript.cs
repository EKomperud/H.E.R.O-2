using UnityEngine;
using System.Collections;

public class PlayerShotScript : MonoBehaviour {

	public PlayerController caster;
	private bool atCaster = false;

	public int damage = 1;

	public Vector2 speed = new Vector2 (5, 5);
	public Vector2 direction = new Vector2(-1, 0);

	// Use this for initialization
	void Start () {

		//Destroy (gameObject, 3);
	}

	void Update() {
		float X = caster.transform.position.x - this.transform.position.x;
		float Y = caster.transform.position.y - this.transform.position.y;
		if ((X > 1 || X < -1) || (Y > 1 || Y < -1)) {
			Vector3 movement = new Vector3 (speed.x * X, speed.y * Y, 0);
			movement *= Time.deltaTime;
			this.transform.Translate (movement);
		} 
	}

	public void MoveToCaster () {
		float X = caster.transform.position.x - this.transform.position.x;
		float Y = caster.transform.position.y - this.transform.position.y;
		Vector3 movement = new Vector3 (speed.x * X, speed.y * Y, 0);
		movement *= Time.deltaTime;
		this.transform.Translate (movement);
	}
}
