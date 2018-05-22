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
    protected Vector2 rightStick;
    protected bool dischargeButton;
    protected Player joystick;
    private float x, y, z, angle, rotationSpeed, bobSpeed, radius, xRotation, yRotation, height;
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
	
	protected virtual void Update () {
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

    private void UpdateNotAiming()
    {
        Vector3 tp = wielder.transform.position;
        angle += rotationSpeed * Time.deltaTime;
        y += bobSpeed * Time.deltaTime;
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

    private void UpdateAiming(Vector2 rightStick)
    {
        float z = (Mathf.Atan2(rightStick.y, rightStick.x) * 57.2958f);
        transform.rotation = Quaternion.Euler(0, 0, z);

        float x = wielder.transform.position.x - this.transform.position.x + (rightStick.x * 1.5f);
        float y = wielder.transform.position.y - this.transform.position.y + (rightStick.y * 1.5f);
        Vector3 movement = new Vector3(5 * x, 5 * y, 0);
        movement *= Time.deltaTime;
        transform.Translate(movement, Space.World);

        angle += rotationSpeed * Time.deltaTime;
    }

    #region Public Methods
    public void SetWielder(NPlayerController n, Player j)
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

    public void Discharge(Vector2 angle)
    {
        held = false;
        rightStick = angle;
        rb.simulated = true;
        rb.velocity = angle * speed;
        cc.enabled = true;
        transform.SetParent(null);
        animator.SetBool("discharged", true);
        StartCoroutine("LifetimeTimer");
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
    #endregion

    #region Private Helpers
    private IEnumerator LifetimeTimer()
    {
        while ((timer += Time.deltaTime) < lifetime)
        {
            yield return new WaitForEndOfFrame();
        }
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
