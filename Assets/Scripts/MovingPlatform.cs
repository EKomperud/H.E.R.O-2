using UnityEngine;
using System.Collections;

public class MovingPlatform : MonoBehaviour {

	public Vector3 endPosition = Vector3.zero;
	public float speed = 1f;
    public Transform playersObject;

	private float timer = 0f;
    private Vector2 frameDist;
    private float _travelFrames;
    private float travelFrames;
	private Vector3 startPosition = Vector3.zero;
	private bool outgoing = true;
    private Vector2 frameMovement;
    private Rigidbody2D rb;

	void Start () {
		startPosition = this.gameObject.transform.position;
		endPosition = endPosition + startPosition;
        rb = GetComponent<Rigidbody2D>();

		float distance = Vector3.Distance (startPosition, endPosition);
		if (distance != 0) {
			speed = speed / distance;
		}
        frameDist = (endPosition - startPosition) * (Time.fixedDeltaTime * speed);
        _travelFrames = (endPosition - startPosition).magnitude / frameDist.magnitude;
        travelFrames = 0;
        frameMovement = frameDist;
    }

    void FixedUpdate () {

        if (outgoing) {
            rb.MovePosition(rb.position + frameDist);
            if (++travelFrames >= _travelFrames)
            {
                outgoing = false;
                travelFrames = 0;
                frameMovement = -frameDist;
            }
		} 
		else {
            rb.MovePosition(rb.position - frameDist);
            if (++travelFrames >= _travelFrames) {
				outgoing = true;
                travelFrames = 0;
                frameMovement = frameDist;
            }
        }
	}

    public Vector2 GetLastFrameMovement()
    {
        return frameMovement;
    }

    void OnDrawGizmos () {
		Gizmos.color = Color.red;
		Gizmos.DrawLine (this.transform.position, endPosition + this.transform.position);
	}
}
