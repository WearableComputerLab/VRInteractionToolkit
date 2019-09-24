using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class ScaledWorldDisableEnableRigidOnPickup : MonoBehaviour {


	public ControllerColliderSWG grab;
	// Use this for initialization
	void Awake () {
		grab.selectedObject.AddListener(setRigidKinematic);
		grab.droppedObject.AddListener(setRigidNotKinematic);
	}
	


	void setRigidKinematic() {
		if(grab.scaleSelected == this.gameObject) {
			this.GetComponent<Rigidbody>().isKinematic = true;
		}
		
	}

	void setRigidNotKinematic() {
		if(grab.scaleSelected == this.gameObject) {
			this.GetComponent<Rigidbody>().isKinematic = false;
		}
	}
}
