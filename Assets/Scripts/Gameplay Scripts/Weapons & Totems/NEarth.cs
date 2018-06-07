using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NEarth : MonoBehaviour {

    private Transform totemObject;
    private NTotemEarth totem;
    private Transform weaponObject;
    private NWeaponEarth weapon;
    private Animator ac;

    protected void Start()
    {
        totemObject = transform.GetChild(0);
        totem = totemObject.GetComponent<NTotemEarth>();
        weaponObject = transform.GetChild(1);
        weapon = weaponObject.GetComponent<NWeaponEarth>();
        ac = GetComponent<Animator>();
    }

    public bool TryPickup()
    {
        return !(weapon.GetWielder() != null);
    }

    public Transform GetWeapon()
    {
        return transform;
    }
}
