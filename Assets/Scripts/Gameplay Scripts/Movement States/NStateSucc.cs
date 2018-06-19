using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NStateSucc : NState
{

    private Vector3 blackHole;

    public NStateSucc(NStateInfo info, EState state, Vector3 blackHole) : base(info, state)
    {
        this.blackHole = blackHole;
    }

    public override void EnterState()
    {
        base.EnterState();
    }

    public override void ExitState()
    {
        base.ExitState();
    }

    public override void StateUpdate()
    {
        if (player.transform.localScale.sqrMagnitude >= 0.0005f)
        {
            Vector3 diff = blackHole - player.transform.position;
            player.transform.position += diff * 0.075f;
            player.transform.Rotate(0f, 0f, 1f);
            player.transform.localScale *= 0.99f;
        }

        base.StateUpdate();
    }

    public override void StateFixedUpdate()
    {
        base.StateFixedUpdate();
    }

    public void SetBlackHolePosition(Vector3 blackHole)
    {
        this.blackHole = blackHole;
    }

}
