using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanResetSigns : MonoBehaviour {

	// Use this for initialization
	void Start () {
		if(this.GetComponent<Rigidbody>() == null) {
			Rigidbody body = this.gameObject.AddComponent<Rigidbody>();
			body.isKinematic = true;
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
