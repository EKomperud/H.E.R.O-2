using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NStateAirborne : NState
{
    protected float directionSwitchRatio;
    protected float hardMaxLateralVelocity;
    protected float softMaxLateralVelocity;

    private float doubleJumpDelay;

    public NStateAirborne(NStateInfo info, EState state) : base(info, state)
    {
        directionSwitchRatio = info.bd.airborneDirectionSwitchRatio;
        hardMaxLateralVelocity = info.bd.airborneHardMaxLateralVelocity;
        softMaxLateralVelocity = info.bd.airborneSoftMaxLateralVelocity;
    }

    public override void EnterState()
    {
        base.EnterState();
        doubleJumpDelay = 0.075f;
        player.SetAnimatorBools("falling", true);
    }

    public override void ExitState()
    {
        base.ExitState();
        player.SetAnimatorBools("falling", false);
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
        else if (freshJumpButton && doubleJumpDelay <= 0 && !GetBool("doubled"))
            return player.StateTransition(EState.jump2);

        BottomCheck();
        if (GroundCheck() || HeadCheck())
        {
            player.SetAnimatorTriggers("landing");
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
        float y = rb.velocity.y - (GetBool("frozen") ? frozenGravityPerFrame : globalGravityPerFrame);
        float leftStickSign = Mathf.Sign(leftStick.x);
        float rbSign = Mathf.Sign(rb.velocity.x);
        if (leftStick.x != 0)
        {
            x = rbSign == leftStickSign ? rb.velocity.x : rb.velocity.x * directionSwitchRatio;
            x += leftStick.x * 1.2f;
        }
        else
        {
            x = Mathf.Abs(rb.velocity.x) > 1.5f ? rb.velocity.x - (rbSign * 1.05f) : 0f;
        }

        x = Mathf.Abs(x) > hardMaxLateralVelocity ? rbSign * hardMaxLateralVelocity : x;
        x = Mathf.Abs(x) > softMaxLateralVelocity ? x * 0.85f : x;
        rb.velocity = new Vector2(x, y);
    }
    #endregion
}
