using UnityEngine;
using System.Collections;

public class PlayerController2 : MonoBehaviour
{

    // Boolean controls
    public bool dead;
    public bool doubleJumpAvailable;
    public bool faceRight;
    public bool inFountain;
    private bool grounded;
    private bool holdingWeapon;

    // Cooldowns. Rate is the time cap. CD is the timer
    public float airRate = 5;       //air
    private float airCD = 0;
    public float burnRate = 5;      //burning
    private float burnCD = 0;
    public float freezeRate = 5;    //frozen
    private float freezeCD = 0;
    public float fireRate = 1;      //fire rate
    private float fireCD = 0;

    // Character objects
    public Stack weapons;
    public IWeapon weapon;
    public Transform defaultWeapon;
    private BoxCollider2D hitbox;
    private Rigidbody2D rigidBody;
    public string name;
    public string character;
    public LayerMask raycastingLayers;
    public FountainScript fountain;

    // Misc
    private float lastXinput;

    /**
     * Internal operation methods
     **/

    public PlayerController2 (string name, string character)
    {
        this.name = name;
        this.character = character;
    }

    void Start()
    {
        weapons = new Stack();
        var shot = Instantiate(defaultWeapon, this.transform.position, Quaternion.identity) as Transform;
        weapon = shot.gameObject.GetComponent<IWeapon>();
        weapon.caster = this;
        hitbox = gameObject.GetComponent<BoxCollider2D>();
        rigidBody = gameObject.GetComponent<Rigidbody2D>();
        //raycastingLayers = gameObject.GetComponent<LayerMask>();
    }

    void Update()
    {
        if (!dead)
        {
            // decrement cooldowns if cooldowns > 0
            Cooldowns(airCD > 0, burnCD > 0, freezeCD > 0, fireCD > 0);

            // get all inputs
            float inputX = MultiInput.GetAxis("LeftJoystickX", "", name);
            float aimX = MultiInput.GetAxis("RightJoystickX", "", name);
            float aimY = MultiInput.GetAxis("RightJoystickY", "", name);
            bool jump = MultiInput.GetButtonDown("A", "", name);
            bool groundPound = MultiInput.GetButton("LeftBumper", "", name);
            bool grab = MultiInput.GetButtonDown("X", "", name);
            bool shoot = MultiInput.GetButtonDown("RightBumper", "", name);

            LateralMovement(inputX);
            VerticalMovement(jump, groundPound);
            WeaponUse(grab, shoot);
        }
        else
        {

        }
    }

    /// <summary>
    /// Handles all lateral movement
    /// </summary>
    private void LateralMovement(float inputX)
    {
        RaycastHit2D[] hits1 = Physics2D.RaycastAll(new Vector3(transform.position.x + hitbox.size.x / 2, transform.position.y), Vector2.down, (float)((hitbox.size.y / 2) + 0.5));
        RaycastHit2D[] hits2 = Physics2D.RaycastAll(new Vector3(transform.position.x - hitbox.size.x / 2, transform.position.y), Vector2.down, (float)((hitbox.size.y / 2) + 0.5));
        RaycastHit2D[] rightHits = Physics2D.RaycastAll(new Vector3(transform.position.x, transform.position.y), Vector2.right, (float)((hitbox.size.x / 2) + 0.5));
        RaycastHit2D[] leftHits = Physics2D.RaycastAll(new Vector3(transform.position.x, transform.position.y), Vector2.left, (float)((hitbox.size.x / 2) + 0.5));
        ArrayList verticalHits = new ArrayList(hits1);
        verticalHits.AddRange(hits2);
        ArrayList lateralHits = new ArrayList(rightHits);
        lateralHits.AddRange(leftHits);
        if (verticalHits.Count != 0) // grounded
        {
            if (inputX > 0.05)
            {
                transform.localScale = new Vector2((float)1.25, transform.localScale.y);
                //if (rigidBody.velocity.x < 7)
                //    rigidBody.AddForce(new Vector2(inputX, 0), ForceMode2D.Impulse);
                rigidBody.velocity = new Vector2(inputX * 10, rigidBody.velocity.y);
            }
            else if (inputX < -0.05)
            {
                transform.localScale = new Vector2((float)-1.25, transform.localScale.y);
                //if (rigidBody.velocity.x > -7)
                //    rigidBody.AddForce(new Vector2(inputX, 0), ForceMode2D.Impulse);
                rigidBody.velocity = new Vector2(inputX * 10, rigidBody.velocity.y);
            }
            else
            {
                rigidBody.velocity = new Vector2(rigidBody.velocity.x * (float)0.9, rigidBody.velocity.y);
            }
        }
        else if (lateralHits.Count == 0)  // airborne
        {
            if (inputX > 0.05)
            {
                transform.localScale = new Vector2((float)1.25, transform.localScale.y);
                float X = rigidBody.velocity.x;
                //if (rigidBody.velocity.x < 3f)
                if (X < 3f)
                    rigidBody.AddForce(new Vector2(inputX, 0), ForceMode2D.Impulse);
                //rigidBody.velocity = new Vector2(inputX * 10, rigidBody.velocity.y);
            }
            else if (inputX < -0.05)
            {
                transform.localScale = new Vector2((float)-1.25, transform.localScale.y);
                if (rigidBody.velocity.x > -3f)
                    rigidBody.AddForce(new Vector2(inputX, 0), ForceMode2D.Impulse);
                //rigidBody.velocity = new Vector2(inputX * 10, rigidBody.velocity.y);
            }
            else
            {
                rigidBody.velocity = new Vector2(rigidBody.velocity.x * (float)0.9, rigidBody.velocity.y);
            }
        }
    }

