using UnityEngine;
using System.Collections;
using Prime31;

public class PlayerController : MonoBehaviour
{

    // Balance numbers
    public float airFireRate = 2f;
    private float airCooldown;
    public float fireRate = 0.3f;
    private float fireCooldown;
    public float walkSpeed = 3;
    public float gravity = -35;
    public float jumpHeight = 2;
    public float maxHeight = 20;
    public float RunArrowStrength = 75;

    // Character objects
    private CharacterController2D _controller;
    private AnimationController2D _animator;
    public AudioClip DeathScreamWhenYouDie;
    private AudioSource audio;
    public Transform defaultShot;
    public HealthScript health;
    public FountainScript fountain;
    public PlayerWeaponScript weapon;
    public RockScript rock;
    public Stack weapons;
    public Vector2 speed = new Vector2(50, 50);

    // Operation booleans
    public bool nearRock = false;
    public bool inFountain = false;
    public string name;
    public bool pushed = false;
    public bool faceRight = false;
    private bool Bounce = false;
    private bool doubleJump = false;
    private bool doubleJumped = false;
    private bool slide = false;
    private bool RunRight = false;
    private bool RunLeft = false;
    public bool RunArrows = false;
    private bool AirControl = true;
    public bool fallDeath = false;
    private bool oldTrigger;
    private bool newTrigger = true;

    // Use this for initialization
    void Start()
    {
        airCooldown = 0;
        health = gameObject.GetComponent<HealthScript>();
        _controller = gameObject.GetComponent<CharacterController2D>();
        _animator = gameObject.GetComponent<AnimationController2D>();
        weapons = new Stack();
    }

    void Awake()
    {
        var shot = Instantiate(defaultShot) as Transform;
        shot.position = transform.position;
        PlayerWeaponScript shotScript = shot.gameObject.GetComponent<PlayerWeaponScript>();
        weapon = shotScript;
        shotScript.caster = this;
        shotScript.MoveToCaster();
    }

