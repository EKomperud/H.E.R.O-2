using UnityEngine;
using System.Collections;

public class RockCollider : MonoBehaviour {
    private CircleCollider2D objectCollider;

    // Use this for initialization
    void Start () {
        GetComponent<Rigidbody2D>().collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        objectCollider = gameObject.GetComponent<CircleCollider2D>();
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        PlayerWeaponScript projectile = collider.gameObject.GetComponent<PlayerWeaponScript>();
        if (projectile != null)
        {
            if (!projectile.shotType.Equals("plasma") && !projectile.shotType.Equals("rock"))
                Destroy(collider.gameObject);
        }
    }
}
