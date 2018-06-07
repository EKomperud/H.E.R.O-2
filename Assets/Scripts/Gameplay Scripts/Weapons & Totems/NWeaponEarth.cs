using System.Collections;
using System.Collections.Generic;
using Rewired;
using UnityEngine;

public class NWeaponEarth : NWeapon {

    bool activeWeapon;
    bool decelerating;
    bool falling;
    [SerializeField] private float decelerationFactor;
    [SerializeField] private float activeTime;
    [SerializeField] private float gravityScale;
    private float _activeTime;

    protected override void Start()
    {
        // Initialize weapon rotation members
        x = y = z = 0;
        rotationSpeed = 4f;
        bobSpeed = rotationSpeed * 2;

        // Set and initialize references
        rb = GetComponent<Rigidbody2D>();
        rb.simulated = false;
        cc = GetComponent<Collider2D>();
        cc.enabled = false;
        animator = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();

        // Initialize state variables
        held = false;
        activeWeapon = false;
        decelerating = false;
        falling = false;
        animator.SetBool("gathering", false);
        animator.SetBool("collided", false);
    }

    protected override void FixedUpdate()
    {
        if (activeWeapon)
        {
            if (held)
            {
                rightStick = new Vector2(joystick.GetAxis("Aim Horizontal"), joystick.GetAxis("Aim Vertical"));
                if (rightStick != Vector2.zero && this.Equals(wielder.GetWeapon()))
                    UpdateAiming(rightStick);
                else
                    UpdateNotAiming();
            }
            else
            {
                if (!decelerating)
                {
                    if ((_activeTime -= Time.fixedDeltaTime) <= 0)
                    {
                        decelerating = true;
                        wielder = null;
                    }
                }
                else if (decelerating && !falling)
                {
                    rb.velocity *= decelerationFactor;
                    if (rb.velocity.sqrMagnitude <= 0.0025f)
                    {
                        falling = true;
                        rb.velocity = Vector2.zero;
                        rb.gravityScale = gravityScale;
                    }
                }
                else if (falling)
                {
                    if (BottomCheck())
                    {
                        // Reset state variables
                        animator.SetBool("gathering", false);
                        animator.SetBool("collided", true);
                        activeWeapon = false;
                        held = false;
                        decelerating = false;
                        falling = false;

                        // Reset reference variables
                        transform.rotation = new Quaternion();
                        rb.simulated = false;
                        cc.enabled = false;
                    }
                }
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        NPlayerController np = collision.collider.gameObject.GetComponent<NPlayerController>();
        NWeapon w = collision.collider.gameObject.GetComponent<NWeapon>();
        if (np != null && np.GetLivingStatus() && np != wielder)
        {
            np.HitByEarth(-collision.contacts[0].normal);
        }
        if (w != null)
        {
            w.HitByEarth();
        }
        else if (collision.collider.gameObject.layer.Equals(LayerMask.NameToLayer("Platforms")) && !(wielder != null))
        {
            wielder = null;
            decelerating = true;
            falling = true;
            rb.gravityScale = gravityScale;
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        NWeapon w = collider.gameObject.GetComponent<NWeapon>();
        if (w != null)
        {
            w.HitByEarth();
        }
    }

    protected override void UpdateNotAiming()
    {
        Vector3 tp = wielder.transform.position;
        angle += rotationSpeed * Time.fixedDeltaTime;
        y += bobSpeed * Time.fixedDeltaTime;
        Vector3 offset = new Vector3(xRotation * Mathf.Sin(angle), yRotation * Mathf.Cos(angle), Mathf.Cos(angle)) * radius;

        orbit = new Vector3(tp.x + offset.x, tp.y + height + offset.y, tp.z + offset.z);
        Vector3 diff = orbit - transform.position;
        if (diff.magnitude <= 0.40f)
        {
            transform.position = orbit;
        }
        else
        {
            transform.position += diff * 0.2f;
        }
        transform.Rotate(0f, 0f, -transform.rotation.z * 2.5f);
    }

    protected override void UpdateAiming(Vector2 rightStick)
    {
        float z = (Mathf.Atan2(rightStick.y, rightStick.x) * 57.2958f);
        transform.rotation = Quaternion.Euler(0, 0, z);

        float x = wielder.transform.position.x - transform.position.x + (rightStick.x * 1.5f);
        float y = wielder.transform.position.y - transform.position.y + (rightStick.y * 1.5f);
        Vector3 movement = new Vector3(5 * x, 5 * y, 0);
        movement *= Time.fixedDeltaTime;
        transform.Translate(movement, Space.World);

        angle += rotationSpeed * Time.fixedDeltaTime;
    }

    public override void SetWielder(NPlayerController n, Player j)
    {
        // Set wielder
        base.SetWielder(n, j);

        // Set state variables
        held = true;
        decelerating = false;
        falling = false;
        activeWeapon = true;
        animator.SetBool("gathering", true);
        animator.SetBool("collided", false);
        _activeTime = activeTime;

        // Set reference variables
        rb.simulated = true;
        rb.gravityScale = 0f;
        cc.enabled = true;
        cc.isTrigger = true;

        // Move to character
        Vector3 pct = wielder.transform.position;
        transform.position = new Vector3(pct.x + 0.5f, pct.y + 0.5f, pct.z);
        orbit = transform.position;
        
    }

    public override void Discharge(Vector2 angle)
    {
        // Unset joystick control
        joystick = null;

        // Set state variables
        held = false;

        // Set reference variables
        rightStick = angle;
        rb.velocity = angle * speed;
        cc.isTrigger = false;
        transform.SetParent(null);
    }

    protected bool BottomCheck()
    {
        double x = transform.position.x + cc.offset.x;
        double y = transform.position.y + cc.offset.y;
        CircleCollider2D circle = (CircleCollider2D)cc;
        float dist = (float)(circle.radius) + 0.05f;
        RaycastHit2D rch = Physics2D.Raycast(new Vector2((float)x, (float)y), Vector2.down, dist, LayerMask.GetMask("Platforms"));
        return rch.collider != null;
    }
}