    // Update is called once per frame
    void Update()
    {
        if (airCooldown > 0)
        {
            airCooldown -= Time.deltaTime;
        }
        if (airCooldown <= 0 && weapon == null && !(weapons.Count >0) )
        {
            Debug.Log("spawn air");
            var shot = Instantiate(defaultShot) as Transform;
            shot.position = transform.position;
            PlayerWeaponScript shotScript = shot.gameObject.GetComponent<PlayerWeaponScript>();
            weapon = shotScript;
            shotScript.caster = this;
            shotScript.MoveToCaster();
        }
        if (fireCooldown > 0)
        {
            fireCooldown -= Time.deltaTime;
        }
        Vector3 velocity = _controller.velocity;

        // Gravity: pull player down
        if (!Bounce)
        {
            velocity.y += gravity * Time.deltaTime;
        }

        // Player is bouncing
        else if (Bounce)
        {
            if (velocity.y > 0)
            {
                velocity.y *= 2.25f;
            }
            else if (velocity.y < 9 && velocity.y > -9)
            {
                velocity.y = 20;
            }
            else
            {
                velocity.y += -velocity.y * 2.25f;
            }
            if (velocity.y > maxHeight)
            {
                velocity.y = maxHeight;
            }
            Bounce = false;
        }

        // Player is moving horizontally
        if (MultiInput.GetAxis("Horizontal", "", name) < 0 || MultiInput.GetAxis("LeftJoystickX", "", name) < 0)
        {
            if (slide)
            {
                if (velocity.x > 0)
                {
                    velocity.x += -walkSpeed * 0.002f;
                }
                else
                {
                    velocity.x += -walkSpeed * 0.11f;
                }
            }
            else if (RunLeft | RunRight)
            {
                RunArrows = true;

            }
            else
            {
                velocity.x = -walkSpeed;
            }
            if (!_controller.isGrounded)
            {
                _animator.setAnimation("Jump");
            }
            else
            {
                _animator.setAnimation("Cyborg");
            }
            _animator.setFacing("Left");
            faceRight = false;
        }
        else if (MultiInput.GetAxis("Horizontal", "", name) > 0 || MultiInput.GetAxis("LeftJoystickX", "", name) > 0)
        {
            if (slide)
            {
                if (velocity.x < 0)
                {
                    velocity.x += walkSpeed * 0.002f;
                }
                else
                {
                    velocity.x += walkSpeed * 0.11f;
                }
            }
            else if (RunLeft | RunRight)
            {
                RunArrows = true;

            }
            else
            {
                velocity.x = walkSpeed;
            }
            if (!_controller.isGrounded)
            {
                _animator.setAnimation("Jump");
            }
            else
            {
                _animator.setAnimation("Cyborg");
            }
            _animator.setFacing("Right");
            faceRight = true;
        }

        else if (!_controller.isGrounded)
        {
            _animator.setAnimation("Jump");
        }

        else
        {
            _animator.setAnimation("Idle");
            if (slide)
            {
                velocity.x += velocity.x * 0.05f;
            }
            else if (RunArrows)
            {
                velocity.x += velocity.x * 3;
            }
            else if (_controller.isGrounded)
            {
                velocity.x = 0;
            }
        }
        if (RunLeft | RunRight)
        {
            RunArrows = true;
        }

        if (RunArrows)
        {
            if (RunRight)
            {
                velocity.x += RunArrowStrength;
                velocity.y += 3;
            }
            else if (RunLeft)
            {
                velocity.x -= RunArrowStrength;
                velocity.y += 3;
            }
            AirControl = false;
        }

        if (pushed)
        {
            if (RunRight)
            {
                velocity.x += RunArrowStrength;
                velocity.y += 3;
            }
            else if (RunLeft)
            {
                velocity.x -= RunArrowStrength;
                velocity.y += 3;
            }
            pushed = false;
        }

        if ((MultiInput.GetAxis("Vertical", "", name) < 0 || MultiInput.GetAxis("LeftJoystickY", "", name) > 0) && !_controller.isGrounded)
        {
            velocity.y += gravity * Time.deltaTime * 4;
        }
        velocity.x *= 0.85f;

        if (!_controller.isGrounded && !doubleJumped)
        {
            doubleJump = true;
        }

        if (_controller.isGrounded)
        {
            doubleJumped = false;
            doubleJump = false;
        }

        // Double jump
        if (!doubleJumped && doubleJump && (MultiInput.GetButtonDown("Jump", "", name) || MultiInput.GetButtonDown("A", "", name)
            || MultiInput.GetButtonDown("LeftBumper", "", name)))
        {
            velocity.y = 0;
            velocity.y = Mathf.Sqrt(2f * jumpHeight * -gravity);
            doubleJump = false;
            doubleJumped = true;
        }

        // First jump
        else if ((MultiInput.GetButtonDown("Jump", "", name) || MultiInput.GetButtonDown("A", "", name) 
            || MultiInput.GetButtonDown("LeftBumper", "", name)) && _controller.isGrounded)
        {
            velocity.y = Mathf.Sqrt(2f * jumpHeight * -gravity);
            doubleJump = true;
            doubleJumped = false;
        }

        _controller.move(velocity * Time.deltaTime);
        RunArrows = false;

        if (_controller.isGrounded)
        {
            AirControl = true;
        }


        float inputX = MultiInput.GetAxis("Horizontal", "", name);
        inputX = MultiInput.GetAxis("LeftJoystickX", "", name);
        float inputY = MultiInput.GetAxis("Vertical", "", name);
        inputY = MultiInput.GetAxis("LeftJoystickY", "", name);

        float aimX = MultiInput.GetAxis("RightJoystickX", "", name);
        float aimY = MultiInput.GetAxis("RightJoystickY", "", name);

        //bool shoot = Input.GetButtonDown("Shoot_P1");
        //bool shoot = MultiInput.GetButtonDown("Shoot", "", name);
        float shoot = MultiInput.GetAxis("RightTrigger", "", name);
        newTrigger = shoot > 0f;
        //bool grab = MultiInput.GetButtonDown("Grab","",name);
        float grab = MultiInput.GetAxis("LeftTrigger", "", name);

        if (shoot > 0f && fireCooldown <= 0)
        {
            fireCooldown = fireRate;
            if (weapon != null && weapon.CanAttack)
            {
                if (weapon.shotType.Equals("air") && airCooldown <= 0)
                {
                    airCooldown = airFireRate;
                    if (aimX != 0 || aimY != 0) weapon.Attack(aimX, aimY);
                    else weapon.Attack(1, 0);
                    
                    weapon = null;
                }
                else
                {
                    if (aimX != 0 || aimY != 0) weapon.Attack(aimX, aimY);
                    else weapon.Attack(1, 0);
                    if (weapons.Count > 0)
                    {
                        Debug.Log("weapon pop");
                        weapon = (PlayerWeaponScript)weapons.Pop();
                    }
                    else weapon = null;
                }
            }
        }
        if (grab > 0)
        {
            if (inFountain)
            {
                if (weapon != null && weapon.shotType.Equals("air"))
                {
                    Destroy(weapon.gameObject);
                }
                fountain.CreateShot(this);

            }
            else if (nearRock)
            {
                if (weapon != null && weapon.shotType.Equals("air"))
                {
                    Destroy(weapon.gameObject);
                }
                while (weapons.Count > 0)
                {
                    weapon = (PlayerWeaponScript)weapons.Pop();
                    Destroy(weapon.gameObject);
                }
                weapon = rock.gameObject.GetComponent<PlayerWeaponScript>();
                weapon.rockScript.caster = this;
                weapon.caster = this;
                weapon.gameObject.GetComponent<Rigidbody2D>().gravityScale = 0;
            }

        }


    }

    void Flip()
    {
        // Switch the way the player is labelled as facing.

        // Multiply the player's x local scale by -1.
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }
    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.tag == "KillZ")
        {
            health.hp = 0;
            audio = gameObject.GetComponent<AudioSource>();
            try
            {
                Destroy(weapon);
            }
            catch (MissingReferenceException e) { }
            audio.PlayOneShot(DeathScreamWhenYouDie, 1f);
            fallDeath = true;
            Destroy(gameObject, 2);
        }
        else if (collider.tag == "Bounce")
        {
            Bounce = true;
        }
        else if (collider.tag == "IceBlock")
        {
            slide = true;
        }
        else if (collider.tag == "RunRight")
        {
            RunRight = true;
        }
        else if (collider.tag == "RunLeft")
        {
            RunLeft = true;
        }
    }

    void OnTriggerStay2D(Collider2D col)
    {
        if (col.tag == "Bounce")
        {
            Bounce = true;
        }

        if (col.tag == "IceBlock")
        {
            slide = true;
        }
        if (col.tag == "RunRight")
        {
            RunRight = true;
        }
        if (col.tag == "RunLeft")
        {
            RunLeft = true;
        }
    }
    void OnTriggerExit2D(Collider2D col)
    {
        if (slide)
        {
            slide = false;
        }
        if (RunLeft)
        {
            RunLeft = false;
        }
        if (RunRight)
        {
            RunRight = false;
        }
        if (RunArrows)
        {
            RunArrows = false;
        }
    }
}