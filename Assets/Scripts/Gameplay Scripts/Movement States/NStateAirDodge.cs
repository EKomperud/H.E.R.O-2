using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NStateAirDodge : NState {
    protected float initialVelocity;
    protected float velocityDecayRate;
    protected float velocityMultiplier;
    protected Vector2 dodgeDirection;
    protected float t;

    public NStateAirDodge(NStateInfo info, EState state) : base(info, state)
    {
        initialVelocity = info.bd.airDodgeInitialVelocity;
        velocityDecayRate = info.bd.airDodgeVelocityDecayRate;
        velocityMultiplier = info.bd.airDodgeVelocityMultiplier;
    }

    public override void EnterState()
    {
        base.EnterState();
        t = 0f;
        dodgeDirection = dodgeDirection != Vector2.zero ? dodgeDirection : rb.velocity.normalized;
        rb.velocity = dodgeDirection * initialVelocity;
        SetBool("dodged", false);
        player.SetAnimatorTriggers("jumping");
        player.SetAnimatorBools("falling", false);
    }

    public override void ExitState()
    {
        base.ExitState();
    }

    public override void StateFixedUpdate()
    {
        if (TransitionChecks())
            return;

        t += Time.fixedDeltaTime;
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


        bool groundCheck = false;
        if (t >= 0.15f)
        {
            BottomCheck();
            groundCheck = GroundCheck();
            if (groundCheck || HeadCheck())
            {
                player.SetAnimatorTriggers("landing");
                if (IceCheck())
                    return player.StateTransition(EState.slipped);
                return player.StateTransition(EState.normal);
            }
            else if (slamButton)
                return player.StateTransition(EState.slam);
            else if (t >= 0.5f && !groundCheck)
                return player.StateTransition(EState.airborne);

        }
        return false;
    }

    protected void PhysicsUpdate()
    {
        dodgeDirection = leftStick != Vector2.zero ? leftStick : dodgeDirection * 0.85f;
        rb.velocity = (-Mathf.Log(t * velocityDecayRate) * dodgeDirection) * velocityMultiplier;
    }

    public void SetDashDirection(Vector2 direction)
    {
        dodgeDirection = direction;
    }
    #endregion
}
