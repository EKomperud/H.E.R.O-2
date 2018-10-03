using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NStateFireBoost : NState {
    protected float boostVelocity;
    protected float boostTime;
    protected float _boostTime;
    protected Vector2 boostDirection;

    public NStateFireBoost(NStateInfo info, EState state) : base(info, state)
    {
        boostVelocity = info.bd.fireBoostVelocity;
        boostTime = info.bd.fireBoostTime;
    }

    public override void EnterState()
    {
        if (leftStick != Vector2.zero)
            SetBoostDirection(leftStick);
        _boostTime = 0f;
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
        _boostTime += Time.fixedDeltaTime;
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


        if (_boostTime >= boostTime)
        {
            BottomCheck();
            if (GroundCheck() || HeadCheck())
            {
                player.SetAnimatorTriggers("landing");
                if (IceCheck())
                    return player.StateTransition(EState.slipped);
                return player.StateTransition(EState.normal);
            }
            return player.StateTransition(EState.airborne);
        }
        return false;
    }

    protected void PhysicsUpdate()
    {
        rb.velocity = boostDirection * boostVelocity;
    }

    public void SetBoostDirection(Vector2 direction)
    {
        boostDirection = direction.normalized;
    }
    #endregion
}
