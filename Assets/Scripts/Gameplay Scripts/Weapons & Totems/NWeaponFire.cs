using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NWeaponFire : NWeapon {

    [Space]
    [Header("Fire members")]
    [SerializeField] private Transform fireBoost;
    [SerializeField] ParticleSystem fireParticles;
    [SerializeField] ParticleSystem boostParticles;
    [SerializeField] private float boostTime;
    Dictionary<GameObject, NPlayerController> collidedWith;
    IEnumerator lifeTimer;
    Vector2 boostPosition;
    Vector2 lastFrameVelocity;
    bool collided;

    protected override void Start()
    {
        base.Start();
        collided = false;
        fireParticles = transform.GetChild(0).GetComponent<ParticleSystem>();
        collidedWith = new Dictionary<GameObject, NPlayerController>();
    }

    protected override void FixedUpdate()
    {
        if (!mobility)
        {
            base.FixedUpdate();
            lastFrameVelocity = rb.velocity;
        }
        else if (!collided)
        {
            Vector2 oldPos = rb.position;
            Vector2 newPos = wielder.GetNextFramePosition() + boostPosition;
            rb.MovePosition(newPos);
            lastFrameVelocity = newPos - oldPos;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        NPlayerController np = collision.collider.gameObject.GetComponent<NPlayerController>();
        NWeapon w = collision.collider.gameObject.GetComponent<NWeapon>();
        if (np != null && !collidedWith.ContainsKey(np.gameObject))
        {
            np.HitByFire(-Vector2.Reflect(lastFrameVelocity, collision.contacts[0].normal));
            collidedWith.Add(np.gameObject, np);
            if (mobility && !collided)
            {
                wielder.FireMobilityCollision(Vector2.Reflect(lastFrameVelocity, collision.contacts[0].normal));
                transform.SetParent(null);
                StopCoroutine(lifeTimer);
                fireBoost.gameObject.SetActive(false);
            }   
            IEnumerator explosion = Explosion(Vector2.Reflect(rb.velocity,collision.contacts[0].normal));
            StartCoroutine(explosion);
        }
        else if (collision.collider.gameObject.layer.Equals(LayerMask.NameToLayer("Platforms")))
        {
            if (mobility && !collided)
            {
                wielder.FireMobilityCollision(Vector2.Reflect(lastFrameVelocity, collision.contacts[0].normal));
                transform.SetParent(null);
                StopCoroutine(lifeTimer);
                fireBoost.gameObject.SetActive(false);
            }
            IEnumerator explosion = Explosion(Vector2.Reflect(lastFrameVelocity, collision.contacts[0].normal));
            StartCoroutine(explosion);
        }
        else if (w != null)
        {
            if (!activeShield)
                w.HitByFire(collision);
            else
                w.HitByFireShield(collision);
        }
    }

    public override void Mobility()
    {
        base.Mobility();
        float suspensionTime = wielder.FireMobility();
        if (leftStick != Vector2.zero)
            boostPosition = leftStick.normalized;
        else
            boostPosition = wielder.GetFlipX() ? new Vector2(-1f, 0f) : new Vector2(1f, 0f);
        IEnumerator mobilityCoroutine = delayedMobility(suspensionTime);
        StartCoroutine(mobilityCoroutine);
    }

    private IEnumerator delayedMobility(float suspensionTime)
    {
        yield return new WaitForSeconds(suspensionTime);
        animator.SetBool("boosted", true);
        mobility = true;
        boostPosition = leftStick != Vector2.zero ? leftStick.normalized : boostPosition;
        rb.MovePosition(wielder.GetNextFramePosition() + boostPosition);
        float z = (Mathf.Atan2(boostPosition.y, boostPosition.x) * 57.2958f);
        ParticleSystem.ShapeModule shape = boostParticles.shape;
        shape.rotation = new Vector3(z, shape.rotation.y, 0f);
        boostParticles.Play();
        boostParticles.transform.SetParent(wielder.transform);
        transform.rotation = Quaternion.Euler(0, 0, z);
        fireBoost.gameObject.SetActive(true);
        Collider2D playerCollider = wielder.GetCollider();
        Physics2D.IgnoreCollision(fireBoost.GetComponent<Collider2D>(), playerCollider, true);
        Physics2D.IgnoreCollision(fireBoost.GetChild(0).GetComponent<Collider2D>(), playerCollider, true);
        Physics2D.IgnoreCollision(cc, playerCollider, true);
        rb.simulated = true;
        cc.enabled = true;
        lifeTimer = LifetimeTimer(boostTime);
        StartCoroutine(lifeTimer);
    }

    public override void Shield()
    {
        base.Shield();
        fireParticles.Play();
        fireParticles.transform.SetParent(null);
        ParticleSystem.MainModule main = fireParticles.main;
        main.stopAction = ParticleSystemStopAction.Destroy;
    }

    private IEnumerator Explosion(Vector2 newAngle)
    {
        collided = true;
        fireParticles.Play();
        fireParticles.transform.SetParent(null);
        ParticleSystem.MainModule main = fireParticles.main;
        main.stopAction = ParticleSystemStopAction.Destroy;
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

    public override void HitByFire(Collision2D collision)
    {
        base.HitByFire(collision);
        if (!activeShield)
        {
            IEnumerator explosion = Explosion(Vector2.Reflect(lastFrameVelocity, collision.contacts[0].normal));
            StartCoroutine(explosion);
        }
    }

    public override void HitByWater(Collision2D collision)
    {
        base.HitByWater(collision);
        if (!activeShield)
            Destroy(gameObject);
        else
            StartCoroutine("ShieldDissipate");
    }

    public override void HitByWaterShield(Collision2D collision)
    {
        base.HitByWaterShield(collision);
        if (!activeShield)
            Destroy(gameObject);
        else
            StartCoroutine("ShieldDissipate");
    }

    public override void HitByEarth()
    {
        base.HitByEarth();
        rb.velocity = -rb.velocity;
        // FIX THIS
        StartCoroutine("Explosion");
    }

    public override void HitByFireShield(Collision2D collision)
    {
        base.HitByFireShield(collision);
        if (!activeShield)
        {
            Physics2D.IgnoreCollision(collision.collider, wielder.GetCollider(), true);
            IEnumerator explosion = Explosion(Vector2.Reflect(lastFrameVelocity, collision.contacts[0].normal));
            StartCoroutine(explosion);
        }
    }
}
