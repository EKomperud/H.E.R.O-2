using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NWeaponPlasma : NWeapon
{
    private float gracePeriod;
    private bool collided;

    protected override void Start()
    {
        base.Start();
        lifetime = Mathf.Infinity;
        gracePeriod = 0.1f;
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        if (!held && gracePeriod > 0)
        {
            gracePeriod -= Time.fixedDeltaTime;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (gracePeriod <= 0)
        {
            NPlayerController np = collision.collider.gameObject.GetComponent<NPlayerController>();
            NWeapon w = collision.collider.gameObject.GetComponent<NWeapon>();
            if (np != null)
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
    }

    private IEnumerator Explosion(float time, float scale)
    {
        collided = true;
        animator.SetBool("collided", true);
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.velocity = Vector2.zero;
        transform.SetParent(null);
        transform.position = new Vector3(transform.position.x, transform.position.y, 0f);
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
