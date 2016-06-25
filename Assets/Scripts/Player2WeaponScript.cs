using UnityEngine;
using System.Collections;

public class Player2WeaponScript : MonoBehaviour
{

    //public Transform shotPrefab;

    //public float shootingRate = 0.25f;

    //private float shootCooldown;

    // ShotScript code
    public Player2Controller caster;
    private bool atCaster = false;
    public string shotType = "default";

    public int damage = 1;
    private float timer = 0f;
    private float sizeX;

    public Vector2 speed = new Vector2(0f, 0f);
    public Vector2 direction = new Vector2(1, 0);

    public bool hasShot { get; set; }
    public bool fire = false;
    private bool connected = false;

    void Start()
    {
        //shootCooldown = 0f;
        sizeX = transform.localScale.x;
        hasShot = false;
    }

    void Update()
    {
        // Shot is following player

        if (!fire)
        {
            if (caster.faceRight)
            {
                direction.x = 1;
            }
            else
            {
                direction.x = -1;
            }


            float X = caster.transform.position.x - this.transform.position.x;
            float Y = caster.transform.position.y - this.transform.position.y;
            if ((X > 1 || X < -1) || (Y > 0.3 || Y < -0.3))
            {
                Vector3 movement = new Vector3((speed.x / 2) * X, (speed.y / 2) * Y, 0);
                movement *= Time.deltaTime;
                this.transform.Translate(movement);
            }
            else
            {
                hasShot = true;
            }
        }

        // Weapon has been fired
        if (fire)
        {
            hasShot = false;
            Destroy(gameObject, 10);

            Vector3 movement = new Vector3(speed.x * direction.x, 0, 0);
            movement *= Time.deltaTime;
            transform.Translate(movement);
        }
        timer = Time.deltaTime;

        // Shot connected
        if (connected)
        {
            if (shotType.Equals("fire"))
            {
                if (transform.localScale.x < (4 * sizeX))
                {
                    transform.localScale = new Vector3(transform.localScale.x * 1.05f, transform.localScale.y * 1.05f, 1);
                }

                else
                    Destroy(gameObject);
            }
        }



    }

    public void Attack()
    {
        if (hasShot)
        {
            fire = true;
        }
    }

    public bool CanAttack
    {
        get
        {
            return hasShot;
        }
    }

    public void MoveToCaster()
    {
        float X = caster.transform.position.x - this.transform.position.x;
        float Y = caster.transform.position.y - this.transform.position.y;
        Vector3 movement = new Vector3(speed.x * X, speed.y * Y, 0);
        movement *= Time.deltaTime;
        this.transform.Translate(movement);
    }

    public void Flip()
    {
        Vector3 weaponScale = transform.localScale;
        weaponScale.x *= -1;
        transform.localScale = weaponScale;
    }

    public void Destroy()
    {
        speed = new Vector2(2, 2);
        connected = true;
    }
}