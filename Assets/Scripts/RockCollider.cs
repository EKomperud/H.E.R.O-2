using UnityEngine;
using System.Collections;

public class RockCollider : MonoBehaviour {
    private CircleCollider2D objectCollider;
    public PlayerController caster;

    // Use this for initialization
    void Start () {
        //GetComponent<Rigidbody2D>().collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        objectCollider = gameObject.GetComponent<CircleCollider2D>();
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        PlayerWeaponScript projectile = collider.gameObject.GetComponent<PlayerWeaponScript>();
        PlayerController player = collider.gameObject.GetComponent<PlayerController>();
        if (projectile != null)
        {
            if (!projectile.shotType.Equals("plasma") && !projectile.shotType.Equals("rock"))
                Destroy(collider.gameObject);
        }
        if (player != null && (caster != null && !caster.Equals(player)))
        {
            player.health.ManualDamage(1, "rock");
        }
    }
}
