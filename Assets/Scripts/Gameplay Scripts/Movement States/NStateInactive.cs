using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NStateInactive : NState
{

    public NStateInactive(NStateInfo info, EState state) : base(info, state)
    {
    }

    public override void EnterState()
    {
        base.EnterState();
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
        if (GetBool("active"))
            return player.StateTransition(EState.normal);
        return false;
    }

    protected void PhysicsUpdate()
    {
        float y = rb.velocity.y - globalGravityPerFrame;
        rb.velocity = new Vector2(0, y);
    }
    #endregion
}
