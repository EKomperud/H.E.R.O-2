using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NStateBounced : NState {

    protected float directionSwitchRatio;
    protected float maxLateralVelocity;
    protected float initialVerticalVelocity;
    protected int transitionLockoutFrames;
    protected int _transitionLockoutFrames;

    public NStateBounced(NStateInfo info, EState state, float initialVelocity, int lockoutFrames) : base(info, state)
    {
        directionSwitchRatio = info.bd.bouncedDirectionSwitchRatio;
        maxLateralVelocity = info.bd.bouncedMaxLateralVelocity;
        initialVerticalVelocity = initialVelocity;
        _transitionLockoutFrames = lockoutFrames;
    }

    public override void EnterState()
    {
        base.EnterState();
        Vector2 v = rb.velocity;
        rb.velocity = new Vector2(v.x, initialVerticalVelocity);
        transitionLockoutFrames = _transitionLockoutFrames;
        SetBool("bounced", false);
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

        transitionLockoutFrames--;
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
        else if (freshJumpButton && transitionLockoutFrames <= 0 && !GetBool("doubled"))
            return player.StateTransition(EState.jump2);

        BottomCheck();
        if (GroundCheck())
        {
            if (IceCheck())
                return player.StateTransition(EState.slipped);
            return player.StateTransition(EState.normal);
        }
        else if (slamButton && transitionLockoutFrames <= 0)
            return player.StateTransition(EState.slam);
        return false;
    }

    protected void PhysicsUpdate()
    {
        float x = 0;
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
        }
        else
        {
            float sign = rb.velocity.x / Mathf.Abs(rb.velocity.x);
            x = Mathf.Abs(rb.velocity.x) > 1.5f ? rb.velocity.x - (sign * 1.05f) : 0f;
        }

        float y = rb.velocity.y - globalGravityPerFrame;
        rb.velocity = new Vector2(x, y);
    }
    #endregion
}
