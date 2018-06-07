using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NWeaponAir : NWeapon {

    bool collided;

    protected override void Start()
    {
        base.Start();
        collided = false;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        NPlayerController np = collision.collider.gameObject.GetComponent<NPlayerController>();
        NWeapon w = collision.collider.gameObject.GetComponent<NWeapon>();
        if (np != null && np != wielder)
        {
            if (!np.GetMovementBool("pushed"))
                np.HitByAir(-collision.contacts[0].normal);
            if (!collided)
            {
                IEnumerator coroutine = Explosion(false);
                StartCoroutine(coroutine);
            }
        }
        if (w != null)
        {
            w.HitByAir();
            if (!collided)
            {
                IEnumerator coroutine = Explosion(false);
                StartCoroutine(coroutine);
            }
        }
        else if (collision.collider.gameObject.layer.Equals(LayerMask.NameToLayer("Platforms")))
        {
            if (!collided)
            {
                IEnumerator coroutine = Explosion(true);
                StartCoroutine(coroutine);
            }
        }
    }

    public override void HitByAir()
    {
        base.HitByAir();
    }

    public override void HitByEarth()
    {
        base.HitByEarth();
    }

    public override void HitByWater()
    {
        base.HitByWater();
    }

    public override void HitByFire()
    {
        base.HitByFire();
    }

    private IEnumerator Explosion(bool reverseDirection)
    {
        collided = true;
        animator.SetBool("collided", true);
        if (reverseDirection)
        {
            rb.velocity = rb.velocity * -0.075f;
            GetComponent<SpriteRenderer>().flipX = true;
            GetComponent<SpriteRenderer>().flipY = true;
        }
        else
        {
            rb.velocity *= 0.075f;
        }
        float timer = 0f;
        while (timer <= 0.5f)
        {
            transform.localScale += new Vector3(0.00125f, 0.00125f, 0.00125f);
            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        Destroy(gameObject);
    }
}
