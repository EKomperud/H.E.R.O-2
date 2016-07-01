using UnityEngine;
using System.Collections;

public class HealthScript : MonoBehaviour {

	public int hp = 5;
	public bool shotsHaveFired = false;
	public bool shotsHaveFired2 = false;
	private PlayerController player; 
	public bool death = false;
	public bool death2 = false;

	void Awake () {
		player = gameObject.GetComponent<PlayerController> ();
	}

	void OnTriggerEnter2D (Collider2D collider) {
		//player = gameObject.GetComponent<PlayerScript>();
		PlayerWeaponScript shot = collider.gameObject.GetComponent<PlayerWeaponScript> ();
        
        if (shot != null ) {
            PlayerController target = shot.caster;
            if (!target.Equals(player))
            {
                if (this.gameObject.tag == "Player")
                {
                    shotsHaveFired = true;
                }
                if (this.gameObject.tag == "Player2")
                {
                    shotsHaveFired2 = true;
                }
                hp -= shot.damage;
                System.Threading.Thread d = new System.Threading.Thread(() => shot.Destroy());
                d.Start();
                //Destroy (shot.gameObject);
                if (hp <= 0)
                {
                    if (this.gameObject.tag == "Player")
                    {
                        death = true;
                    }
                    if (this.gameObject.tag == "Player2")
                    {
                        death2 = true;
                    }
                    if (player.weapon != null)
                    {
                        Destroy(player.weapon);
                    }
                    Destroy(gameObject);
                }
            }	
		}
	}
}
