using UnityEngine;
using System.Collections;

public class SpawnPoint : MonoBehaviour {

    private Transform player;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void SetPlayer(Transform player, string name)
    {
        this.player = player;
        PlayerController controller = player.GetComponent<PlayerController>();
        controller.name = name;
    }

    public void SpawnPlayer()
    {
        var p = Instantiate(player) as Transform;
        p.position = transform.position;
    }
}
