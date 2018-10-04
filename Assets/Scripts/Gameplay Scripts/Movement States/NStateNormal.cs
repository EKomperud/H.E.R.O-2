using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NStateNormal : NState {

    protected float directionSwitchRatio;
    protected float maxLateralVelocity;
    protected MovingPlatform movingPlatform;

    public NStateNormal(NStateInfo info, EState state) : base(info, state)
    {
        directionSwitchRatio = info.bd.normalDirectionSwitchRatio;
        maxLateralVelocity = info.bd.normalMaxLateralVelocity;
    }

    public override void EnterState()
    {
        base.EnterState();
        Vector2 v = rb.velocity;
        rb.velocity = new Vector2(v.x * 0.8f, v.y);
        //player.SetAnimatorBools("grounded", true);
        SetBool("doubled", false);
    }

    public override void ExitState()
    {
        base.ExitState();
    }

    public override void StateUpdate()
    {
        base.StateUpdate();
    }

    public override void StateFixedUpdate()
    {

        PhysicsUpdate();

        if (TransitionChecks())
            return;

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
        else if (GetBool("dodged"))
            return player.StateTransition(EState.airDodge);
        else if (GetBool("boosted"))
            return player.StateTransition(EState.suspended);
        else if (GetBool("pushed"))
            return player.StateTransition(EState.pushed);
        else if (GetBool("bounced"))
            return player.StateTransition(EState.bounced);
        else if (freshJumpButton)
            return player.StateTransition(EState.jump1);

        BottomCheck();
        if (!GroundCheck())
            return player.StateTransition(EState.airborne);
        else if (IceCheck())
            return player.StateTransition(EState.slipped);
        
        return false;
    }

    protected void PhysicsUpdate()
    {
        float x = 0;
        if (leftStick.x > 0)
        {
            x = rb.velocity.x > 0 ? rb.velocity.x : rb.velocity.x * directionSwitchRatio;
            x = Mathf.Min(x + (leftStick.x * 2.5f), maxLateralVelocity * Mathf.Abs(leftStick.x));
        }
        else if (leftStick.x < 0)
        {
            x = rb.velocity.x < 0 ? rb.velocity.x : rb.velocity.x * directionSwitchRatio;
            x = Mathf.Max(x + (leftStick.x * 2.5f), -maxLateralVelocity * Mathf.Abs(leftStick.x));
        }
        else if (GetBool("frozen"))
        {
            float sign = rb.velocity.x / Mathf.Abs(rb.velocity.x);
            x = Mathf.Abs(rb.velocity.x) > frozenSlideSpeed ? rb.velocity.x - (sign * 0.5f) : rb.velocity.x;
        }
        else
        {
            float sign = rb.velocity.x / Mathf.Abs(rb.velocity.x);
            x = Mathf.Abs(rb.velocity.x) > 1.5f ? rb.velocity.x - (sign * 1.2f) : 0f;
        }
        rb.velocity = new Vector2(x, -globalGravityPerFrame);

        movingPlatform = MovingPlatformCheck();
        if (movingPlatform != null)
        {
            rb.MovePosition(rb.position + movingPlatform.GetLastFrameMovement() + (rb.velocity * Time.fixedDeltaTime));
        }
    }
    #endregion
}
