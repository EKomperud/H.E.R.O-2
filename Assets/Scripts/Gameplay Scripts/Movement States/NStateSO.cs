using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "StateBalance", menuName = "States", order = 1)]

public class NStateSO : ScriptableObject {

    #region Members
    [Header("Global")]
    [SerializeField] public float globalGravityPerFrame;

    [Header("Normal/Grounded State")]
    [SerializeField] public float normalDirectionSwitchRatio;
    [SerializeField] public float normalMaxLateralVelocity;
    [Space]

    [Header("Slippery/Grounded State")]
    [SerializeField] public float slippedDirectionSwitchRatio;
    [SerializeField] public float slippedMaxLateralVelocity;
    [Space]

    [Header("Jump1 State")]
    [SerializeField] public float jump1DirectionSwitchRatio;
    [SerializeField] public float jump1MaxLateralVelocity;
    [SerializeField] public float jump1InitialVelocity;
    [SerializeField] public float jump1ContinuedHoldVelocity;
    [SerializeField] public int jump1ContinuedHoldFrames;
    [SerializeField] public int jump1ContinuedHoldDelayFrames;
    [SerializeField] public int jump1ShortHopFrames;
    [SerializeField] public int jump1ShortHopVelocity;
    [SerializeField] public int jump1RegularHopVelocity;
    [Space]

    [Header("Jump2 State")]
    [SerializeField] public float jump2DirectionSwitchRatio;
    [SerializeField] public float jump2MaxLateralVelocity;
    [SerializeField] public float jump2InitialVelocity;
    [Space]

    [Header("Slam State")]
    [SerializeField] public float slamIncreasedGravityPerFrame;
    [SerializeField] public float slamMaxLateralVelocity;
    [SerializeField] public int slamJumpLockoutFrames;

    [Header("Ash State")]
    [SerializeField] public float ashGravityPerFrame;
    [Space]

    [Header("Airborne State")]
    [SerializeField] public float airborneDirectionSwitchRatio;
    [SerializeField] public float airborneHardMaxLateralVelocity;
    [SerializeField] public float airborneSoftMaxLateralVelocity;
    [Space]

    [Header("Bounced State")]
    [SerializeField] public float bouncedDirectionSwitchRatio;
    [SerializeField] public float bouncedMaxLateralVelocity;
    [SerializeField] public float bouncedInitialVelocity;
    [SerializeField] public int bouncedTransitionLockoutFrames;
    [Space]

    [Header("Boinked State")]
    [SerializeField] public float boinkedInitialVelocity;
    [SerializeField] public int boinkedTransitionLockoutFrames;

    [Header("Pushed State")]
    [SerializeField] public float pushedDuration;

    [Header("Air Dodge State")]
    [SerializeField] public float airDodgeInitialVelocity;
    [SerializeField] public float airDodgeVelocityDecayRate;
    [SerializeField] public float airDodgeVelocityMultiplier;

    [Header("Suspended State")]
    [SerializeField] public float suspendedSuspensionTime;
    [SerializeField] public float suspendedVelocityDecay;

    [Header("Fire Boost State")]
    [SerializeField] public float fireBoostVelocity;
    [SerializeField] public float fireBoostTime;

    [Space]

    [Header("Frozen SubState")]
    [SerializeField] public float frozenDuration;
    [SerializeField] public float frozenSlideSpeed;
    [SerializeField] public float frozenGravityPerFrame;
    [Space]

    [Header("Burning SubState")]
    [SerializeField] public float burningDuration;


    #endregion
}
