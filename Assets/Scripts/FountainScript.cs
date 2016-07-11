using UnityEngine;
using System.Collections;

public class FountainScript : MonoBehaviour {

	public Transform shotPrefab;
    public string fountainType;

    public float cooldownRate = 3f;
    private float cooldown;

	public ArrayList Players = new ArrayList();

    void Start ()
    {
        cooldown = 0;
    }

    void Update ()
    {
        if (cooldown > 0)
        {
            cooldown -= Time.deltaTime;
        }
    }

	void OnTriggerEnter2D (Collider2D collider) {        
		PlayerController player = collider.gameObject.GetComponent<PlayerController> ();
		if (player != null) {
            player.fountain = this;
			player.inFountain = true;
			Players.Add (player);
		}
	}

	void OnTriggerExit2D (Collider2D collider) {
		PlayerController player = collider.gameObject.GetComponent<PlayerController> ();
		if (player != null) {
            if (player.fountain.Equals(this))
            {
                player.fountain = null;
                player.inFountain = false;
            }
			Players.Remove (player);
		}
	}

	public void CreateShot (PlayerController player) {
        if (cooldown <= 0)
        {
            cooldown = cooldownRate;
            if (fountainType.Equals("plasma"))
            {
                var shot = Instantiate(shotPrefab) as Transform;
                shot.position = transform.position;
                PlayerWeaponScript shotScript = shot.gameObject.GetComponent<PlayerWeaponScript>();
                player.weapon = shotScript;
                shotScript.caster = player;
                shotScript.MoveToCaster();
            }
            if (fountainType.Equals("fire"))
            {
                var shot1 = Instantiate(shotPrefab) as Transform;
                var shot2 = Instantiate(shotPrefab) as Transform;
                var shot3 = Instantiate(shotPrefab) as Transform;
                shot1.position = shot2.position = shot3.position = transform.position;
                PlayerWeaponScript shotScript1 = shot1.gameObject.GetComponent<PlayerWeaponScript>();
                PlayerWeaponScript shotScript2 = shot2.gameObject.GetComponent<PlayerWeaponScript>();
                PlayerWeaponScript shotScript3 = shot3.gameObject.GetComponent<PlayerWeaponScript>();
                player.weapon = shotScript1;
                player.weapons.Push(shotScript2);
                player.weapons.Push(shotScript3);
                shotScript1.caster = shotScript2.caster = shotScript3.caster = player;
                Debug.Log("caster set");
                //shotScript1.caster = player;
                //shotScript2.caster = player;
                //shotScript3.caster = player;
                shotScript1.MoveToCaster();
                shotScript2.MoveToCaster();
                shotScript3.MoveToCaster();
            }
            else if (fountainType.Equals("water"))
            {
                var shot = Instantiate(shotPrefab) as Transform;
                shot.position = transform.position;
                PlayerWeaponScript shotScript = shot.gameObject.GetComponent<PlayerWeaponScript>();
                player.weapon = shotScript;
                shotScript.caster = player;
                shotScript.MoveToCaster();
            }
        }
	}
		
}
