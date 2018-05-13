using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "StateBalance", menuName = "States", order = 1)]

public class NStateSO : ScriptableObject {

    #region Members
    [Header("Normal/Grounded State")]
    [SerializeField] float normalDirectionSwitchRatio;
    [SerializeField] float normalMaxLateralSpeed;
    [SerializeField] float normalMaxVerticalSpeed;
    [SerializeField] float normalGravityPerFrame;
    [Space]

    [Header("Jump1 State")]
    [SerializeField] float jump1DirectionSwitchRatio;
    [SerializeField] float jump1MaxLateralSpeed;
    [SerializeField] float jump1MaxVerticalSpeed;
    [SerializeField] float jump1GravityPerFrame;
    [Space]

    [Header("Jump2 State")]
    [SerializeField] float jump2DirectionSwitchRatio;
    [SerializeField] float jump2MaxLateralSpeed;
    [SerializeField] float jump2MaxVerticalSpeed;
    [SerializeField] float jump2GravityPerFrame;
    [Space]

    [Header("Ash State")]
    [SerializeField] float ashDirectionSwitchRatio;
    [SerializeField] float ashMaxLateralSpeed;
    [SerializeField] float ashMaxVerticalSpeed;
    [SerializeField] float ashGravityPerFrame;
    [Space]

    [Header("Airborne State")]
    [SerializeField] float airborneDirectionSwitchRatio;
    [SerializeField] float airborneMaxLateralSpeed;
    [SerializeField] float airborneMaxVerticalSpeed;
    [SerializeField] float airborneGravityPerFrame;
    [Space]

    [Header("Frozen State")]
    [SerializeField] float frozenDirectionSwitchRatio;
    [SerializeField] float frozenMaxLateralSpeed;
    [SerializeField] float frozenMaxVerticalSpeed;
    [SerializeField] float frozenGravityPerFrame;

    #endregion

    #region Getters & Setters
    public float GetDirectionSwitchRatio(EState state)
    {
        switch (state)
        {
            case EState.normal:
                return normalDirectionSwitchRatio;
            case EState.jump1:
                return jump1DirectionSwitchRatio;
            case EState.jump2:
                return jump2DirectionSwitchRatio;
            case EState.ashes:
                return ashDirectionSwitchRatio;
            case EState.airborne:
                return airborneDirectionSwitchRatio;
            case EState.frozen:
                return frozenDirectionSwitchRatio;
            default:
                return -1;
        }
    }

    public float GetMaxLateralSpeed(EState state)
    {
        switch (state)
        {
            case EState.normal:
                return normalMaxLateralSpeed;
            case EState.jump1:
                return jump1MaxLateralSpeed;
            case EState.jump2:
                return jump2MaxLateralSpeed;
            case EState.ashes:
                return ashMaxLateralSpeed;
            case EState.airborne:
                return airborneMaxLateralSpeed;
            case EState.frozen:
                return frozenMaxLateralSpeed;
            default:
                return -1;
        }
    }

    public float GetMaxVerticalSpeed(EState state)
    {
        switch (state)
        {
            case EState.normal:
                return normalMaxVerticalSpeed;
            case EState.jump1:
                return jump1MaxVerticalSpeed;
            case EState.jump2:
                return jump2MaxVerticalSpeed;
            case EState.ashes:
                return ashMaxVerticalSpeed;
            case EState.airborne:
                return airborneMaxVerticalSpeed;
            case EState.frozen:
                return frozenMaxVerticalSpeed;
            default:
                return -1;
        }
    }

    public float GetGravityPerFrame(EState state)
    {
        switch (state)
        {
            case EState.normal:
                return normalGravityPerFrame;
            case EState.jump1:
                return jump1GravityPerFrame;
            case EState.jump2:
                return jump2GravityPerFrame;
            case EState.ashes:
                return ashGravityPerFrame;
            case EState.airborne:
                return airborneGravityPerFrame;
            case EState.frozen:
                return frozenGravityPerFrame;
            default:
                return -1;
        }
    }
    #endregion
}
