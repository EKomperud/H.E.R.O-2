﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class NWeapon : MonoBehaviour {

    protected NPlayerController wielder;
    protected Rigidbody2D rb;
    protected Collider2D cc;
    protected Animator animator;
    protected SpriteRenderer sr;
    protected Vector3 orbit;
    protected Vector2 rightStick;
    protected Vector2 dischargeAngle;
    protected bool dischargeButton;
    protected Player joystick;
    protected float x, y, z, angle, rotationSpeed, bobSpeed, radius, xRotation, yRotation, height;
    protected bool held;
    protected float lifetime, timer;

    [SerializeField] protected float speed;
    [SerializeField] protected EElement element;

	protected virtual void Start () {
        x = y = z = 0;
        rotationSpeed = 4f;
        bobSpeed = rotationSpeed * 2;
        Vector3 pct = wielder.transform.position;
        transform.position = new Vector3(pct.x + 0.5f, pct.y + 0.5f, pct.z);
        orbit = transform.position;
        held = true;
        rb = GetComponent<Rigidbody2D>();
        rb.simulated = false;
        cc = GetComponent<Collider2D>();
        cc.enabled = false;
        animator = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        lifetime = 3f;
        timer = 0f;
    }
	
	protected virtual void FixedUpdate () {
        if (held)
        {
            rightStick = new Vector2(joystick.GetAxis("Aim Horizontal"), joystick.GetAxis("Aim Vertical"));
            if (rightStick != Vector2.zero && this.Equals(wielder.GetWeapon()))
            {
                UpdateAiming(rightStick);
            }
            else
            {
                UpdateNotAiming();
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collider)
    {
        NWeaponPlasma plasma = collider.transform.GetComponentInParent<NWeaponPlasma>();
        if (plasma != null)
        {
            if (element != EElement.plasma || transform.localScale.x < plasma.transform.localScale.x)
            {
                Vector2 dist = collider.transform.position - transform.position;
                Vector2 oldVelocity = rb.velocity;
                rb.velocity += (dist / dist.sqrMagnitude) * (plasma.transform.localScale.x);
                float angleBetween = Vector2.Angle(oldVelocity, rb.velocity);
                transform.Rotate(0f, 0f, -angleBetween);
            }
        }
    }

    protected virtual void UpdateNotAiming()
    {
        Vector3 tp = wielder.transform.position;
        angle += rotationSpeed * Time.fixedDeltaTime;
        y += bobSpeed * Time.fixedDeltaTime;
        Vector3 offset = new Vector3(xRotation * Mathf.Sin(angle), yRotation * Mathf.Cos(angle), Mathf.Cos(angle)) * radius;

        orbit = new Vector3(tp.x + offset.x, tp.y + height + offset.y, tp.z + offset.z);
        Vector3 diff = orbit - transform.position;
        if (diff.magnitude <= 0.30f)
        {
            transform.position = orbit;
        }
        else
        {
            transform.position += diff * 0.2f;
        }
        transform.Rotate(0f, 0f, -transform.rotation.z * 5f);

    }

    protected virtual void UpdateAiming(Vector2 rightStick)
    {
        float z = (Mathf.Atan2(rightStick.y, rightStick.x) * 57.2958f);
        transform.rotation = Quaternion.Euler(0, 0, z);

        float x = wielder.transform.position.x - this.transform.position.x + (rightStick.x * 1.5f);
        float y = wielder.transform.position.y - this.transform.position.y + (rightStick.y * 1.5f);
        Vector3 movement = new Vector3(5 * x, 5 * y, 0);
        movement *= Time.fixedDeltaTime;
        transform.Translate(movement, Space.World);

        angle += rotationSpeed * Time.fixedDeltaTime;
    }

    #region Public Methods
    public void SetSpriteDirection(bool flipX)
    {
        sr.flipX = flipX;
    }

    public NPlayerController GetWielder()
    {
        return wielder;
    }

    public virtual void SetWielder(NPlayerController n, Player j)
    {
        wielder = n;
        joystick = j;
    }

    public void SetParameters(float a, float xr, float yr, float r, float h)
    {
        angle = a == Mathf.Infinity ? angle : a;
        xRotation = xr == Mathf.Infinity ? xRotation : xr;
        yRotation = yr == Mathf.Infinity ? yRotation : yr;
        radius = r == Mathf.Infinity ? radius : r;
        height = h == Mathf.Infinity ? height : h;
    }

    public virtual void Discharge(Vector2 angle, Collider2D playerCollider, bool flipX)
    {
        dischargeAngle = angle;
        held = false;
        rightStick = angle;
        rb.simulated = true;
        if (angle == Vector2.zero)
        {
            angle = flipX ? new Vector2(-1f, 0f) : new Vector2(1f, 0f);
            sr.flipX = flipX;
            rb.velocity = angle * speed;
            //rb.MovePosition(new Vector2(transform.position.x, playerCollider.transform.position.y));
            transform.position = new Vector2(transform.position.x, playerCollider.transform.position.y);
        }
        else
        {
            rb.velocity = angle * speed;
        }
        Physics2D.IgnoreCollision(cc, playerCollider, true);
        cc.enabled = true;
        transform.SetParent(null);
        animator.SetBool("discharged", true);
        StartCoroutine("LifetimeTimer");
    }

    public virtual void Shield()
    {

    }

    public void HitByPlasma(Vector3 blackHole)
    {
        rb.simulated = false;
        cc.enabled = false;
        transform.SetParent(null);
        timer = 0f;
        lifetime = 5f;
        StopAllCoroutines();
        IEnumerator plasmaCoroutine = CaughtByPlasma(blackHole);
        StartCoroutine(plasmaCoroutine);
    }

    public virtual void HitByFire()
    {

    }

    public virtual void HitByWater()
    {

    }

    public virtual void HitByAir(Vector2 normal, NPlayerController wielder)
    {

    }

    public virtual void HitByEarth()
    {

    }


    #endregion

    #region Private Helpers
    protected virtual IEnumerator LifetimeTimer()
    {
        yield return new WaitForSeconds(lifetime);
        Destroy(gameObject);
    }

    private IEnumerator CaughtByPlasma(Vector3 blackHole)
    {
        while ((timer += Time.deltaTime) < lifetime)
        {
            Vector3 diff = blackHole - transform.position;
            transform.position += diff * 0.075f;
            transform.Rotate(0f, 0f, 1f);
            transform.localScale *= 0.99f;
            yield return new WaitForEndOfFrame();
        }
        Destroy(gameObject);
    }
    #endregion
}
