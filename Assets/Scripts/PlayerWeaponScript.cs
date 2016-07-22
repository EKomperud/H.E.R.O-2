using UnityEngine;
using System.Collections;

public class PlayerWeaponScript : MonoBehaviour
{
    private AnimationController2D animator;
    public RockScript rockScript;
    private Rigidbody2D rb2d;

    // ShotScript code
    public PlayerController caster;
    private bool atCaster = false;
    private bool right = true;
    public string shotType = "default";
    private float destroyCountdown = 0.25f;
    public int damage = 1;
    private float sizeX;

    public Vector2 speed = new Vector2(0f, 0f);
    public Vector2 direction = new Vector2(1, 1);

    public bool hasShot { get; set; }
    public bool fire = false;
    public bool connected = false;
    public bool rock = false;
    private float airTime;

    void Start()
    {
        rockScript = gameObject.GetComponent<RockScript>();
        animator = gameObject.GetComponent<AnimationController2D>();
        rb2d = GetComponent<Rigidbody2D>();
        sizeX = transform.localScale.x;
        hasShot = false;
        switch (shotType)
        {
            case "air":
                animator.setAnimation("mostlyBlank");
                break;
            case "water":
                animator.setAnimation("waterIdle");
                break;
            case "fire":
                animator.setAnimation("fireIdle");
                break;
            case "plasma":
                animator.setAnimation("plasmaIdle");
                break;
        }

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
                    float inputX = MultiInput.GetAxis("RightJoystickX", "", caster.name);
                    float inputY = MultiInput.GetAxis("RightJoystickY", "", caster.name);
                    float z = -(Mathf.Atan2(inputY, inputX) * 57.2958f);
                    Vector2 mag = new Vector2(inputX, inputY);
                    if (mag.magnitude < 0.15f)
                    {
                        inputX = 0;
                        inputY = 0;
                    }

                    float X = caster.transform.position.x - this.transform.position.x + inputX;
                    float Y = caster.transform.position.y - this.transform.position.y - inputY;
                    float Z = (Mathf.Atan2(Y, X) * 57.2958f);

                    if ((X > 0.1 || X < -0.1) || (Y > 0.1 || Y < -0.1))
                    {
                        if (shotType.Equals("water")) {
                            if (mag.magnitude >= 0.5)
                            {
                                    gameObject.transform.rotation = Quaternion.Euler(0, 0, z);
                            }
                            else
                            {
                                gameObject.transform.rotation = Quaternion.Euler(0, 0, Z);
                            }
                        }
                        atCaster = false;
                        Vector3 movement = new Vector3((speed.x / 2) * X, (speed.y / 2) * Y, 0);
                        movement *= Time.deltaTime;
                        transform.Translate(movement, Space.World);
                    }

                    else
                    {
                        hasShot = true;
                        atCaster = true;
                        if (!caster.faceRight)
                        {
                            transform.rotation = Quaternion.Euler(180, 0, 180);
                        }
                    }
                }

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
                        animator.setAnimation("plasmaSpin");
                        if (transform.localScale.x < (4 * sizeX))
                        {
                            transform.localScale = new Vector3(transform.localScale.x * 1.05f, transform.localScale.y * 1.05f, 1);
                        }
                    }
                    else if (shotType.Equals("air"))
                    {
                        Destroy(gameObject);
                    }
                    else if (shotType.Equals("water"))
                    {
                        animator.setAnimation("waterFreeze");
                        if (destroyCountdown >=0)
                        {
                            destroyCountdown -= Time.deltaTime;
                        }
                        else
                            Destroy(gameObject);
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
                rb2d.isKinematic = true;
                if (!fire)
                {




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
            }
            else
                rb2d.isKinematic = false;
        }       
    }

    void FixedUpdate()
    {
        if (rock)
        {
            if (caster != null)
            {
                rb2d.isKinematic = true;
                if (!fire)
                {
                    if (caster.faceRight)
                        direction.x = 1;
                    else
                        direction.x = -1;

                    float inputX = MultiInput.GetAxis("RightJoystickX", "", caster.name);
                    float inputY = MultiInput.GetAxis("RightJoystickY", "", caster.name);
                    Vector2 mag = new Vector2(inputX, inputY);
                    if (mag.magnitude < 0.1f)
                    {
                        inputX = 0;
                        inputY = 0;
                    }
                    float Xrock = (caster.transform.position.x + (1 * direction.x)) - this.transform.position.x + inputX;
                    float Y = caster.transform.position.y - this.transform.position.y - inputY;
                    if ((Xrock > 1.5 || Xrock < -1.5) || (Y > 0.1 || Y < -0.1))
                    {
                        atCaster = false;
                        Vector2 movement = new Vector2((speed.x / 2) * Xrock, (speed.y / 2) * Y);
                        movement *= Time.deltaTime;
                        rb2d.MovePosition(rb2d.position + (movement * 25) * Time.fixedDeltaTime);
                    }
                    else
                    {
                        hasShot = true;
                    }
                }
            }


            // Weapon has been fired
            if (fire && speed.x >= 1)
            {
                gameObject.GetComponent<Rigidbody2D>().gravityScale = 0;
                hasShot = false;

                Vector2 movement = new Vector2(speed.x * direction.x * 1.5f, -(speed.y * direction.y * 1.5f));
                speed.x = (speed.x - speed.x * 0.075f);
                speed.y = (speed.y - speed.y * 0.1f);
                
                movement *= Time.deltaTime;
                rb2d.MovePosition(rb2d.position + (movement*50) * Time.fixedDeltaTime);
                //rb2d.AddForce((movement * 500)); // * Time.fixedDeltaTime);
            }
            else if (speed.x < 1)
            {
                caster = null;
                fire = false;
                connected = false;
                hasShot = false;
                caster = null;
                speed.x = 15;
                speed.y = 10;
                gameObject.GetComponent<Rigidbody2D>().gravityScale = 6;
            }
        }
        else
        {
            if (fire && !connected)
            {

                if (shotType.Equals("fire"))
                    animator.setAnimation("Fireball Shoot");

                if (shotType.Equals("plasma"))
                    animator.setAnimation("plasmaShoot");

                if (shotType.Equals("water"))
                    animator.setAnimation("waterShoot");

                if (shotType.Equals("air"))
                {
                    if (animator.getAnimation().Equals("mostlyBlank"))
                    {
                        Destroy(gameObject);
                    }
                    //animator.setAnimation("airShoot");
                    airTime += Time.deltaTime;
                }

                hasShot = false;
                Vector2 movement = new Vector2(speed.x * direction.x, -(speed.y * direction.y));
                //Debug.Log("" + direction.x);
                movement *= Time.deltaTime;
                rb2d.MovePosition(rb2d.position + (movement*50) * Time.fixedDeltaTime);

            }
        }
    }

    void OnTriggerEnter2D(Collider2D collider)
    {

        PlayerWeaponScript projectile = collider.gameObject.GetComponent<PlayerWeaponScript>();
        PlayerController player = collider.gameObject.GetComponent<PlayerController>();

        // Collided with another projectile
        if (projectile != null  && caster != null && !projectile.caster.Equals(this.caster))
        {
            if (shotType.Equals("air"))
            {
                if (fire)
                {
                    if (projectile.shotType.Equals("fire") || projectile.shotType.Equals("water"))
                    {
                        projectile.direction.x *= -1;
                        projectile.direction.y *= -1;
                    }
                    Destroy(gameObject);
                }
            }
            if (shotType.Equals("fire"))
            {
                if (projectile.shotType.Equals("water"))
                {
                    Destroy(projectile.gameObject);
                    Destroy(gameObject);
                }
            }
            if (shotType.Equals("water"))
            {
                
            }
        }
        // Collided with a player
        else if (player != null && fire)
        {
            if (shotType.Equals("rock") && speed.x > 1 && (caster == null || !caster.Equals(player)))
            {
                Debug.Log("rock collision");
                float mag = speed.magnitude;
                HealthScript h = player.GetComponent<HealthScript>();
                h.ManualDamage(1);
            }
            if (!caster.Equals(player))
            {
                if (shotType.Equals("air"))
                {
                    float apv = speed.magnitude;
                    player.AirPushVelocity = (5 / airTime) * direction.x;
                    player.pushed = true;
                    connected = true;
                }
                if (shotType.Equals("water"))
                {
                    animator.setAnimation("waterFreeze");
                    player.jumpHeight = 0.25f;
                    player.frozen = true;
                    player.freezeWarmup = player.freezeTime;
                    connected = true;
                    speed = new Vector2(2, 2);
                }
                if (shotType.Equals("fire"))
                {
                    if (!player.burning)
                    {
                        player.burning = true;
                        player.burnDown = player.burnTime;
                        Transform burnParticles = player.transform.GetChild(0);
                        ParticleSystem nbaJamOnFireEdition = burnParticles.GetComponent<ParticleSystem>();
                        nbaJamOnFireEdition.Play();
                        connected = true;
                    }
                }
                if (shotType.Equals("plasma"))
                {
                    connected = true;
                }
            }
        }

    }

    void RaycastTrigger(Collider2D collider)
    {
        WallScript wall = collider.gameObject.GetComponent<WallScript>();
        if (wall != null && !wall.bounceConstraint)
        {
            direction.x *= -1;
            direction.y *= -1;
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
            if (!rock)
                animator.setAnimation(shotType + "Shoot");       
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
}
