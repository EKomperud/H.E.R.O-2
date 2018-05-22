using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NStateFrozen : NState {

    private float timer;

    public NStateFrozen(NStateInfo info, EState state) : base(info, state)
    {
        validTransitions = new bool[7] { true, false, true, true, false, true, true };
    }

    public override void EnterState()
    {
        base.EnterState();
        ac.SetBool("frozen", true);
        timer = 2f;
    }

    public override void ExitState()
    {
        base.ExitState();
        ac.SetBool("frozen", false);
    }

    public override void StateUpdate()
    {
        base.StateUpdate();
        if (timer > 0)
        {
            timer -= Time.deltaTime;
        }
        else
        {
            if (GroundCheck())
            {
                ac.SetBool("grounded", true);
                player.TryStateTransition(EState.normal);
            }
            else
            {
                ac.SetBool("grounded", false);
                player.TryStateTransition(EState.jump2);
            }
        }
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
        float x;
        if (GroundCheck())
        {
            x = Mathf.Abs(rb.velocity.x) > 1.5f ? rb.velocity.x - (sign * 1.001f) : rb.velocity.x;
        }
        else
        {
            x = rb.velocity.x;
        }

        float y = rb.velocity.y - gravityPerFrame;
        rb.velocity = new Vector2(x, y);
    }
    #endregion
}
