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
        gracePeriod = 0.25f;
    }

    protected override void Update()
    {
        base.Update();
        if (!held && gracePeriod > 0)
        {
            gracePeriod -= Time.deltaTime;
        }
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (gracePeriod <= 0)
        {
            NPlayerController np = collider.gameObject.GetComponent<NPlayerController>();
            NWeapon w = collider.gameObject.GetComponent<NWeapon>();
            if (np != null)
            {
                np.HitByPlasma(transform.position);
                IEnumerator explosion = Explosion(1f, 0.25f);
                StartCoroutine(explosion);
            }
            else if (collider.gameObject.tag.Equals("Environment") && !collided)
            {
                IEnumerator explosion = Explosion(1f, 0.25f);
                StartCoroutine(explosion);
            }
            else if (w != null)
            {
                NWeaponPlasma wp = collider.gameObject.GetComponent<NWeaponPlasma>();
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
