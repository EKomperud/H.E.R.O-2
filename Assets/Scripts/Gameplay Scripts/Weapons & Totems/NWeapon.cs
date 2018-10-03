using System.Collections;
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
    protected Vector2 leftStick;
    protected Vector2 rightStick;
    protected Vector2 dischargeAngle;
    protected bool dischargeButton;
    protected bool mobilityButton;
    protected bool shieldButton;
    protected bool activeShield;
    protected Player joystick;
    protected float x, y, z, angle, rotationSpeed, bobSpeed, radius, xRotation, yRotation, height;
    protected bool held;
    protected bool shield;
    protected bool mobility;
    protected float lifetime = 3f;
    protected float timer;
    protected Vector3 _shieldGrowthRate;

    [SerializeField] protected float speed;
    [SerializeField] protected EElement element;

    [Header("Shield Attributes")]
    [SerializeField] protected int shieldFrames;
    [SerializeField] protected float shieldSize;
    [SerializeField] protected float shieldGrowthRate;

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
        _shieldGrowthRate = new Vector3(shieldGrowthRate, shieldGrowthRate, 0f);
    }

    void Update()
    {
        if (held && this.Equals(wielder.GetWeapon()) && wielder.GetWeaponCooled())
        {
            leftStick = new Vector2(joystick.GetAxis("Move Horizontal"), joystick.GetAxis("Move Vertical"));
            rightStick = new Vector2(joystick.GetAxis("Aim Horizontal"), joystick.GetAxis("Aim Vertical"));
            dischargeButton = joystick.GetButtonDown("Discharge Weapon");
            mobilityButton = joystick.GetButtonDown("Mobility");

            if (joystick.GetButtonDown("Shield") || (shieldButton && joystick.GetButton("Shield")))
                shieldButton = true;
            else if (!joystick.GetButton("Shield"))
                shieldButton = false;

            if (dischargeButton)
                Discharge();

            if (mobilityButton)
                Mobility();
        }
    }

    protected virtual void FixedUpdate () {
        if (held)
        {
            if (this.Equals(wielder.GetWeapon()))
            {
                if (shieldButton || activeShield)
                    UpdateShielding();
                else if (rightStick != Vector2.zero)
                    UpdateAiming(rightStick);
                else
                    UpdateNotAiming();
            }
            else
            {
                if (activeShield)
                    UpdateShielding();
                else
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
        //if (this.Equals(wielder.GetWeapon()))
        //    wielder.SetAnimatorBools("casting", false);
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
        //if (this.Equals(wielder.GetWeapon()))
        //    wielder.SetAnimatorBools("casting", true);
        float z = (Mathf.Atan2(rightStick.y, rightStick.x) * 57.2958f);
        transform.rotation = Quaternion.Euler(0, 0, z);

        float x = wielder.transform.position.x - this.transform.position.x + (rightStick.x * 1.5f);
        float y = wielder.transform.position.y - this.transform.position.y + (rightStick.y * 1.5f);
        Vector3 movement = new Vector3(10 * x, 10 * y, 0);
        movement *= Time.fixedDeltaTime;
        transform.Translate(movement, Space.World);

        angle += rotationSpeed * Time.fixedDeltaTime;
    }

    protected virtual void UpdateShielding()
    {
        if (!activeShield)
        {
            //if (this.Equals(wielder.GetWeapon()))
            //    wielder.SetAnimatorBools("casting", true);
            transform.position = new Vector3(transform.position.x, transform.position.y, -1f);
            float x = wielder.transform.position.x - this.transform.position.x;
            float y = wielder.transform.position.y - this.transform.position.y;
            Vector3 movement = new Vector3(10 * x, 10 * y, 0f);
            movement *= Time.fixedDeltaTime;
            transform.Translate(movement, Space.World);

            float direction = (Mathf.Atan2(y, x) * 57.2958f);
            transform.rotation = Quaternion.Euler(0, 0, direction);

            angle += rotationSpeed * Time.fixedDeltaTime;
            if (Vector2.Distance(transform.position, wielder.transform.position) <= 0.05f)
                Shield();
        }
        else
        {
            if (transform.localScale.x <= shieldSize)
                transform.localScale += _shieldGrowthRate;

            rb.MovePosition(wielder.GetNextFramePosition());

            if (--shieldFrames == 0)
                StartCoroutine("ShieldDissipate");
        }
    }

    #region Public Methods
    public void SetRightStick(Vector2 rs)
    {
        rightStick = rs;
    }

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

    public virtual void Discharge()
    {
        bool flipX = wielder.GetFlipX();
        float flip = flipX ? -1 : 1;
        wielder.SetAnimatorFloats("attackX", rightStick.x * flip);
        wielder.SetAnimatorFloats("attackY", rightStick.y);
        wielder.SetAnimatorTriggers("attacking");

        dischargeAngle = rightStick;
        Collider2D playerCollider = wielder.GetCollider();
        Physics2D.IgnoreCollision(cc, playerCollider, true);

        held = false;
        rb.simulated = true;
        if (dischargeAngle == Vector2.zero)
        {
            dischargeAngle = flipX ? new Vector2(-1f, 0f) : new Vector2(1f, 0f);
            sr.flipX = flipX;
            rb.velocity = dischargeAngle * speed;
            transform.position = new Vector2(transform.position.x, playerCollider.transform.position.y);
        }
        else
        {
            rb.velocity = dischargeAngle * speed;
        }
        cc.enabled = true;
        transform.SetParent(null);
        animator.SetBool("discharged", true);
        IEnumerator lifeTimer = LifetimeTimer(0f);
        StartCoroutine(lifeTimer);
        wielder.WeaponUsed();
    }

    public virtual void Mobility()
    {
        wielder.WeaponUsed();
    }

    public virtual void Shield()
    {
        activeShield = true;
        transform.position = wielder.transform.position;
        gameObject.layer = LayerMask.NameToLayer("Shield");
        Collider2D playerCollider = wielder.GetCollider();
        Physics2D.IgnoreCollision(cc, playerCollider, true);
        rb.simulated = true;
        cc.enabled = true;
        animator.SetBool("shielded", true);
        wielder.WeaponUsed();
    }

    public virtual void WielderDied()
    {
        Destroy(gameObject);
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

    public virtual void HitByFire(Collision2D collision)
    {

    }

    public virtual void HitByWater(Collision2D collision)
    {

    }

    public virtual void HitByAir(Vector2 normal, NPlayerController wielder)
    {

    }

    public virtual void HitByEarth()
    {

    }

    public void HitByPlasmaShield(Vector3 blackHole)
    {
        
    }

    public virtual void HitByFireShield(Collision2D collision)
    {

    }

    public virtual void HitByWaterShield(Collision2D collision)
    {

    }

    public virtual void HitByAirShield(Vector2 normal, NPlayerController wielder)
    {

    }

    public virtual void HitByEarthShield()
    {

    }


    #endregion

    #region Private Helpers
    protected virtual IEnumerator ShieldDissipate()
    {
        animator.SetBool("dissipated", true);
        cc.enabled = false;
        rb.simulated = false;
        yield return new WaitForSeconds(0.25f);
        Destroy(gameObject);
    }

    protected virtual IEnumerator LifetimeTimer(float l)
    {
        l = l == 0 ? lifetime : l;
        yield return new WaitForSeconds(l);
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
