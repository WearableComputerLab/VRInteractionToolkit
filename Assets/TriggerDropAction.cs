using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerDropAction : MonoBehaviour {


    void OnTriggerEnter(Collider col) {
        if (this.transform.name == col.transform.name) {
            col.transform.position = this.transform.position;
            col.transform.rotation = this.transform.rotation;
        }
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
