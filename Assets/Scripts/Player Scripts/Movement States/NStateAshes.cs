using System.Collections;
using System.Collections.Generic;
using InControl;
using UnityEngine;

public class NStateAshes : NState {

    public NStateAshes(NStateInfo info, EState state) : base(info, state)
    {
        validTransitions = new bool[7] { false, false, false, false, false, false, true };
    }

    public override void EnterState()
    {
        base.EnterState();
        ac.SetBool("ashes", true);
    }

    public override void StateUpdate()
    {
        base.StateUpdate();
    }

    public override void StateFixedUpdate()
    {
        PhysicsUpdate();
        base.StateFixedUpdate();
    }

    #region Helpers
    protected void PhysicsUpdate()
    {
        float sign = rb.velocity.x / Mathf.Abs(rb.velocity.x);
        float x = Mathf.Abs(rb.velocity.x) > 1.5f ? rb.velocity.x - (sign * 1.05f) : 0f;

        float y = rb.velocity.y - gravityPerFrame;
        rb.velocity = new Vector2(x, y);
    }
    #endregion
}
