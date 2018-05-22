﻿using System.Collections;
using System.Collections.Generic;
using InControl;
using UnityEngine;

public class NStateJump2 : NState
{

    public NStateJump2(NStateInfo info, EState state) : base(info, state)
    {
        validTransitions = new bool[7] { true, false, false, true, false, true, true };
    }

    public override void EnterState()
    {
        base.EnterState();
        Vector2 v = rb.velocity;
        rb.velocity = new Vector2(v.x, maxVerticalSpeed);
        ac.SetBool("grounded", false);
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
        if (GroundCheck())
        {
            rb.velocity = new Vector2(rb.velocity.x, 0f);
            player.TryStateTransition(EState.normal);
            return true;
        }
        return false;
    }

    protected void PhysicsUpdate()
    {
        float x = 0;
        if (leftStick.x > 0)
        {
            x = rb.velocity.x > 0 ? rb.velocity.x : rb.velocity.x * directionSwitchRatio;
            x = Mathf.Min(x + (leftStick.x * 1.5f), maxLateralSpeed * Mathf.Abs(leftStick.x));
        }
        else if (leftStick.x < 0)
        {
            x = rb.velocity.x < 0 ? rb.velocity.x : rb.velocity.x * directionSwitchRatio;
            x = Mathf.Max(x + (leftStick.x * 1.5f), -maxLateralSpeed * Mathf.Abs(leftStick.x));
        }
        else
        {
            float sign = rb.velocity.x / Mathf.Abs(rb.velocity.x);
            x = Mathf.Abs(rb.velocity.x) > 1.5f ? rb.velocity.x - (sign * 1.05f) : 0f;
        }

        float y = rb.velocity.y - gravityPerFrame;
        rb.velocity = new Vector2(x, y);
    }
    #endregion
}