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
    [SerializeField] private float aimRadius;
    [SerializeField] private float followSpeed;
    private float _activeTime;
    protected RaycastHit2D[] rch0 = new RaycastHit2D[1];
    protected RaycastHit2D[] rch1 = new RaycastHit2D[1];
    protected RaycastHit2D[] rch2 = new RaycastHit2D[1];
    protected RaycastHit2D[] hits;
    protected MovingPlatform mp;
    protected Vector2 gravityVector;

    protected override void Start()
    {
        // Initialize weapon movement members
        x = y = z = 0;
        rotationSpeed = 4f;
        bobSpeed = rotationSpeed * 2;
        gravityVector = new Vector2(0, -gravityScale);

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
                    BottomCheck();
                    if (GroundOrPlayerCheck())
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
                        rb.gravityScale = 0f;
                        cc.isTrigger = true;
                    }
                }
            }
        }
        else
        {
            BottomCheck();
            rb.velocity = GroundOrPlayerCheck() ? Vector2.zero : rb.velocity + gravityVector;
            mp = MovingPlatformCheck();
            if (mp != null)
            {
                rb.MovePosition(rb.position + mp.GetLastFrameMovement() + (rb.velocity * Time.fixedDeltaTime));
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
        else if (collision.collider.gameObject.layer.Equals(LayerMask.NameToLayer("Platforms")) && (wielder != null))
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
        if (diff.magnitude <= 0.20f)
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

        float x = wielder.transform.position.x - transform.position.x + (rightStick.x * aimRadius);
        float y = wielder.transform.position.y - transform.position.y + (rightStick.y * aimRadius);
        Vector2 movement = new Vector3(followSpeed * x, followSpeed * y);
        movement *= Time.fixedDeltaTime;
        rb.MovePosition(rb.position + movement);

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

    public override void Discharge(Vector2 angle, Collider2D playerCollider, bool flipX)
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

    protected void BottomCheck()
    {
        double x = transform.position.x + cc.offset.x;
        double y = transform.position.y + cc.offset.y;
        CircleCollider2D circle = (CircleCollider2D)cc;
        float dist = (float)(circle.radius) + 0.05f;
        rch0[0] = rch1[0] = rch2[0] = new RaycastHit2D();
        Physics2D.RaycastNonAlloc(new Vector2((float)x - circle.radius, (float)y), Vector2.down, rch0, dist, LayerMask.GetMask("Platforms", "Player"));
        Physics2D.RaycastNonAlloc(new Vector2((float)x, (float)y), Vector2.down, rch1, dist, LayerMask.GetMask("Platforms", "Player"));
        Physics2D.RaycastNonAlloc(new Vector2((float)x + circle.radius, (float)y), Vector2.down, rch2, dist, LayerMask.GetMask("Platforms", "Player"));
    }

    protected bool GroundOrPlayerCheck()
    {
        return (rch0[0].collider != null || rch1[0].collider != null || rch2[0].collider != null);
    }

    protected MovingPlatform MovingPlatformCheck()
    {
        if (rch1[0].collider != null && rch1[0].collider.gameObject.tag.Equals("MovingPlatform"))
            return rch0[0].collider.gameObject.GetComponent<MovingPlatform>();
        if (rch1[0].collider != null && rch1[0].collider.gameObject.tag.Equals("MovingPlatform"))
            return rch0[0].collider.gameObject.GetComponent<MovingPlatform>();
        if (rch2[0].collider != null && rch2[0].collider.gameObject.tag.Equals("MovingPlatform"))
            return rch0[0].collider.gameObject.GetComponent<MovingPlatform>();
        return null;
    }
}
