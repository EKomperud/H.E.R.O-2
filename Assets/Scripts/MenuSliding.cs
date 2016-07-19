using UnityEngine;
using System.Collections;

public class MenuSliding : MonoBehaviour {

	public Vector3 endPosition = Vector3.zero;
	public float speed = 3f;

	private float timer = 0f;
	private Vector3 startPosition = Vector3.zero;
	private GameManager Menu;


	// Use this for initialization
	void Start () {
		Menu = GameObject.Find("GameManager").GetComponent<GameManager> ();
		startPosition = this.gameObject.transform.position;
		endPosition = endPosition + startPosition;

		float distance = Vector3.Distance (startPosition, endPosition);
		if (distance != 0) {
			speed = speed / distance;
		}
	}

	// Update is called once per frame
	void Update () {
		if (Menu.playBack) {
			if (Menu.setTimer) {
				timer = 0;
				Menu.setTimer = false;
			}
			timer += Time.deltaTime * speed;
			this.transform.position = Vector3.Lerp (endPosition, startPosition, timer);
			if (this.transform.position == startPosition) {
				Menu.doorDown = true;
				Menu.playBack = false;
				Menu.MainMenuSlide = true;
				Menu.doorUp = false;
				Menu.setTimer = true;
			} 
			else {
				Menu.doorDown = false;
			}

		} 
		else if (Menu.MainMenuSlide) {
			if (Menu.setTimer) {
				timer = 0;
				Menu.setTimer = false;
			}
			timer += Time.deltaTime * speed;
			this.transform.position = Vector3.Lerp (startPosition, endPosition, timer);
			if (this.transform.position == endPosition) {
				Menu.doorUp = true;
				Menu.doorDown = false;
			} 
			else {
				Menu.doorUp = false;
			}
		}
	}

	void OnDrawGizmos () {
		Gizmos.color = Color.red;
		Gizmos.DrawLine (this.transform.position, endPosition + this.transform.position);
	}

	void doorSlider () {
		
	}
}
