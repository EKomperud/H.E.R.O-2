using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;

public class NStateNormal : NState {

    public NStateNormal(NStateInfo info, EState state) : base(info, state)
    {
        validTransitions = new bool[7] { false, true, false, true, true, true, true };
    }

    public override void EnterState()
    {
        base.EnterState();
        Vector2 v = rb.velocity;
        rb.velocity = new Vector2(v.x * 0.8f, maxVerticalSpeed);
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
        if (!GroundCheck())
        {
            player.TryStateTransition(EState.airborne);
            return true;
        }
        if (jumpButton)
        {
            player.TryStateTransition(EState.jump1);
            return true;
        }
        return false;
    }

    protected void PhysicsUpdate()
    {
        float x = 0;
        if (leftStick.x > 0)
        {
            x = rb.velocity.x > 0 ? rb.velocity.x : rb.velocity.x * (directionSwitchRatio - 0.2f);
            x = Mathf.Min(x + (leftStick.x * 2.5f), maxLateralSpeed * Mathf.Abs(leftStick.x));
        }
        else if (leftStick.x < 0)
        {
            x = rb.velocity.x < 0 ? rb.velocity.x : rb.velocity.x * (directionSwitchRatio - 0.2f);
            x = Mathf.Max(x + (leftStick.x * 2.5f), -maxLateralSpeed * Mathf.Abs(leftStick.x));
        }
        else
        {
            float sign = rb.velocity.x / Mathf.Abs(rb.velocity.x);
            x = Mathf.Abs(rb.velocity.x) > 1.5f ? rb.velocity.x - (sign * 1.2f) : 0f;
        }
        rb.velocity = new Vector2(x, rb.velocity.y - gravityPerFrame);
    }
    #endregion
}
