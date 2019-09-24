using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WIM_IDHandler : MonoBehaviour {

	public int ID = 0;

	public void addID(GameObject obj) {
		ID++;
		obj.AddComponent<ObjectID> ().ID = ID;
        if (obj.GetComponent<Rigidbody>() != null && obj.GetComponent<Rigidbody>().isKinematic == false) {
            obj.GetComponent<ObjectID>().movableObject = true;
        }
		print ("Added object ID:" + ID + " to object:" + obj.name);
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
