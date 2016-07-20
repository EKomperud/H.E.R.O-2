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

    void Update()
    {
        if (hp <= 0)
        {
            Debug.Log("here");
            if (this.gameObject.tag == "Player")
            {
                Keeper.death = true;
            }
            if (this.gameObject.tag == "Player2")
            {
                Keeper.death2 = true;
            }
            if (this.gameObject.tag == "Player3")
            {
                Keeper.death3 = true;
            }
            if (this.gameObject.tag == "Player4")
            {
                Keeper.death4 = true;
            }
            if (player.weapon != null)
            {
                Destroy(player.weapon);
            }
            Destroy(gameObject);
        }
    }


    void OnTriggerEnter2D (Collider2D collider) {
		//player = gameObject.GetComponent<PlayerScript>();
		PlayerWeaponScript shot = collider.gameObject.GetComponent<PlayerWeaponScript> ();

		if (shot != null ) {
			PlayerController target = shot.caster;
			if (!target.Equals(player))
			{
				//Debug.Log ("the code gets here");
				if (this.gameObject.tag == "Player")
				{
					shotsHaveFired = true;
				}
				if (this.gameObject.tag == "Player2")
				{
					shotsHaveFired2 = true;
				}
				hp -= shot.damage;
				//System.Threading.Thread d = new System.Threading.Thread(() => shot.connected);
				//d.Start();
				//Destroy (shot.gameObject);
			}	
		}
	}

    public void ManualDamage (int d)
    {
        hp -= d;
        OnTriggerEnter2D(null);
    }
}
