using System.Collections;
using System.Collections.Generic;
using InControl;
using UnityEngine;

public class NStateJump2 : NState
{
    protected float directionSwitchRatio;
    protected float maxLateralVelocity;
    protected float initialVerticalVelocity;

    public NStateJump2(NStateInfo info, EState state) : base(info, state)
    {
        directionSwitchRatio = info.bd.jump2DirectionSwitchRatio;
        maxLateralVelocity = info.bd.jump2MaxLateralVelocity;
        initialVerticalVelocity = info.bd.jump2InitialVelocity;
    }

    public override void EnterState()
    {
        base.EnterState();
        Vector2 v = rb.velocity;
        rb.velocity = new Vector2(v.x, initialVerticalVelocity);
        ac.SetBool("grounded", false);
        SetBool("doubled", true);
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
        if (TransitionChecks())
            return;

        PhysicsUpdate();
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

        BottomCheck();
        if (GroundCheck())
        {
            if (IceCheck())
                return player.StateTransition(EState.slipped);
            return player.StateTransition(EState.normal);
        }
        else if (slamButton)
            return player.StateTransition(EState.slam);
        return false;
    }

    protected void PhysicsUpdate()
    {
        float x = 0;
        float y = rb.velocity.y - globalGravityPerFrame;
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
        else if (GetBool("frozen"))
        {
            x = rb.velocity.x;
            y = rb.velocity.y - frozenGravityPerFrame;
        }
        else
        {
            float sign = rb.velocity.x / Mathf.Abs(rb.velocity.x);
            x = Mathf.Abs(rb.velocity.x) > 1.5f ? rb.velocity.x - (sign * 1.05f) : 0f;
        }

        rb.velocity = new Vector2(x, y);
    }
    #endregion
}
