using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NStateSlipped : NState {

    protected float directionSwitchRatio;
    protected float maxLateralVelocity;

    public NStateSlipped(NStateInfo info, EState state) : base(info, state)
    {
        directionSwitchRatio = info.bd.slippedDirectionSwitchRatio;
        maxLateralVelocity = info.bd.slippedMaxLateralVelocity;
    }

    public override void EnterState()
    {
        base.EnterState();
        Vector2 v = rb.velocity;
        rb.velocity = new Vector2(v.x * 0.8f, 0);
        ac.SetBool("grounded", true);
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

        else if (freshJumpButton)
            return player.StateTransition(EState.jump1);
        BottomCheck();
        bool grounded = GroundCheck();
        if (grounded && !IceCheck())
            return player.StateTransition(EState.normal);
        else if (!grounded)
            return player.StateTransition(EState.airborne);
        
        return false;
    }

    protected void PhysicsUpdate()
    {
        float x = 0;
        if (leftStick.x > 0)
        {
            x = rb.velocity.x > 0 ? rb.velocity.x : rb.velocity.x * directionSwitchRatio;
            x = Mathf.Min(x + (leftStick.x * 1.15f), maxLateralVelocity * Mathf.Abs(leftStick.x));
        }
        else if (leftStick.x < 0)
        {
            x = rb.velocity.x < 0 ? rb.velocity.x : rb.velocity.x * directionSwitchRatio;
            x = Mathf.Max(x + (leftStick.x * 1.15f), -maxLateralVelocity * Mathf.Abs(leftStick.x));
        }
        else if (GetBool("frozen"))
        {
            float sign = rb.velocity.x / Mathf.Abs(rb.velocity.x);
            x = Mathf.Abs(rb.velocity.x) > frozenSlideSpeed ? rb.velocity.x - (sign * 0.05f) : rb.velocity.x;
        }
        else
        {
            float sign = rb.velocity.x / Mathf.Abs(rb.velocity.x);
            x = Mathf.Abs(rb.velocity.x) > 1.5f ? rb.velocity.x - (sign * 0.25f) : 0f;
        }
        rb.velocity = new Vector2(x, 0);
    }
    #endregion
}
