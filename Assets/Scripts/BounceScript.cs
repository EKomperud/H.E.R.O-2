using UnityEngine;
using System.Collections;

public class BounceScript : MonoBehaviour {

    private AnimationController2D animator;
    private float animationTimer = 0.5f;

    // Use this for initialization
    void Start () {
        animator = gameObject.GetComponent<AnimationController2D>();
    }
	
	// Update is called once per frame
	void Update () {
        if (animationTimer >= 0)
            animationTimer -= Time.deltaTime;
        else
            animator.setAnimation("BounceBlock Idle");
	}

    void OnTriggerEnter2D(Collider2D collider)
    {
        PlayerController player = collider.gameObject.GetComponent<PlayerController>();
        if (player != null)
        {
            animator.setAnimation("BounceBlock anim");
            animationTimer = 0.20f;
        }
            
    }
}
