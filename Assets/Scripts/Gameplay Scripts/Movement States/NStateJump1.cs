using System.Collections;
using System.Collections.Generic;
using InControl;
using UnityEngine;

public class NStateJump1 : NState {

    protected float directionSwitchRatio;
    protected float maxLateralVelocity;
    protected float initialVerticalVelocity;
    protected float continuedHoldVelocity;
    protected int _continuedHoldFrames;
    protected int continuedHoldFrames;
    protected int _continuedHoldDelayFrames;
    protected int continuedHoldDelayFrames;

    protected int _shortHopFrames;
    protected int shortHopFrames;
    protected float shortHopVelocity;
    protected float regularHopVelocity;
    protected bool jumped;

    private float doubleJumpDelay;

    public NStateJump1(NStateInfo info, EState state) : base(info, state)
    {
        directionSwitchRatio = info.bd.jump1DirectionSwitchRatio;
        maxLateralVelocity = info.bd.jump1MaxLateralVelocity;
        initialVerticalVelocity = info.bd.jump1InitialVelocity;
        continuedHoldVelocity = info.bd.jump1ContinuedHoldVelocity;
        _continuedHoldFrames = info.bd.jump1ContinuedHoldFrames;
        _continuedHoldDelayFrames = info.bd.jump1ContinuedHoldDelayFrames;

        _shortHopFrames = info.bd.jump1ShortHopFrames;
        shortHopVelocity = info.bd.jump1ShortHopVelocity;
        regularHopVelocity = info.bd.jump1RegularHopVelocity;
    }

    public override void EnterState()
    {
        base.EnterState();
        Vector2 v = rb.velocity;
        doubleJumpDelay = 0.075f;
        ac.SetBool("grounded", false);
        shortHopFrames = 0;
        jumped = false;
        player.TrySpawnAir();
    }

    public override void ExitState()
    {
        base.ExitState();

    }

    public override void StateUpdate()
    {
        base.StateUpdate();
        doubleJumpDelay -= Time.deltaTime;
    }

    public override void StateFixedUpdate()
    {
        if (TransitionChecks())
            return;

        PhysicsUpdate();

        if (heldJumpButton)
            shortHopFrames++;
        else
            shortHopFrames = _shortHopFrames + 1;
        base.StateFixedUpdate();
    }

    #region Helpers
    protected bool TransitionChecks()
    {
        if (GetBool("succed"))
            return player.StateTransition(EState.succ);
        else if (GetBool("ashed"))
            return player.StateTransition(EState.ashes);
        else if (GetBool("spiked"))
            return player.StateTransition(EState.spiked);
        else if (GetBool("pushed"))
            return player.StateTransition(EState.pushed);
        else if (GetBool("bounced"))
            return player.StateTransition(EState.bounced);
        else if (freshJumpButton && doubleJumpDelay <= 0)
            return player.StateTransition(EState.jump2);

        BottomCheck();
        if ((GroundCheck() || HeadCheck()) && jumped)
        {
            if (IceCheck())
                return player.StateTransition(EState.slipped);
            return player.StateTransition(EState.normal);
        }
        else if (slamButton && jumped)
            return player.StateTransition(EState.slam);
        return false;
    }

    protected void PhysicsUpdate()
    {
        float x = 0;
        float y = rb.velocity.y - globalGravityPerFrame;
        if (!GetBool("frozen"))
        {
            if (leftStick.x > 0)
            {
                x = rb.velocity.x > 0 ? rb.velocity.x : rb.velocity.x * directionSwitchRatio;
                x = Mathf.Min(x + (leftStick.x * 1.5f), maxLateralVelocity * Mathf.Abs(leftStick.x));
            }
            else if (leftStick.x < 0)
            {
                x = rb.velocity.x < 0 ? rb.velocity.x : rb.velocity.x * directionSwitchRatio;
                x = Mathf.Max(x + (leftStick.x * 1.5f), -maxLateralVelocity * Mathf.Abs(leftStick.x));
            }
            else
            {
                float sign = rb.velocity.x / Mathf.Abs(rb.velocity.x);
                x = Mathf.Abs(rb.velocity.x) > 1.5f ? rb.velocity.x - (sign * 1.05f) : 0f;
            }

            if (!jumped && !heldJumpButton && shortHopFrames <= _shortHopFrames)
            {
                y = shortHopVelocity;
                jumped = true;
            }
            else if (!jumped && shortHopFrames > _shortHopFrames)
            {
                y = regularHopVelocity;
                jumped = true;
            }
        }
        else if (GetBool("frozen"))
        {
            x = rb.velocity.x;
            y = rb.velocity.y - frozenGravityPerFrame;
        }
        
        rb.velocity = new Vector2(x, y);
    }
    #endregion
}
