using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NTotemEarth : NTotem {

    NWeaponEarth earth;

    protected override void Start()
    {
        base.Start();
        earth = transform.GetComponentInParent<NWeaponEarth>();
    }

    public override bool Pickup()
    {
        //return !(earth.GetWielder() != null);
        return !earth.GetActive();
    }

    public override Transform GetWeapon()
    {
        return earth.transform;
    }
}
