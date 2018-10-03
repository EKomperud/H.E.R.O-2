using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationTest : MonoBehaviour {

    Rigidbody2D rb;
    Animator animator;
    float speedX;

	void Start () {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
	}
	
	void Update () {
        speedX = Input.GetKey(KeyCode.R) ? 1f : 0f;
        animator.SetFloat("speedX", speedX);
	}
}
