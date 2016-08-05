using UnityEngine;
using System.Collections;
using Prime31;
public class PlayerController : MonoBehaviour
{

    // Balance numbers
    public float burnTime = 3f;
    public float burnDown;
    public float freezeTime = 4f;
    public float freezeWarmup;
    public float airFireRate = 2f;
    private float airCooldown;
    public float fireRate = 0.3f;
    private float fireCooldown;
    public float walkSpeed = 3;
    public float airSpeed = 6;
    public float gravity = -35;
    public float jumpHeight = 2;
    public float maxHeight = 20;
    public float RunArrowStrength = 75;
    public float AirPushVelocity { get; set; }

    // Character objects
    private CharacterController2D _controller;
    private AnimationController2D _animator;
	private SpriteRenderer colorChange;
    private AudioSource audioPlayer;
    private Sounds sounds;
    public AudioClip DeathScreamWhenYouDie;
    private AudioSource audio;
    public Transform defaultShot;
    public HealthScript health;
    public FountainScript fountain;
    public PlayerWeaponScript weapon;
    public RockScript rock;
    public RockCollider rockCollider;
    public Stack weapons;
    public Vector2 speed = new Vector2(50, 50);

    // Operation booleans
    public bool dead = false;
    public bool burning = false;
    public bool nearRock = false;
    public bool inFountain = false;
    private bool plasmaDeath = false;
    public string name;
    public string character;
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
	private bool colorTimer = false;
	private float colorCountNumber = 0;
    public bool frozen = false;

    // Use this for initialization
    void Start()
    {
        airCooldown = 0;
        health = gameObject.GetComponent<HealthScript>();
        _controller = gameObject.GetComponent<CharacterController2D>();
        _animator = gameObject.GetComponent<AnimationController2D>();
		colorChange = gameObject.GetComponent<SpriteRenderer> ();
        weapons = new Stack();
        audioPlayer = GetComponent<AudioSource>();
        sounds = GetComponent<Sounds>();
    }

    void Awake()
    {
        var shot = Instantiate(defaultShot) as Transform;
        shot.position = transform.position;
        PlayerWeaponScript shotScript = shot.gameObject.GetComponent<PlayerWeaponScript>();
        weapon = shotScript;
        shotScript.caster = this;
        shotScript.MoveToCaster();
        frozen = false;
    }

