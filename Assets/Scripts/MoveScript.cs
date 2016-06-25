using UnityEngine;
using System.Collections;

public class MoveScript : MonoBehaviour {

	public Vector2 speed = new Vector2 (15, 15);
	public Vector2 direction = new Vector2(-1, 0);
	public bool fire = false;

	// Update is called once per frame
	void Update () {
		if (fire) {
			Vector3 movement = new Vector3 (speed.x * direction.x, speed.y * direction.y, 0);
			movement *= Time.deltaTime;
			transform.Translate (movement);
		}
	}
}
