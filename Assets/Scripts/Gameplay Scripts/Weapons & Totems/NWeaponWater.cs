using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NWeaponWater : NWeapon
{
    ParticleSystem waterParticles;
    Collider2D weaponCollider;
    Collider2D shieldCollider;

    protected override void Start()
    {
        base.Start();
        waterParticles = transform.GetChild(0).GetComponent<ParticleSystem>();
        weaponCollider = GetComponent<Collider2D>();
        shieldCollider = transform.GetChild(1).GetComponent<Collider2D>();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        NPlayerController np = collision.collider.gameObject.GetComponent<NPlayerController>();
        NWeapon w = collision.collider.gameObject.GetComponent<NWeapon>();
        if (np != null)
        {
            np.HitByWater();
            StartCoroutine("Explosion");
        }
        else if (collision.collider.gameObject.layer.Equals(LayerMask.NameToLayer("Platforms")))
        {
            StartCoroutine("Explosion");
        }
        else if (w != null)
        {
            if (!activeShield)
                w.HitByWater(collision);
            else
                w.HitByWaterShield(collision);
        }
    }

    public override void Shield()
    {
        cc = shieldCollider;
        shieldCollider.gameObject.layer = LayerMask.NameToLayer("Shield");
        base.Shield();
        waterParticles.Play();
        waterParticles.transform.SetParent(null);
        ParticleSystem.MainModule main = waterParticles.main;
        main.stopAction = ParticleSystemStopAction.Destroy;
    }

    private IEnumerator Explosion()
    {
        waterParticles.Play();
        waterParticles.transform.SetParent(null);
        ParticleSystem.MainModule main = waterParticles.main;
        main.stopAction = ParticleSystemStopAction.Destroy;
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

    public override void HitByAir(Vector2 normal, NPlayerController wielder)
    {
        base.HitByAir(normal, wielder);
        Physics2D.IgnoreCollision(cc, this.wielder.GetCollider(), false);
        Physics2D.IgnoreCollision(cc, wielder.GetCollider(), true);
        this.wielder = wielder;
        rb.velocity = normal * speed;
    }

    public override void HitByFire(Collision2D collision)
    {
        base.HitByFire(collision);
        if (!activeShield)
            Destroy(gameObject);
        else
            StartCoroutine("ShieldDissipate");
    }

    public override void HitByFireShield(Collision2D collision)
    {
        base.HitByFireShield(collision);
        if (!activeShield)
            Destroy(gameObject);
        else
            StartCoroutine("ShieldDissipate");
    }

    public override void HitByEarth()
    {
        base.HitByEarth();
        rb.velocity = -rb.velocity;
        StartCoroutine("Explosion");
    }
}
