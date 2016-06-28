using UnityEngine;
using System.Collections;

public class MenuSliding : MonoBehaviour {

	public Vector3 endPosition = Vector3.zero;
	public float speed = 3f;
	public float buttonCountSecond = 0;
	public float buttonCountThird = 0;
	public float buttonCountFirstMax = 5;
	public float buttonCountSecondMax = 8;
	public float buttonCountThirdMax = 2;

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
			if (Menu.buttonCountFirst < 10 && this.gameObject.tag == "firstLayer" && Menu.setTimer) {
				timer = 0;
				Menu.buttonCountFirst += 1;
			}
			if (Menu.buttonCountFirst < 10 && this.gameObject.tag == "secondLayer" && Menu.setTimer) {
				timer = 0;
				Menu.buttonCountFirst += 1;
			}
			if (Menu.buttonCountFirst < 10 && this.gameObject.tag == "backLayer" && Menu.setTimer) {
				timer = 0;
				Menu.buttonCountFirst += 1;
			}
			if (Menu.buttonCountFirst >= 10 && Menu.setTimer) {
				Menu.buttonCountFirst = 0;
				Menu.setTimer = false;
			}
			timer += Time.deltaTime * speed;
			this.transform.position = Vector3.Lerp (endPosition, startPosition, timer);

		} 
		if (Menu.MainMenuSlide) {
			if (Menu.buttonCountFirst < 10 && this.gameObject.tag == "firstLayer" && Menu.setTimer) {
				timer = 0;
				Menu.buttonCountFirst += 1;
			}
			if (Menu.buttonCountFirst < 10 && this.gameObject.tag == "secondLayer" && Menu.setTimer) {
				timer = 0;
				Menu.buttonCountFirst += 1;
			}
			if (Menu.buttonCountFirst < 10 && this.gameObject.tag == "backLayer" && Menu.setTimer) {
				timer = 0;
				Menu.buttonCountFirst += 1;
			}
			if (Menu.buttonCountFirst >= 10 && Menu.setTimer) {
				Menu.buttonCountFirst = 0;
				Menu.setTimer = false;
			}
			timer += Time.deltaTime * speed;
			this.transform.position = Vector3.Lerp (startPosition, endPosition, timer);
		}
	}

	void OnDrawGizmos () {
		Gizmos.color = Color.red;
		Gizmos.DrawLine (this.transform.position, endPosition + this.transform.position);
	}
}
