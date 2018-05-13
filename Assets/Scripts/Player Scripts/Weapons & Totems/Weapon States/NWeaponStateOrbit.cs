using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;

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
	
	public override void StateUpdate (InputDevice j) {
        if (j.RightStick.Value == Vector2.zero)
        {
            Vector3 tp = wielder.transform.position;
            angle += rotationSpeed * Time.deltaTime;
            y += bobSpeed * Time.deltaTime;
            Vector2 offset = new Vector2(Mathf.Sin(angle), Mathf.Cos(angle)) * radius;
            transform.position = new Vector3(tp.x + offset.x, tp.y + (offset.y / 4f), tp.z + offset.y);
        }
        else
        {

        }
    }
}
