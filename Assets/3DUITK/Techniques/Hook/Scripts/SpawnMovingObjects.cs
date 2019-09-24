using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnMovingObjects : MonoBehaviour {

    public GameObject thingToSpwan;

	// Use this for initialization
	void Start () {
        InvokeRepeating("Spawn", 2, 2);
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void Spawn()
    {
        Instantiate(thingToSpwan, transform.position, transform.rotation);
    }
}
