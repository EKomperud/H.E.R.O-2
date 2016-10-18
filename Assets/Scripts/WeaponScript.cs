using UnityEngine;
using System.Collections;

public interface IWeapon {

    PlayerController2 caster { get; set; }

    void Attack(float x, float y);

    //void MoveToCaster();
}
