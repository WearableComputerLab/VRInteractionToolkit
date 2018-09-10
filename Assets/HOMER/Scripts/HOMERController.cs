using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class HOMERController : MonoBehaviour {

	// Use this for initialization
	void Awake() {
		HOMER homer = GetComponent<HOMER>();
		if(homer.controllerRight != null || homer.controllerLeft != null) {
			// Only needs to set up once so will return otherwise
			return;
		}

		// Locates the camera rig and its child controllers
		SteamVR_ControllerManager CameraRigObject;
		if((CameraRigObject = FindObjectOfType<SteamVR_ControllerManager>()) != null) {
			homer.controllerRight = CameraRigObject.right;
        	homer.controllerLeft = CameraRigObject.left;
		}	
	}
}
