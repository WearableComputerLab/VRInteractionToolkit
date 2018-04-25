using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Information on hook technique
// http://www.eecs.ucf.edu/isuelab/publications/pubs/Cashion_Jeffrey_A_201412_PhD.pdf pf 13

public class Hook : MonoBehaviour {

    List<GameObject> nearbyObjects;

	// Use this for initialization
	void Start () {
        nearbyObjects = new List<GameObject>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void populateGameObjectList()
    {
        var allObjects = FindObjectsOfType<GameObject>();
        foreach (GameObject each in allObjects)
        {
            nearbyObjects.Add(each);
        }
    }

}
