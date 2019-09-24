using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DepthRayDisableEnableRigidOnPickup : MonoBehaviour {

	public DepthRay ray;
	// Use this for initialization
	void Start () {
		ray.selectedObject.AddListener(setRigidKinematic);
		ray.droppedObject.AddListener(setRigidNotKinematic);
	}
	


	void setRigidKinematic() {
		if(ray.currentClosestObject == this.gameObject) {
			this.GetComponent<Rigidbody>().isKinematic = true;
		}
		
	}

	void setRigidNotKinematic() {
		if(ray.currentClosestObject == this.gameObject) {
			this.GetComponent<Rigidbody>().isKinematic = false;
		}
	}
}
