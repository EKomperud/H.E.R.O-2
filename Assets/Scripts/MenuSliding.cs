using UnityEngine;
using System.Collections;

public class MenuSliding : MonoBehaviour {

	public Vector3 endPosition = Vector3.zero;
	public float speed = 1f;

	private float timer = 0f;
	private Vector3 startPosition = Vector3.zero;
	private bool outgoing = true;
	public GameManager Menu; 


	// Use this for initialization
	void Start () {
		startPosition = this.gameObject.transform.position;
		endPosition = endPosition + startPosition;

		float distance = Vector3.Distance (startPosition, endPosition);
		if (distance != 0) {
			speed = speed / distance;
		}
	}
	
	// Update is called once per frame
	void Update () {
		timer += Time.deltaTime * speed;

		if (outgoing) {
			this.transform.position = Vector3.Lerp (startPosition, endPosition, timer);
			if (Menu.MainMenuSlide) {
				outgoing = false;
				timer = 0;
			}
		} 
		else {
			this.transform.position = Vector3.Lerp (endPosition, startPosition, timer);
			if (Menu.MainMenuSlide && Menu.Back) {
				outgoing = true;
				timer = 0;
			}
		}
	}

	void OnDrawGizmos () {
		Gizmos.color = Color.red;
		Gizmos.DrawLine (this.transform.position, endPosition + this.transform.position);
	}
}
