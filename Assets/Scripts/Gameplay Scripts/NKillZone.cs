using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NKillZone : MonoBehaviour {
    void OnTriggerEnter2D(Collider2D collider)
    {
        NPlayerController player = collider.gameObject.GetComponent<NPlayerController>();
        if (player != null)
        {
            player.DeathByFalling();
        }
    }
}