    /// <summary>
    /// Handles all vertical movement
    /// </summary>
    private void VerticalMovement(bool jump, bool groundPound)
    {
        rigidBody.AddForce(Physics2D.gravity);
        if (jump)
        {
            RaycastHit2D[] hits1 = Physics2D.RaycastAll(new Vector3(transform.position.x + hitbox.size.x / 2, transform.position.y), Vector2.down, (float)((hitbox.size.y / 2) + 0.2));
            RaycastHit2D[] hits2 = Physics2D.RaycastAll(new Vector3(transform.position.x - hitbox.size.x / 2, transform.position.y), Vector2.down, (float)((hitbox.size.y / 2) + 0.2));
            RaycastHit2D[] rightHits = Physics2D.RaycastAll(new Vector3(transform.position.x, transform.position.y), Vector2.right, (float)((hitbox.size.x / 2) + 0.2));
            RaycastHit2D[] leftHits = Physics2D.RaycastAll(new Vector3(transform.position.x, transform.position.y), Vector2.left, (float)((hitbox.size.x / 2) + 0.2));
            ArrayList hitsList = new ArrayList(hits1);
            hitsList.AddRange(hits2);
            //hitsList.AddRange(rightHits);
            //hitsList.AddRange(leftHits);
            if (hitsList.Count != 0)
            {
                foreach (RaycastHit2D h in hitsList)
                    if (h.collider.gameObject.tag.Equals("Terrain"))
                    {
                        rigidBody.AddForce(new Vector2(0, 15), ForceMode2D.Impulse);
                        break;
                    }
            }
            if (rightHits.Length != 0 || leftHits.Length != 0)
            {
                if (rightHits.Length != 0)
                {
                    foreach (RaycastHit2D h in rightHits)
                        if (h.collider.gameObject.tag.Equals("Terrain"))
                        {
                            rigidBody.AddForce(new Vector2(-10, 15), ForceMode2D.Impulse);
                            break;
                        }
                }
                if (leftHits.Length != 0)
                {
                    foreach (RaycastHit2D h in leftHits)
                        if (h.collider.gameObject.tag.Equals("Terrain"))
                        {
                            rigidBody.AddForce(new Vector2(10, 15), ForceMode2D.Impulse);
                            break;
                        }
                }
            }
        }
        if (groundPound)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, (float)((hitbox.size.y / 2)+0.5));
            if (hit.collider != null) { }
            else
                rigidBody.AddForce(new Vector2(0, -1), ForceMode2D.Impulse);
        }
    }

    /// <summary>
    /// Handles player cooldowns. 
    /// Inputs all cooldowns and decrements them if they are greater than 0. 
    /// </summary>
    private void Cooldowns(bool air, bool burn, bool frozen, bool firing)
    {
        if (air)    
            airCD -= Time.deltaTime;
        if (airCD <= 0)     // Spawn air if its CD is up and this player has no weapon
        {
            if (weapon != null) { }
            else
            {
                var shot = Instantiate(defaultWeapon, this.transform.position, Quaternion.identity) as Transform;
                weapon = shot.gameObject.GetComponent<IWeapon>();
                weapon.caster = this;
            }
        }

        if (burn)
        {
            burnCD -= Time.deltaTime;
            if (burnCD <= 0)
            {
                // Player disintegrates
            }
        }
        if (frozen)
            freezeCD -= Time.deltaTime;
        if (firing)
            fireCD -= Time.deltaTime;
    }

    /// <summary>
    /// Handles all weapon use
    /// </summary>
    private void WeaponUse(bool grab, bool shoot)
    {
        if (grab && !holdingWeapon)
        {
            Destroy(weapon);
            fountain.CreateShot(this);
        }
        if (shoot && holdingWeapon)
        {

        }
    }

    void OnTriggerStay2D(Collider2D collider)
    {
        //if (collider.tag.Equals("terrain") && rigidBody.velocity.y == 0)
        //{
        //    grounded = true;
        //}
    }

    /**
     * Public methods
     * */
    
    /// <summary>
    /// Hit this player with a weapon
    /// </summary>
    public void WeaponHit(string type)
    {
        switch (type)
        {
            case "fire":; break;
            case "water":; break;
            case "air":; break;
            case "rock":; break;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.right * transform.localScale.x * (float)((hitbox.size.y / 2) + 0.1));
    }
}
