using UnityEngine;
using System.Collections;

public abstract class IWeapon : MonoBehaviour {

    public PlayerController2 caster;

    public abstract void Attack(float x, float y);

    //public void Destroy();

    //void MoveToCaster();
}
