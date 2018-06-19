using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NWeaponFire : NWeapon {

    Vector2 lastFrameVelocity;

    protected override void Start()
    {
        base.Start();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        lastFrameVelocity = rb.velocity;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        NPlayerController np = collision.collider.gameObject.GetComponent<NPlayerController>();
        NWeapon w = collision.collider.gameObject.GetComponent<NWeapon>();
        if (np != null)
        {
            np.HitByFire();
            IEnumerator explosion = Explosion(Vector2.Reflect(rb.velocity,collision.contacts[0].normal));
            StartCoroutine(explosion);
        }
        else if (collision.collider.gameObject.layer.Equals(LayerMask.NameToLayer("Platforms")))
        {
            IEnumerator explosion = Explosion(Vector2.Reflect(lastFrameVelocity, collision.contacts[0].normal));
            StartCoroutine(explosion);
        }
        else if (w != null)
        {
            w.HitByFire();
        }
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        NPlayerController np = collider.gameObject.GetComponent<NPlayerController>();
        NWeapon w = collider.gameObject.GetComponent<NWeapon>();
        if (np != null && np != wielder)
            np.HitByFire();
        else if (w != null)
            w.HitByFire();
    }

    private IEnumerator Explosion(Vector2 newAngle)
    {
        animator.SetBool("collided", true);
        float rotationAngle = Vector2.SignedAngle(lastFrameVelocity, newAngle);
        rb.velocity = newAngle * 0.05f;
        transform.Rotate(0f, 0f, rotationAngle);
        gameObject.GetComponent<Collider2D>().isTrigger = true;
        float timer = 0f;
        while (timer <= 0.5f)
        {
            transform.localScale += new Vector3(0.025f,0.025f,0.025f);
            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        Destroy(gameObject);
    }

    public override void HitByAir(Vector2 normal, NPlayerController wielder)
    {
        base.HitByAir(normal, wielder);
        Physics2D.IgnoreCollision(cc, this.wielder.GetCollider(), false);
        Physics2D.IgnoreCollision(cc, wielder.GetCollider(), true);
        this.wielder = wielder;
        rb.velocity = normal * speed;
    }

    public override void HitByWater()
    {
        base.HitByWater();
        Destroy(gameObject);
    }

    public override void HitByEarth()
    {
        base.HitByEarth();
        rb.velocity = -rb.velocity;
        StartCoroutine("Explosion");
    }
}
