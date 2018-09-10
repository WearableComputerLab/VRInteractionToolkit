using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class DepthRayController : MonoBehaviour {

	// Use this for initialization
	void Awake() {
		DepthRay depth = GetComponent<DepthRay>();
		if(depth.controllerRight != null || depth.controllerLeft != null) {
			// Only needs to set up once so will return otherwise
			return;
		}

		// Locates the camera rig and its child controllers
		SteamVR_ControllerManager CameraRigObject;
		if((CameraRigObject = FindObjectOfType<SteamVR_ControllerManager>()) != null) {
			depth.controllerRight = CameraRigObject.right;
        	depth.controllerLeft = CameraRigObject.left;
		}		
	}
}
