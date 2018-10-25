using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanResetSigns : MonoBehaviour {

	// Use this for initialization
	void Start () {
		if(this.gameObject.GetComponent<Collider>() == null) {
			BoxCollider box = this.gameObject.AddComponent<BoxCollider>();
			box.isTrigger = true;
			box.transform.localScale= new Vector3(0.1f, 0.1f, 0.1f);
		}
		if(this.GetComponent<Rigidbody>() == null) {
			Rigidbody body = this.gameObject.AddComponent<Rigidbody>();
			body.isKinematic = true;
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
