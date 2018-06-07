using UnityEngine;
using System.Collections;

public class BounceScript : MonoBehaviour {

    private Animator animator;

    void Start () {
        animator = gameObject.GetComponent<Animator>();
    }
	
    void OnTriggerEnter2D(Collider2D collider)
    {
        NPlayerController player = collider.gameObject.GetComponent<NPlayerController>();
        if (player != null)
        {
            animator.SetTrigger("activated");
            player.Bounce();
        }
    }
}
