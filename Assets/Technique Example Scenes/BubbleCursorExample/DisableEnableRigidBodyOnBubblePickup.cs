using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableEnableRigidBodyOnBubblePickup : MonoBehaviour {

	public BubbleCursor bubble;
	// Use this for initialization
	void Start () {
		bubble.selectedObject.AddListener(setRigidKinematic);
		bubble.droppedObject.AddListener(setRigidNotKinematic);
	}
	


	void setRigidKinematic() {
		if(bubble.lastSelectedObject == this.gameObject) {
			this.GetComponent<Rigidbody>().isKinematic = true;
		}
		
	}

	void setRigidNotKinematic() {
		if(bubble.lastSelectedObject == this.gameObject) {
			this.GetComponent<Rigidbody>().isKinematic = false;
		}
	}
}
