using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NWeaponStateOrbit : NWeaponState {

    private float x, y, z, angle, rotationSpeed, bobSpeed, radius;

    void Start () {
        x = y = z = 0;
        rotationSpeed = 4f;
        bobSpeed = rotationSpeed * 2;
        radius = 1f;
        Vector3 pct = wielder.transform.position;
        transform.position = new Vector3(pct.x + 0.5f, pct.y + 0.5f, pct.z);
    }
}
