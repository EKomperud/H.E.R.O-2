using UnityEngine;
using System.Collections;

public class HealthScript : MonoBehaviour {

	public int hp = 5;
	public bool shotsHaveFired = false;
	public bool shotsHaveFired2 = false;
	private PlayerController player; 
	public NumberKeeper Keeper;

	void Awake () {
		player = gameObject.GetComponent<PlayerController> ();
		Keeper = GameObject.Find("NumberKeeper").GetComponent<NumberKeeper> ();
	}

	void OnTriggerEnter2D (Collider2D collider) {
		//player = gameObject.GetComponent<PlayerScript>();
		PlayerWeaponScript shot = collider.gameObject.GetComponent<PlayerWeaponScript> ();

		if (hp <= 0)
		{
			if (this.gameObject.tag == "Player")
			{
				Keeper.death = true;
			}
			if (this.gameObject.tag == "Player2")
			{
				Keeper.death2 = true;
			}
			if (player.weapon != null)
			{
				Destroy(player.weapon);
			}
			Destroy(gameObject);
		}
		if (shot != null ) {
			PlayerController target = shot.caster;
			if (!target.Equals(player))
			{
				Debug.Log ("the code gets here");
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
			}	
		}
	}
}
