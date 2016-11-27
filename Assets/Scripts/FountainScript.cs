using UnityEngine;
using System.Collections;

public class FountainScript : MonoBehaviour {

	public Transform shotPrefab;
    public string fountainType;

    public float cooldownRate = 3f;
    public float cooldown;

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
            if (cooldown <=0)
            {
                Transform particles = transform.GetChild(0);
                ParticleSystem p = particles.GetComponent<ParticleSystem>();
                p.Play();
                AnimationController2D anim = GetComponent<AnimationController2D>();
                anim.setAnimation(fountainType + "Norm");
            }
        }
    }

	void OnTriggerEnter2D (Collider2D collider) {
        if (collider.tag.Equals("Player"))
        {
            PlayerController2 player = collider.gameObject.GetComponent<PlayerController2>();
            player.fountain = this;
            player.inFountain = true;
            Players.Add(player);
        }        
	}

	void OnTriggerExit2D (Collider2D collider) {
        if (collider.tag.Equals("Player"))
        {
            PlayerController2 player = collider.gameObject.GetComponent<PlayerController2>();
            if (player.fountain.Equals(this))
            {
                player.fountain = null;
                player.inFountain = false;
            }
            Players.Remove(player);
        }
	}

	public void CreateShot (PlayerController2 player) {
        if (cooldown <= 0)
        {
            cooldown = cooldownRate;
            if (fountainType.Equals("plasma"))
            {
                var shot = Instantiate(shotPrefab) as Transform;
                shot.position = transform.position;
                IWeapon shotScript = shot.gameObject.GetComponent<IWeapon>();
                player.weapon = shotScript;
                shotScript.caster = player;
                //shotScript.MoveToCaster();
            }
            if (fountainType.Equals("fire"))
            {
                var shot1 = Instantiate(shotPrefab) as Transform;
                var shot2 = Instantiate(shotPrefab) as Transform;
                var shot3 = Instantiate(shotPrefab) as Transform;
                shot1.position = shot2.position = shot3.position = transform.position;
                IWeapon shotScript1 = shot1.gameObject.GetComponent<IWeapon>();
                IWeapon shotScript2 = shot2.gameObject.GetComponent<IWeapon>();
                IWeapon shotScript3 = shot3.gameObject.GetComponent<IWeapon>();
                player.weapon = shotScript1;
                player.weapons.Push(shotScript2);
                player.weapons.Push(shotScript3);
                shotScript1.caster = shotScript2.caster = shotScript3.caster = player;
                //shotScript1.MoveToCaster();
                //shotScript2.MoveToCaster();
                //shotScript3.MoveToCaster();
            }
            else if (fountainType.Equals("water"))
            {
                var shot = Instantiate(shotPrefab) as Transform;
                shot.position = transform.position;
                IWeapon shotScript = shot.gameObject.GetComponent<IWeapon>();
                player.weapon = shotScript;
                shotScript.caster = player;
                //shotScript.MoveToCaster();

            }
            Transform particles = transform.GetChild(0);
            ParticleSystem p = particles.GetComponent<ParticleSystem>();
            p.Stop();
            AnimationController2D anim = GetComponent<AnimationController2D>();
            anim.setAnimation(fountainType + "Despawn");
        }
	}
		
}
