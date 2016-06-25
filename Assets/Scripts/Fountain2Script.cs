using UnityEngine;
using System.Collections;

public class Fountain2Script : MonoBehaviour
{

    public Transform shotPrefab;

    public ArrayList Players = new ArrayList();

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        Player2Controller player = collider.gameObject.GetComponent<Player2Controller>();
        if (player != null)
        {
            player.fountain = this;
            player.inFountain = true;
            Players.Add(player);
        }
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        Player2Controller player = collider.gameObject.GetComponent<Player2Controller>();
        if (player != null)
        {
            if (player.fountain.Equals(this))
            {
                player.fountain = null;
                player.inFountain = false;
            }
            Players.Remove(player);
        }
    }

    public void CreateShot(Player2Controller player)
    {
        var shot = Instantiate(shotPrefab) as Transform;
        shot.position = transform.position;

        Player2WeaponScript shotScript = shot.gameObject.GetComponent<Player2WeaponScript>();
        player.weapon = shotScript;
        shotScript.caster = player;
        shotScript.MoveToCaster();
    }

}