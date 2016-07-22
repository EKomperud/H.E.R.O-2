using UnityEngine;
using System.Collections;

public class CrossSceneRetention : MonoBehaviour {
    private static CrossSceneRetention instance;
	void Start () {
        DontDestroyOnLoad(this);
        if (instance == null)
        {
            instance = this;
        }
        else
            Destroy(gameObject);
    }
}
