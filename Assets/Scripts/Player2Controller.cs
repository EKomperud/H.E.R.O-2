﻿using UnityEngine;
using System.Collections;
using Prime31;

public class Player2Controller : MonoBehaviour
{

    public float walkSpeed = 3;
    public float gravity = -35;
    public float jumpHeight = 2;
    public float maxHeight = 20;
    public float RunArrowStrength = 75;

    private CharacterController2D _controller;
    private AnimationController2D _animator;
    private AudioSource audio;
    public AudioClip DeathScreamWhenYouDie;
    public ArrayList fountains = new ArrayList();

    public Health2Script health;
    public Fountain2Script fountain;
    public bool inFountain = false;
    public RockScript rock;
    public bool nearRock = false;
    public Player2WeaponScript weapon;
    private bool holdingWeapon = false;
    public Vector2 speed = new Vector2(50, 50);
    public string name;
    public bool faceRight = false;
    private bool Bounce = false;
    private bool doubleJump = false;            // Allows the player to double jump
    private bool doubleJumped = false;          // Records if the player has double jumped
    private bool slide = false;
    private bool RunRight = false;
    private bool RunLeft = false;
    private bool RunArrows = false;
    private bool AirControl = true;

    // Use this for initialization
    void Start()
    {
        health = gameObject.GetComponent<Health2Script>();
        _controller = gameObject.GetComponent<CharacterController2D>();
        _animator = gameObject.GetComponent<AnimationController2D>();
    }

    void Awake()
    {

        //weapons = GetComponentsInChildren<WeaponScript> ();
    }

    // Update is called once per frame
    void Update()
    {

        Vector3 velocity = _controller.velocity;

        if (!Bounce)
        {
            velocity.y += gravity * Time.deltaTime;
        }

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

        if (Input.GetAxis("Horizontal_P2") < 0 || Input.GetAxis("LeftJoystickX_P2") < 0)
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

        else if (Input.GetAxis("Horizontal_P2") > 0 || Input.GetAxis("LeftJoystickX_P2") > 0)
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

        if ((Input.GetAxis("Vertical_P2") < 0 || Input.GetAxis("LeftJoystickY_P2") > 0) && !_controller.isGrounded)
        {
            velocity.y += gravity * Time.deltaTime * 4;
        }

        if (weapon == null)
            holdingWeapon = false;
        float inputX = Input.GetAxis("Horizontal_P2");
        inputX = Input.GetAxis("LeftJoystickX_P2");
        float inputY = Input.GetAxis("Vertical_P2");
        inputY = Input.GetAxis("LeftJoystickY_P2");

        bool shoot = Input.GetButtonDown("Shoot_P2");
        shoot = Input.GetButtonDown("X_P2");
        bool grab = Input.GetButtonDown("Grab_P2");
        grab = Input.GetButtonDown("Y_P2");

        if (shoot)
        {
            if (weapon != null && weapon.CanAttack)
            {
                holdingWeapon = false;
                weapon.Attack();
            }
        }
        if (grab && !holdingWeapon)
        {
            if (inFountain)
            {
                fountain.CreateShot(this);
                holdingWeapon = true;
            }
            else if (nearRock)
            {
                weapon = rock.gameObject.GetComponent<Player2WeaponScript>();
                holdingWeapon = true;
                weapon.caster = this;
            }

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
        if (!doubleJumped && doubleJump && (Input.GetButtonDown("Jump_P2") || Input.GetButtonDown("A_P2")))
        {
            velocity.y = 0;
            velocity.y = Mathf.Sqrt(2f * jumpHeight * -gravity);
            doubleJump = false;
            doubleJumped = true;
        }

        // First jump
        else if ((Input.GetButtonDown("Jump_P2") || Input.GetButtonDown("A_P2")) && _controller.isGrounded)
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