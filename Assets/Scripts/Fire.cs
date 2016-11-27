using UnityEngine;
using System.Collections;
using System;

public class Fire : IWeapon
{
    //public PlayerController2 caster;
    public Vector2 speed;
    private Rigidbody2D rigidBody;
    private bool fired;
    private Vector2 direction;

    // Use this for initialization
    void Start () {
        speed = new Vector2(10f, 10f);
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        if (!fired)
            AtCaster();
        else
        {
            Vector2 movement = new Vector2(speed.x * direction.x * 1.5f, -(speed.y * direction.y * 3.0f));
            speed.x = (speed.x - speed.x * 0.070f);
            speed.y = (speed.y - speed.y * 0.4f);
            movement *= Time.deltaTime;
            rigidBody.MovePosition(rigidBody.position + (movement * 50) * Time.fixedDeltaTime);
        }
	}

    void OnTriggerEnter2D (Collider2D collider)
    {
        string tag = collider.gameObject.tag;
        switch (tag)
        {
            case "Player":
                PlayerController2 player = collider.gameObject.GetComponent<PlayerController2>();
                if (!player.Equals(caster))
                    player.WeaponHit("fire"); break;
            default:; break;
        }
    }

    public override void Attack(float x, float y)
    {
        direction.x = x;
        direction.y = y;
        fired = true;
    }

    private void AtCaster()
    {
        float inputX = MultiInput.GetAxis("RightJoystickX", "", caster.name);       // Determine if there is any player input
        float inputY = MultiInput.GetAxis("RightJoystickY", "", caster.name);
        float inputZ = -(Mathf.Atan2(inputY, inputX) * 57.2958f);
        Vector2 mag = new Vector2(inputX, inputY);
        if (mag.magnitude < 0.1f)
            inputX = inputY = 0;

        float X = caster.transform.position.x - gameObject.transform.position.x + inputX; // Determine the position of the weapon
        float Y = caster.transform.position.y - gameObject.transform.position.y - inputY;
        float Z = (Mathf.Atan2(Y, X) * 57.2958f);

        if ((X > 0.1 || X < -0.1) || (Y > 0.1 || Y < -0.1))                         // Move it if it ought to be moved
        {
            if (mag.magnitude >= 0.5)
                gameObject.transform.rotation = Quaternion.Euler(0, 0, inputZ);
            else
                gameObject.transform.rotation = Quaternion.Euler(0, 0, Z);

            Vector3 movement = new Vector3((speed.x / 2) * X, (speed.y / 2) * Y, 0);
            movement *= Time.deltaTime;
            gameObject.transform.Translate(movement, Space.World);
        }
    }

    //public override void Destroy()
    //{
    //    Destroy(this);
    //}
}
