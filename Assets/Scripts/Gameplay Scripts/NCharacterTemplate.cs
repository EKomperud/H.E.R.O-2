using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NCharacterTemplate  {
    public RuntimeAnimatorController head;
    public RuntimeAnimatorController body;
    public RuntimeAnimatorController arms;
    public RuntimeAnimatorController legs;
    public Color pants;

    public RuntimeAnimatorController[] GetAnimatorControllers()
    {
        return new RuntimeAnimatorController[] { body, head, arms, legs };
    }

    public Color GetPants()
    {
        return pants;
    }
}
