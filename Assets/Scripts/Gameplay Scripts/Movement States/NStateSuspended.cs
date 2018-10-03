using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NStateSuspended : NState {
    protected float suspensionTime;
    protected float _suspensionTime;
    protected float velocityDecay;

    public NStateSuspended(NStateInfo info, EState state) : base(info, state)
    {
        suspensionTime = info.bd.suspendedSuspensionTime;
        velocityDecay = info.bd.suspendedVelocityDecay;
    }

    public override void EnterState()
    {
        SetBool("boosted", false);
        _suspensionTime = 0f;
        base.EnterState();
    }

    public override void ExitState()
    {
        base.ExitState();
    }

    public override void StateFixedUpdate()
    {
        if (TransitionChecks())
            return;
        _suspensionTime += Time.fixedDeltaTime;
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

        if (_suspensionTime >= suspensionTime)
            return player.StateTransition(EState.fireBoost);
        return false;
    }

    protected void PhysicsUpdate()
    {
        rb.velocity = rb.velocity.sqrMagnitude > 0.25f ? rb.velocity * velocityDecay : Vector2.zero;
    }

    public float GetSuspensionTime()
    {
        return suspensionTime;
    }
    #endregion
}
