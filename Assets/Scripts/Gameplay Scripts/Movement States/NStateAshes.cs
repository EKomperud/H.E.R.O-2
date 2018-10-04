using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NStateAshes : NState {

    protected float gravityPerFrame;

    public NStateAshes(NStateInfo info, EState state) : base(info, state)
    {
        gravityPerFrame = info.bd.ashGravityPerFrame;
    }

    public override void EnterState()
    {
        base.EnterState();
        player.SetAnimatorBools("ashes", true);
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
        else if (GetBool("pushed"))
            return player.StateTransition(EState.pushed);
        return false;
    }

    protected void PhysicsUpdate()
    {
        float sign = rb.velocity.x / Mathf.Abs(rb.velocity.x);
        float x = Mathf.Abs(rb.velocity.x) > 1.5f ? rb.velocity.x - (sign * 1.05f) : 0f;

        float y = rb.velocity.y - globalGravityPerFrame - gravityPerFrame;
        rb.velocity = new Vector2(x, y);
    }
    #endregion
}
