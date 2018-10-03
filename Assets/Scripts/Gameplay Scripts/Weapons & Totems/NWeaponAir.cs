using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NWeaponAir : NWeapon {

    [Space]
    [Header("Air Members")]
    [SerializeField] private ParticleSystem airParticles;
    [SerializeField] private Transform airShield;
    [SerializeField] private float shieldTime;
    bool collided;

    protected override void Start()
    {
        base.Start();
        collided = false;
    }

    protected override void FixedUpdate()
    {
        if (!mobility)
            base.FixedUpdate();
        else
            rb.MovePosition(wielder.GetNextFramePosition());
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        NPlayerController np = collision.collider.gameObject.GetComponent<NPlayerController>();
        NWeapon w = collision.collider.gameObject.GetComponent<NWeapon>();
        if (np != null)
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
            w.HitByAir(-collision.contacts[0].normal, wielder);
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

    public override void Mobility()
    {
        sr.enabled = false;
        mobility = true;
        transform.localPosition = new Vector3(0f, 0f, 0f);
        airShield.gameObject.SetActive(true);
        gameObject.layer = 9;
        airShield.gameObject.layer = 9;
        Collider2D playerCollider = wielder.GetCollider();
        Physics2D.IgnoreCollision(airShield.GetComponent<Collider2D>(), playerCollider, true);
        rb.simulated = true;
        airParticles.Play();
        airParticles.transform.SetParent(wielder.transform);
        base.Mobility();
        IEnumerator lifeTimer = LifetimeTimer(shieldTime);
        StartCoroutine(lifeTimer);
        wielder.AirMobility();
    }
}
