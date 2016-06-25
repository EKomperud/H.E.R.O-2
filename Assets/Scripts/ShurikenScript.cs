using UnityEngine;
using System.Collections;

public class ShurikenScript : MonoBehaviour {

	public int damage = 1;

	public Vector2 speed = new Vector2 (30, 30);
	public Vector2 direction = new Vector2(1, 0);

	// Use this for initialization
	void Start () {
	
		Destroy (gameObject, 3);
	}

	void Update() {
		Vector3 movement = new Vector3 (speed.x * direction.x, speed.y * direction.y, 0);
		movement *= Time.deltaTime;
		transform.Translate (movement);
	}
}
