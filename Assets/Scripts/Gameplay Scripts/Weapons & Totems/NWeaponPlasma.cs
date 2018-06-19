using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NWeaponPlasma : NWeapon
{
    private float gracePeriod;
    private bool collided;
    private CircleCollider2D circleCollider;
    private CircleCollider2D pullCollider;
    private Collider2D playerCollider;

    protected override void Start()
    {
        base.Start();
        circleCollider = (CircleCollider2D)cc;
        pullCollider = transform.GetChild(0).GetComponent<CircleCollider2D>();
        pullCollider.enabled = false;
        lifetime = Mathf.Infinity;
        gracePeriod = 0.1f;
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        if (!held && gracePeriod > 0)
        {
            gracePeriod -= Time.fixedDeltaTime;
            if (gracePeriod <= 0)
                Physics2D.IgnoreCollision(cc, playerCollider, false);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        NPlayerController np = collision.collider.gameObject.GetComponent<NPlayerController>();
        NWeapon w = collision.collider.gameObject.GetComponent<NWeapon>();
        if (np != null && gracePeriod <= 0)
        {
            np.HitByPlasma(transform.position);
            IEnumerator explosion = Explosion(1f, 0.25f);
            StartCoroutine(explosion);
        }
        else if (collision.collider.gameObject.layer.Equals(LayerMask.NameToLayer("Platforms")) && !collided)
        {
            IEnumerator explosion = Explosion(1f, 0.25f);
            StartCoroutine(explosion);
        }
        else if (w != null)
        {
            NWeaponPlasma wp = collision.collider.gameObject.GetComponent<NWeaponPlasma>();
            if (wp != null)
            {
                if (transform.localScale.x > wp.transform.localScale.x)
                {
                    wp.HitByPlasma(transform.position);
                    IEnumerator explosion = Explosion(2f, 0.5f);
                    StartCoroutine(explosion);
                }
            }
            else
            {
                w.HitByPlasma(transform.position);
                IEnumerator explosion = Explosion(0.25f, 0.1f);
                StartCoroutine(explosion);
            }
        }
    }

    public override void Discharge(Vector2 angle, Collider2D playerCollider, bool flipX)
    {
        base.Discharge(angle, playerCollider, flipX);
        this.playerCollider = playerCollider;
    }

    private IEnumerator Explosion(float time, float scale)
    {
        collided = true;
        animator.SetBool("collided", true);
        circleCollider.radius = 0.325f;
        pullCollider.enabled = true;
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.useFullKinematicContacts = true;
        rb.velocity = Vector2.zero;
        transform.SetParent(null);
        transform.position = new Vector3(transform.position.x, transform.position.y, 1f);
        float timer = 0f;
        while (timer <= 1f)
        {
            float fx = 1 / ((50 * timer) + 0.5f);
            fx *= scale;
            //fx *= 0.5f;
            transform.localScale += new Vector3(fx, fx, fx);
            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }
}
