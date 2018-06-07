using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NWeaponWater : NWeapon
{

    protected override void Start()
    {
        base.Start();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        NPlayerController np = collision.collider.gameObject.GetComponent<NPlayerController>();
        NWeapon w = collision.collider.gameObject.GetComponent<NWeapon>();
        if (np != null && np != wielder)
        {
            np.HitByWater();
            StartCoroutine("Explosion");
        }
        else if (collision.collider.gameObject.tag.Equals("Environment"))
        {
            StartCoroutine("Explosion");
        }
        else if (w != null)
        {
            w.HitByWater();
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

    public override void HitByAir()
    {
        base.HitByAir();
        rb.velocity = -rb.velocity;
    }

    public override void HitByFire()
    {
        base.HitByFire();
        Destroy(gameObject);
    }

    public override void HitByEarth()
    {
        base.HitByEarth();
        rb.velocity = -rb.velocity;
        StartCoroutine("Explosion");
    }
}
