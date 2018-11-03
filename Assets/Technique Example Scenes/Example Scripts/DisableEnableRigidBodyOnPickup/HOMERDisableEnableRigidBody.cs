using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HOMERDisableEnableRigidBody : MonoBehaviour {

	public HOMER homer;
	// Use this for initialization
	void Start () {
		homer.selectedObjectEvent.AddListener(setRigidKinematic);
		homer.droppedObject.AddListener(setRigidNotKinematic);
	}



	void setRigidKinematic() {
		if(homer.selectedObject == this.gameObject) {
			this.GetComponent<Rigidbody>().isKinematic = true;
		}

	}

	void setRigidNotKinematic() {
		if(homer.selectedObject == this.gameObject) {
			this.GetComponent<Rigidbody>().isKinematic = false;
		}
	}
}
