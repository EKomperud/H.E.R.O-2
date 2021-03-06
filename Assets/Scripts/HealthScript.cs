﻿using UnityEngine;
using System.Collections;

public class HealthScript : MonoBehaviour {

	public int hp = 5;
	public bool shotsHaveFired = false;
	public bool shotsHaveFired2 = false;
	private PlayerController player; 
	public NumberKeeper Keeper;
    private GameManager manager;

	void Awake () {
		player = gameObject.GetComponent<PlayerController> ();
		Keeper = GameObject.Find("NumberKeeper").GetComponent<NumberKeeper> ();
        manager = GameObject.Find("GameManager").GetComponent<GameManager>(); ;
    }


    void OnTriggerEnter2D (Collider2D collider) {
		//player = gameObject.GetComponent<PlayerScript>();
		PlayerWeaponScript shot = collider.gameObject.GetComponent<PlayerWeaponScript> ();
        GameObject killz = collider.gameObject;
		if (shot != null && !player.dead) {
			PlayerController target = shot.caster;
			if (target == null || !target.Equals(player))
			{
				hp -= shot.damage;
                if (hp <= 0)
                {
                    manager.KillPlayer(player.name);
                    if (player.weapon != null)
                    {
                        Destroy(player.weapon);
                    }
                    player.dead = true;
                    player.DeathAnim(shot.shotType);
                }
            }	
		}
        else if ((killz.gameObject.tag.Equals("KillZ") || killz.gameObject.tag.Equals("Spikes"))&& !player.dead)
        {
            if (killz.gameObject.tag.Equals("Spikes"))
            {
                Transform blood = transform.GetChild(3);
                ParticleSystem bloody = blood.GetComponent<ParticleSystem>();
                bloody.Play();
            }
            hp = 0;
            manager.KillPlayer(player.name);
            if (player.weapon != null)
            {
                Destroy(player.weapon);
            }
            player.dead = true;
            player.DeathAnim(killz.gameObject.tag);
        }
	}

    public void ManualDamage (int d, string cause)
    {
        if (hp > 0 && !player.dead)
        {
            hp -= d;
            if (hp <= 0)
            {
                manager.KillPlayer(player.name);
                if (player.weapon != null)
                {
                    Destroy(player.weapon);
                }
                player.dead = true;
                player.DeathAnim(cause);
            }
        }
    }
}
