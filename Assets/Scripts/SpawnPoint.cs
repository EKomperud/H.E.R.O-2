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

    public void SpawnPlayer(string playerNumber, string character, Transform prefab)
    {
        var p = Instantiate(prefab) as Transform;
        p.position = transform.position;
        PlayerController controller = p.gameObject.GetComponent<PlayerController>();
        controller.name = playerNumber;
        controller.character = character;
    }
}
