using UnityEngine;
using System.Collections;

public class PlayerWeaponScript : MonoBehaviour
{
    private AnimationController2D animator;
    public RockScript rockScript;

    // ShotScript code
    public PlayerController caster;
    private bool atCaster = false;
    private bool right = true;
    public string shotType = "default";

    public int damage = 1;
    private float sizeX;

    public Vector2 speed = new Vector2(0f, 0f);
    public Vector2 direction = new Vector2(1, 1);

    public bool hasShot { get; set; }
    public bool fire = false;
    private bool connected = false;
    public bool rock = false;

    void Start()
    {
        rockScript = gameObject.GetComponent<RockScript>();
        animator = gameObject.GetComponent<AnimationController2D>();
        sizeX = transform.localScale.x;
        hasShot = false;
    }

    void Update()
    {
        if (!rock)
        {
            // Regular weapons
            if (caster != null)
            {
                // Weapon is following player
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
                    if (!rock && (X > 2 || X < -2) || (Y > 0.1 || Y < -0.1))
                    {
                        atCaster = false;
                        Vector3 movement = new Vector3((speed.x / 2) * X, (speed.y / 2) * Y, 0);
                        movement *= Time.deltaTime;
                        this.transform.Translate(movement);
                    }

                    else
                    {
                        hasShot = true;
                        atCaster = true;
                    }
                }

                // Weapon has been fired
                if (fire && !connected)
                {

                    if (shotType.Equals("fire"))
                        animator.setAnimation("Fireball Shoot");

                    if (shotType.Equals("plasma"))
                        animator.setAnimation("plasmaShoot");

                    hasShot = false;

                    Vector3 movement = new Vector3(speed.x * direction.x, -(speed.y * direction.y), 0);
                    if (shotType.Equals("rock"))
                    {
                        movement = new Vector3(speed.x * direction.x * 1.5f, 0, 0);
                        speed.x -= 0.2f;
                    }
                    movement *= Time.deltaTime;
                    transform.Translate(movement);

                }

                // Shot connected
                if (connected)
                {
                    if (shotType.Equals("fire"))
                    {
                        animator.setAnimation("Fireball Explosion");
                        if (transform.localScale.x < (4 * sizeX))
                        {
                            transform.localScale = new Vector3(transform.localScale.x * 1.05f, transform.localScale.y * 1.05f, 1);
                        }
                        else
                            Destroy(gameObject);
                    }
                    else if (shotType.Equals("plasma"))
                    {
                        animator.setAnimation("plasmaExplosion");
                        if (transform.localScale.x < (4 * sizeX))
                        {
                            transform.localScale = new Vector3(transform.localScale.x * 1.05f, transform.localScale.y * 1.05f, 1);
                        }
                    }
                }
            }
            else
            {
                Destroy(gameObject);
            }
        }

        // Rock
        if (rock)
        {
            if (caster != null)
            {
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


                    float Xrock = (caster.transform.position.x + (1 * direction.x)) - this.transform.position.x;
                    float Y = caster.transform.position.y - this.transform.position.y;
                    if ((Xrock > 1.5 || Xrock < -1.5) || (Y > 0.1 || Y < -0.1))
                    {

                        atCaster = false;
                        Vector3 movement = new Vector3((speed.x / 2) * Xrock, (speed.y / 2) * Y, 0);
                        movement *= Time.deltaTime;
                        this.transform.Translate(movement);
                    }
                    else
                    {
                        hasShot = true;
                    }
                }

                // Weapon has been fired
                if (fire && speed.x > 0)
                {
                    gameObject.GetComponent<Rigidbody2D>().gravityScale = 7;
                    hasShot = false;

                    Vector3 movement = new Vector3(speed.x * direction.x, 0, 0);
                    if (shotType.Equals("rock"))
                    {
                        movement = new Vector3(speed.x * direction.x * 1.5f, speed.y * direction.y * 1.5f, 0);
                        speed.x -= 0.5f;
                    }
                    movement *= Time.deltaTime;
                    transform.Translate(movement);

                }

                // Shot connected
                if (connected)
                {
                    fire = false;
                    connected = false;
                    hasShot = false;
                    speed.x = 20;
                    caster = null;
                }

                // Rock has stopped
                if (speed.x <= 0)
                {
                    fire = false;
                    connected = false;
                    hasShot = false;
                    speed.x = 20;
                    caster = null;
                }
            }
        }
        

    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        PlayerWeaponScript projectile = collider.gameObject.GetComponent<PlayerWeaponScript>();
        PlayerController player = collider.gameObject.GetComponent<PlayerController>();
        if (projectile != null && shotType.Equals("rock") && !projectile.caster.Equals(this.caster))
        {
            Destroy(collider.gameObject);
        }
        else if (player != null && !caster.Equals(player))
        {
            if (shotType.Equals("air"))
            {
                player.pushed = true;
            }
        }
    }

    public void Attack(float X, float Y)
    {
        direction.x = X;
        direction.y = Y;
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

    public void Burn()
    {
        //wtf;	
    }
}
