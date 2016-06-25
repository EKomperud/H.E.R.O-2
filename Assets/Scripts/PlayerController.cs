using UnityEngine;
using System.Collections;
using Prime31;

public class PlayerController : MonoBehaviour
{

    public float walkSpeed = 3;
    public float gravity = -35;
    public float jumpHeight = 2;
    public float maxHeight = 20;

    private CharacterController2D _controller;
    private AnimationController2D _animator;
    private AudioSource audio;
    public AudioClip DeathScreamWhenYouDie;
    public ArrayList fountains = new ArrayList();

    public FountainScript fountain;

    public bool inFountain = false;
    public PlayerWeaponScript weapon;
    private bool holdingWeapon = false;
    public Vector2 speed = new Vector2(50, 50);
    public string name;
    public bool faceRight = false;
    private bool Bounce = false;
    private bool doubleJump = false;            // Allows the player to double jump
    private bool doubleJumped = false;          // Records if the player has double jumped
    private bool slide = false;

    // Use this for initialization
    void Start()
    {

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

        if (Input.GetAxis("Horizontal_P1") < 0 || Input.GetAxis("LeftJoystickX") < 0)
        {

            if (slide)
            {
                velocity.x += -walkSpeed * velocity.x;
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

        else if (Input.GetAxis("Horizontal_P1") > 0 || Input.GetAxis("LeftJoystickX") > 0)
        {
            if (slide)
            {
                velocity.x += walkSpeed * velocity.x;
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
            else
            {
                velocity.x = 0;
            }
        }

        if ((Input.GetAxis("Vertical_P1") < 0 || Input.GetAxis("LeftJoystickY") > 0) && !_controller.isGrounded)
        {
            velocity.y += gravity * Time.deltaTime * 4;
        }

        if (weapon == null)
            holdingWeapon = false;
        float inputX = Input.GetAxis("Horizontal_P1");
        inputX = Input.GetAxis("LeftJoystickX");
        float inputY = Input.GetAxis("Vertical_P1");
        inputY = Input.GetAxis("LeftJoystickY");

        bool shoot = Input.GetButtonDown("Shoot_P1");
        shoot = Input.GetButtonDown("X");
        bool grab = Input.GetButtonDown("Grab_P1");
        grab = Input.GetButtonDown("Y");

        if (shoot)
        {
            if (weapon != null && weapon.CanAttack)
            {
                holdingWeapon = false;
                weapon.Attack();
            }
        }
        if (grab && inFountain && !holdingWeapon)
        {
            //FountainScript fountain = (FountainScript) fountains [0];
            fountain.CreateShot(this);
            holdingWeapon = true;
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
        if (!doubleJumped && doubleJump && (Input.GetButtonDown("Jump_P1") || Input.GetButtonDown("A")))
        {
            velocity.y = 0;
            velocity.y = Mathf.Sqrt(2f * jumpHeight * -gravity);
            doubleJump = false;
            doubleJumped = true;
        }

        // First jump
        else if ((Input.GetButtonDown("Jump_P1") || Input.GetButtonDown("A")) && _controller.isGrounded)
        {
            velocity.y = Mathf.Sqrt(2f * jumpHeight * -gravity);
            doubleJump = true;
            doubleJumped = false;
        }

        _controller.move(velocity * Time.deltaTime);
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
        else
        {
            slide = false;
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
        else
        {
            slide = false;
        }
    }
}