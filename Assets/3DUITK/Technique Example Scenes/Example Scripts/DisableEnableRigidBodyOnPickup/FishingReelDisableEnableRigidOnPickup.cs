using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishingReelDisableEnableRigidOnPickup : MonoBehaviour {
	public FishingReel reel;
	// Use this for initialization
	void Start () {
		reel.selectedObject.AddListener(setRigidKinematic);
		reel.droppedObject.AddListener(setRigidNotKinematic);
	}
	


	void setRigidKinematic() {
		if(reel.lastSelectedObject == this.gameObject) {
			this.GetComponent<Rigidbody>().isKinematic = true;
		}
		
	}

	void setRigidNotKinematic() {
		if(reel.lastSelectedObject == this.gameObject) {
			this.GetComponent<Rigidbody>().isKinematic = false;
		}
	}
}
