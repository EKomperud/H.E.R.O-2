using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class NState {

    protected NPlayerController player;
    protected Rigidbody2D rb;
    protected BoxCollider2D bc;
    protected Animator ac;
    protected SpriteRenderer sr;
    protected NStateSO bd;
    protected float directionSwitchRatio;
    protected float maxLateralSpeed;
    protected float maxVerticalSpeed;
    protected float gravityPerFrame;
    protected EState state;
    protected bool[] validTransitions;

    // Joystick & Controls
    protected Player joystick;
    protected Vector2 leftStick;
    protected bool jumpButton;

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
        jumpButton = false;

        directionSwitchRatio = bd.GetDirectionSwitchRatio(state);
        maxLateralSpeed = bd.GetMaxLateralSpeed(state);
        maxVerticalSpeed = bd.GetMaxVerticalSpeed(state);
        gravityPerFrame = bd.GetGravityPerFrame(state);

        validTransitions = new bool[6] { false, false, false, false, false, false };
    }

    public virtual void EnterState()
    {

    }

    public virtual void ExitState()
    {

    }

    public bool AskValidTransition(EState newState)
    {
        return validTransitions[(int)newState];
    }

    public virtual void StateUpdate()
    {
        leftStick = new Vector2(joystick.GetAxis("Move Horizontal"),joystick.GetAxis("Move Vertical"));
        if (leftStick.x < 0)
        {
            sr.flipX = true;
        }
        else if (leftStick.x > 0)
        {
            sr.flipX = false;
        }

        jumpButton = joystick.GetButtonDown("Jump");
    }

    public virtual void StateFixedUpdate()
    {
        ac.SetFloat("speed", rb.velocity.magnitude);
    }

    protected bool GroundCheck()
    {
        double x = player.transform.position.x + bc.offset.x;
        double y = player.transform.position.y + bc.offset.y;
        float dist = (float)(bc.size.y * 0.5) + 0.05f;
        RaycastHit2D rch0 = Physics2D.Raycast(new Vector2((float)(x - bc.size.x * 0.5f + 0.05f), (float)y), Vector2.down, dist, LayerMask.GetMask("Platforms"));
        RaycastHit2D rch1 = Physics2D.Raycast(new Vector2((float)x, (float)y), Vector2.down, dist, LayerMask.GetMask("Platforms"));
        RaycastHit2D rch2 = Physics2D.Raycast(new Vector2((float)(x + bc.size.x * 0.5f - 0.05f), (float)y), Vector2.down, dist, LayerMask.GetMask("Platforms"));
        return ((rch0.collider != null) || (rch1.collider != null) || (rch2.collider != null));
    }

}
