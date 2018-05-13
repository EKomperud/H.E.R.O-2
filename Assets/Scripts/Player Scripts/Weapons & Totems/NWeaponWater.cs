using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NWeaponWater : NWeapon
{

    protected override void Start()
    {
        base.Start();
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        NPlayerController np = collider.gameObject.GetComponent<NPlayerController>();
        if (np != null && np != wielder)
        {
            np.HitByWater();
            StartCoroutine("Explosion");
        }
        else if (collider.gameObject.tag.Equals("Environment"))
        {
            StartCoroutine("Explosion");
        }
    }

    private IEnumerator Explosion()
    {
        animator.SetBool("collided", true);
        rb.velocity *= 0.1f;
        float timer = 0f;
        while (timer <= 0.417f)
        {
            transform.localScale += new Vector3(0.015f, 0.015f, 0.015f);
            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        Destroy(gameObject);
    }
}
