using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class NState {

    // References
    protected NPlayerController player;
    protected Rigidbody2D rb;
    protected BoxCollider2D bc;
    protected Animator ac;
    protected SpriteRenderer sr;
    protected NStateSO bd;
    protected EState state;

    // Joystick & Controls
    protected Player joystick;
    protected Vector2 leftStick;
    protected bool freshJumpButton;
    protected bool heldJumpButton;
    protected bool slamButton;

    // Global movement variables
    protected float globalGravityPerFrame;
    protected float frozenSlideSpeed;
    protected float frozenGravityPerFrame;

    // Members
    protected RaycastHit2D[] hits;
    protected int platformsLayer;

    public NState(NStateInfo info, EState state)
    {
        this.state = state;
        player = info.p;
        rb = info.r;
        bc = info.b;
        bd = info.bd;
        ac = info.ac;
        sr = info.sr;
        joystick = info.j;
        freshJumpButton = false;
        heldJumpButton = false;

        globalGravityPerFrame = info.bd.globalGravityPerFrame;
        frozenSlideSpeed = info.bd.frozenSlideSpeed;
        frozenGravityPerFrame = info.bd.frozenGravityPerFrame;

        platformsLayer = LayerMask.NameToLayer("Platforms");
    }

    public virtual void EnterState()
    {
        BottomCheck();
    }

    public virtual void ExitState()
    {

    }

    public virtual void StateUpdate()
    {
        leftStick = Vector2.zero;
        freshJumpButton = false;
        if (player.GetLivingStatus())
        {
            if (!GetBool("frozen"))
            {
                leftStick = new Vector2(joystick.GetAxis("Move Horizontal"), joystick.GetAxis("Move Vertical"));
                freshJumpButton = joystick.GetButtonDown("Jump");
                heldJumpButton = joystick.GetButton("Jump");
                slamButton = joystick.GetButton("Slam");
                //bool jb = joystick.getbutt
            }
            if (leftStick.x < 0)
                sr.flipX = true;
            else if (leftStick.x > 0)
                sr.flipX = false;
        }
    }

    public virtual void StateFixedUpdate()
    {
        ac.SetFloat("speed", rb.velocity.x);
    }

    public void SetBool(string s, bool b)
    {
        player.SetMovementBool(s, b);
    }

    public bool GetBool(string s)
    {
        return player.GetMovementBool(s);
    }

    protected void BottomCheck()
    {
        double x = player.transform.position.x + bc.offset.x;
        double y = player.transform.position.y + bc.offset.y;
        float dist = (float)(bc.size.y * 0.5) + 0.05f;
        RaycastHit2D[] rch0 = new RaycastHit2D[2];
        RaycastHit2D[] rch1 = new RaycastHit2D[2];
        RaycastHit2D[] rch2 = new RaycastHit2D[2];
        Physics2D.RaycastNonAlloc(new Vector2((float)((x - bc.size.x * 0.5f) + 0.05f), (float)y), Vector2.down, rch0, dist, LayerMask.GetMask("Platforms", "Player"));
        Physics2D.RaycastNonAlloc(new Vector2((float)x, (float)y), Vector2.down, rch1, dist, LayerMask.GetMask("Platforms", "Player"));
        Physics2D.RaycastNonAlloc(new Vector2((float)((x + bc.size.x * 0.5f) - 0.05f), (float)y), Vector2.down, rch2, dist, LayerMask.GetMask("Platforms", "Player"));
        hits = new RaycastHit2D[3] { rch0[1], rch1[1], rch2[1] };
    }

    protected bool GroundCheck()
    {
        foreach (RaycastHit2D hit in hits)
            if (hit.collider != null && hit.collider.gameObject.layer.Equals(platformsLayer))
                return true;
        return false;
    }

    protected MovingPlatform MovingPlatformCheck()
    {
        foreach (RaycastHit2D hit in hits)
            if (hit.collider != null && hit.collider.gameObject.tag.Equals("MovingPlatform"))
                return hit.collider.gameObject.GetComponent<MovingPlatform>();
        return null;
    }

    protected bool IceCheck()
    {
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider != null && !hit.collider.tag.Equals("IceBlock"))
                return false;
            else if (!(hit.collider != null))
                return false;
        }
        return true;
    }

    protected NPlayerController HeadCheck()
    {
        foreach (RaycastHit2D hit in hits)
            if (hit.collider != null)
            {
                NPlayerController otherPlayer = hit.collider.gameObject.GetComponent<NPlayerController>();
                if (otherPlayer != null && otherPlayer != player && otherPlayer.GetLivingStatus())
                    return otherPlayer;
            }
        return null;
    }
}
