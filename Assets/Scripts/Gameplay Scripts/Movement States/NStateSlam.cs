using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NStateSlam : NState {

    protected float maxLateralVelocity;
    protected float increasedGravityPerFrame;
    protected int _jumpLockoutFrames;
    protected int jumpLockoutFrames;

    public NStateSlam(NStateInfo info, EState state) : base(info, state)
    {
        maxLateralVelocity = info.bd.slamMaxLateralVelocity;
        increasedGravityPerFrame = info.bd.slamIncreasedGravityPerFrame;
        _jumpLockoutFrames = info.bd.slamJumpLockoutFrames;
    }

    public override void EnterState()
    {
        base.EnterState();
        //if (rb.velocity.y > 0)
        //    rb.velocity = new Vector2(rb.velocity.x, 0f);
        if (rb.velocity.y >= -7f)
            rb.velocity = new Vector2(rb.velocity.x, -7f);
        else
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y - 2f);

        jumpLockoutFrames = _jumpLockoutFrames;
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

        jumpLockoutFrames--;
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

        BottomCheck();
        NPlayerController otherPlayer = HeadCheck();
        if (otherPlayer != null && otherPlayer.GetLivingStatus())
        {
            otherPlayer.DeathBySpikes(false);
            SetBool("doubled", false);
            player.TrySpawnAir();
            return player.StateTransition(EState.boinked);
        }
        else if (GroundCheck())
        {
            if (GroundCheck())
            {
                if (IceCheck())
                    return player.StateTransition(EState.slipped);
                return player.StateTransition(EState.normal);
            }
        }
        else if (jumpLockoutFrames <= 0 && !slamButton)
            return player.StateTransition(EState.airborne);
        return false;
    }

    protected void PhysicsUpdate()
    {
        float x = 0;
        float y = rb.velocity.y - globalGravityPerFrame - increasedGravityPerFrame;
        if (!GetBool("frozen"))
        {
            if (leftStick.x > 0)
                x = Mathf.Min(x + (leftStick.x * 0.2f), maxLateralVelocity * Mathf.Abs(leftStick.x));
            else if (leftStick.x < 0)
                x = Mathf.Max(x + (leftStick.x * 0.2f), -maxLateralVelocity * Mathf.Abs(leftStick.x));
            else
            {
                float sign = rb.velocity.x / Mathf.Abs(rb.velocity.x);
                x = Mathf.Abs(rb.velocity.x) > 1.5f ? rb.velocity.x - (sign * 1.05f) : 0f;
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
