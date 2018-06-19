using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NStatePushed : NState {

    private Vector2 pushDirection;
    private float pushInitialVelocity;
    private float _pushDuration;
    private float pushDuration;

    public NStatePushed(NStateInfo info, EState state, Vector2 pushDirection) : base(info, state)
    {
        this.pushDirection = pushDirection;
        _pushDuration = info.bd.pushedDuration;
        pushInitialVelocity = info.bd.pushedInitialVelocity;
    }

    public override void EnterState()
    {
        base.EnterState();
        rb.velocity = pushDirection * pushInitialVelocity;
        if (pushDirection.x < 0)
            sr.flipX = true;
        else if (pushDirection.x > 0)
            sr.flipX = false;
        ac.SetBool("grounded", false);
        pushDuration = _pushDuration;
    }

    public override void ExitState()
    {
        SetBool("pushed", false);
        base.ExitState();
    }

    public override void StateUpdate()
    {
        base.StateUpdate();
        if (pushDirection.x < 0)
            sr.flipX = true;
        else if (pushDirection.x > 0)
            sr.flipX = false;
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

        pushDuration -= Time.fixedDeltaTime;
        if (pushDuration <= 0)
        {
            BottomCheck();
            if (GroundCheck() || HeadCheck())
            {
                if (IceCheck())
                    return player.StateTransition(EState.slipped);
                return player.StateTransition(EState.normal);
            }
            else
                return player.StateTransition(EState.airborne);
        }
        return false;
    }

    protected void PhysicsUpdate()
    {
        float x = 0;
        if (GetBool("frozen"))
        {
            x = rb.velocity.x;
        }
        else
        {
            float sign = rb.velocity.x / Mathf.Abs(rb.velocity.x);
            x = Mathf.Abs(rb.velocity.x) > 1.5f ? rb.velocity.x - (sign * 0.15f) : 0f;
        }

        float y = rb.velocity.y - globalGravityPerFrame;
        rb.velocity = new Vector2(x, y);
    }

    public void SetPushedDirection(Vector2 direction)
    {
        pushDirection = direction;
    }
    #endregion
}