    // Update is called once per frame
    void Update()
    {

        if (!dead)
        {
            if (colorChange.color == new Color(150, 0, 0))
            {
                if (colorTimer)
                {
                    colorCountNumber = 0;
                    colorTimer = false;
                }
                colorCountNumber += Time.deltaTime;
                if (colorCountNumber > 10)
                {
                    colorChange.color = new Color(255, 255, 255);
                    Debug.Log("here");
                }
            }
            //Debug.Log("AirPushVelocity = " + AirPushVelocity);
            if (airCooldown > 0 && weapon == null)
            {
                airCooldown -= Time.deltaTime;
            }
            if (airCooldown <= 0 && weapon == null && !(weapons.Count > 0))
            {
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

            if (burning)
            {
                burnDown -= Time.deltaTime;
                if (burnDown <= 0f)
                    health.ManualDamage(health.hp, "fire");
            }

            Vector3 velocity = _controller.velocity;
            if (_controller.isGrounded && _controller.ground != null && _controller.ground.tag == "MovingPlatform")
            {
                this.transform.parent = _controller.ground.transform;
            }
            else
            {
                if (this.transform.parent != null)
                {
                    this.transform.parent = null;
                }
            }

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

            if (!frozen)
            {
                // Player is moving horizontally
                //if (MultiInput.GetAxis("Horizontal", "", name) < 0 || MultiInput.GetAxis("LeftJoystickX", "", name) < 0)
                if (MultiInput.GetAxis("LeftJoystickX", "", name) < 0)
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
                    else if (!_controller.isGrounded)
                    {
                        velocity.x = -airSpeed;
                    }
                    else
                    {
                        velocity.x = -walkSpeed;
                    }
                    if (!_controller.isGrounded)
                    {
                        _animator.setAnimation(character + " Jump");
                    }
                    else
                    {
                        _animator.setAnimation(character + " Walk");
                    }
                    _animator.setFacing("Left");
                    faceRight = false;
                }
                //else if (MultiInput.GetAxis("Horizontal", "", name) > 0 || MultiInput.GetAxis("LeftJoystickX", "", name) > 0)
                else if (MultiInput.GetAxis("LeftJoystickX", "", name) > 0)
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
                    else if (!_controller.isGrounded)
                    {
                        velocity.x = airSpeed;
                    }
                    else
                    {
                        velocity.x = walkSpeed;
                    }
                    if (!_controller.isGrounded)
                    {
                        _animator.setAnimation(character + " Jump");
                    }
                    else
                    {
                        _animator.setAnimation(character + " Walk");
                    }
                    _animator.setFacing("Right");
                    faceRight = true;
                }
                else if (!_controller.isGrounded)
                {
                    _animator.setAnimation(character + " Jump");
                }

                else
                {
                    _animator.setAnimation(character + " Idle");
                    if (slide)
                    {
                        velocity.x += velocity.x * 0.05f;
                    }
                    else if (RunArrows)
                    {
                        velocity.x += velocity.x * 3;
                    }
                    else if (pushed) velocity.x += velocity.x * 1.2f;
                    else if (_controller.isGrounded)
                    {
                        velocity.x = 0;
                    }
                }
            }
            else if (frozen)
            {
                freezeWarmup -= Time.deltaTime;
                if (freezeWarmup <= 0)
                {
                    frozen = false;
                    jumpHeight = 2;
                }
            }


            if (RunLeft || RunRight)
            {
                RunArrows = true;
            }

            if (RunArrows)
            {
                if (RunRight)
                {
                    velocity.x += RunArrowStrength;
                    velocity.y += 10;
                }
                else if (RunLeft)
                {
                    velocity.x -= RunArrowStrength;
                    velocity.y += 10;
                }
                AirControl = false;
            }

            if (pushed)
            {
                if (!faceRight)
                {
                    Debug.Log("AirPushVelocity = " + AirPushVelocity);
                    velocity.x += AirPushVelocity;
                    velocity.y += 3;
                }
                else if (faceRight)
                {
                    Debug.Log("AirPushVelocity = " + AirPushVelocity);
                    velocity.x -= AirPushVelocity;
                    velocity.y += 3;
                }
                pushed = false;
                colorChange.color = new Color(0, 0, 0);
            }

            //if ((MultiInput.GetAxis("Vertical", "", name) < 0 || MultiInput.GetAxis("LeftJoystickY", "", name) > 0) && !_controller.isGrounded)
            if (MultiInput.GetButton("LeftBumper", "", name) && !_controller.isGrounded)
            {
                velocity.y += gravity * Time.deltaTime  * 3;
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
            //if (!doubleJumped && doubleJump && (MultiInput.GetButtonDown("Jump", "", name) || MultiInput.GetButtonDown("A", "", name)
            //    || MultiInput.GetButtonDown("LeftBumper", "", name)))
            if (!doubleJumped && doubleJump && (MultiInput.GetButtonDown("A", "", name)
                || MultiInput.GetButtonDown("LeftBumper", "", name)))
            {
                velocity.y = 0;
                velocity.y = Mathf.Sqrt(2f * jumpHeight * -gravity);
                doubleJump = false;
                doubleJumped = true;
                Transform djParticle = transform.GetChild(1);
                ParticleSystem dj = djParticle.GetComponent<ParticleSystem>();
                dj.Play();
            }

            // First jump
            //else if ((MultiInput.GetButtonDown("Jump", "", name) || MultiInput.GetButtonDown("A", "", name) 
            //    || MultiInput.GetButtonDown("LeftBumper", "", name)) && _controller.isGrounded)
            else if ((MultiInput.GetButtonDown("A", "", name)
                || MultiInput.GetButtonDown("LeftBumper", "", name)) && _controller.isGrounded)
            {
                audioPlayer.PlayOneShot(sounds.Jump);
                velocity.y = Mathf.Sqrt(2f * jumpHeight * -gravity);
                doubleJump = true;
                doubleJumped = false;
            }
            pushed = false;
            colorTimer = true;


            _controller.move(velocity * Time.deltaTime);
            RunArrows = false;

            if (_controller.isGrounded)
            {
                AirControl = true;
            }


            //float inputX = MultiInput.GetAxis("Horizontal", "", name);
            float inputX = MultiInput.GetAxis("LeftJoystickX", "", name);
            //float inputY = MultiInput.GetAxis("Vertical", "", name);
            float inputY = MultiInput.GetAxis("LeftJoystickY", "", name);

            float aimX = MultiInput.GetAxis("RightJoystickX", "", name);
            float aimY = MultiInput.GetAxis("RightJoystickY", "", name);

            //bool shoot = Input.GetButtonDown("Shoot_P1");
            //bool shoot = MultiInput.GetButtonDown("Shoot", "", name);
            bool shoot = MultiInput.GetButton("RightBumper", "", name);
            //bool grab = MultiInput.GetButtonDown("Grab","",name);

            float grab = MultiInput.GetAxis("X", "", name);

            if (shoot && fireCooldown <= 0)
            {
                fireCooldown = fireRate;
                if (weapon != null && weapon.hasShot)
                {
                    if (weapon.rock)
                    {
                        weapon.gameObject.GetComponent<Rigidbody2D>().gravityScale = 6;

                    }
                    if (weapon.shotType.Equals("air") && airCooldown <= 0)
                    {
                        airCooldown = airFireRate;
                        if (aimX != 0 || aimY != 0)
                        {
                            float x = aimX * 10;
                            float y = aimY * 10;
                            float z = -(Mathf.Atan2(y, x) * 57.2958f);
                            if (aimX < 0)
                                weapon.gameObject.transform.Rotate(180, 0, -z, Space.Self);
                            else
                                weapon.gameObject.transform.Rotate(0, 0, z, Space.Self);
                            weapon.Attack(aimX, aimY);
                        }
                        else
                        {
                            if (faceRight)
                            {
                                weapon.Attack(1, 0);
                            }
                            else
                            {
                                weapon.gameObject.transform.Rotate(180, 0, 180, Space.Self);
                                weapon.Attack(-1, 0);
                            }
                        }
                        weapon = null;
                    }
                    else
                    {
                        if (aimX != 0 || aimY != 0)
                        {
                            float x = aimX * 10;
                            float y = aimY * 10;
                            float z = -(Mathf.Atan2(y, x) * 57.2958f);
                            weapon.Attack(aimX, aimY);
                            Debug.Log("this is happening");
                        }
                        else
                        {
                            if (faceRight)
                                weapon.Attack(1, 0);
                            else
                                weapon.Attack(-1, 0);
                        }
                        // Post weapon firing
                        if (weapons.Count > 0)
                        {
                            weapon = (PlayerWeaponScript)weapons.Pop();
                        }
                        else weapon = null;
                    }
                }
                airCooldown += 0.75f;
            }
            if (grab > 0)
            {
                if (nearRock)
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
                    weapon = rock.gameObject.GetComponentInParent<PlayerWeaponScript>();
                    //weapon.rockScript.player = this;
                    weapon.caster = this;
                    weapon.gameObject.GetComponent<Rigidbody2D>().gravityScale = 0;
                }

                else if (inFountain)
                {
                    if (weapon != null && weapon.shotType.Equals("air") && weapons.Count == 0)
                    {
                        Destroy(weapon.gameObject);
                        fountain.CreateShot(this);
                    }
                    else if (weapon == null)
                    {
                        fountain.CreateShot(this);
                        if (weapon.shotType.Equals("water"))
                        {
                            burning = false;

                            Transform burnParticles = transform.GetChild(0);
                            ParticleSystem nbaJamOnFireEdition = burnParticles.GetComponent<ParticleSystem>();
                            nbaJamOnFireEdition.Stop();
                        }
                    }
                }
            }
        }
        else
        {
            if (plasmaDeath)
            {
                transform.Rotate(new Vector3(0, 0, 10));
                transform.localScale = new Vector3(transform.localScale.x * 0.95f, transform.localScale.y * 0.95f, 0f);
            }
        }
    }

    public void DeathAnim(string style)
    {
        if (style.Equals("fire"))
        {
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            rb.isKinematic = false;
            rb.gravityScale *= 2;
            rb.AddForce(new Vector2(0, 7.5f), ForceMode2D.Impulse);
            Transform burnParticles = transform.GetChild(0);
            ParticleSystem nbaJamOnFireEdition = burnParticles.GetComponent<ParticleSystem>();
            nbaJamOnFireEdition.Stop();
            colorChange.color = new Color(0, 0, 0);
        }
        else if (style.Equals("Spikes"))
        {
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            rb.isKinematic = false;
            rb.gravityScale *= 2;
            rb.AddForce(new Vector2(0, 7.5f), ForceMode2D.Impulse);
            audioPlayer.PlayOneShot(sounds.SpikeDeath);
        }
        else if (style.Equals("plasma"))
        {
            plasmaDeath = true;
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
        PlayerController player = collider.GetComponent<PlayerController>();
        if (player!=null && burning)
        {
            player.burning = true;
            player.burnDown = player.burnTime;
            Transform burnParticles = player.transform.GetChild(0);
            ParticleSystem nbaJamOnFireEdition = burnParticles.GetComponent<ParticleSystem>();
            nbaJamOnFireEdition.Play();
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