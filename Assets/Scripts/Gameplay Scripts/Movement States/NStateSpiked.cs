using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NStateSpiked : NState {

    private Vector2 spikedDirection;

    public NStateSpiked(NStateInfo info, EState state, Vector2 spikedDirection) : base(info, state)
    {
        this.spikedDirection = spikedDirection;
    }

    public override void EnterState()
    {
        base.EnterState();
        rb.velocity = spikedDirection;
        if (spikedDirection.x < 0)
            sr.flipX = true;
        else if (spikedDirection.x > 0)
            sr.flipX = false;
        SetBool("pushed",false);
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

    public void SetSpikedDirection(Vector2 direction)
    {
        spikedDirection = direction;
    }

    #region Helpers
    protected bool TransitionChecks()
    {
        if (GetBool("succed"))
            return player.StateTransition(EState.succ);
        else if (GetBool("ashed"))
            return player.StateTransition(EState.ashes);
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
    #endregion
}
