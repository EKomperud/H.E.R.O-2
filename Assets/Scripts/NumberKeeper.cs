using UnityEngine;
using System.Collections;

public class NumberKeeper : MonoBehaviour {

	public int numberOfRounds = 0;
	public int previousRounds = 0;
	public int P1WINS = 0;
	public int P2WINS = 0;
	public int P3WINS = 0;
	public int P4WINS = 0;
	public int numOfP = 0;
	public int preNumOfP = 0;
	public bool hasHit1 = false;
	public bool hasHit2 = false;
	public bool hasHit3 = false;
	public bool hasHit4 = false;
	public bool death = false;
	public bool death2 = false;
	public bool death3 = false;
	public bool death4 = false;
	public bool p1Input = false;
	public bool p2Input = false;
	public bool p3Input = false;
	public bool p4Input = false;
	public bool ifDied = false;


	// Use this for initialization
	void Start () {
		DontDestroyOnLoad (this.gameObject);
	}
	
	// Update is called once per frame
	void Update () {
        Debug.Log(numOfP);
	}
}
