using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SphereCastingController : MonoBehaviour {

	void Awake() {
		SphereCasting sphere = GetComponent<SphereCasting>();
		if(sphere.controllerRight != null || sphere.controllerLeft != null) {
			// Only needs to set up once so will return otherwise
			return;
		}

		// Locates the camera rig and its child controllers
		SteamVR_ControllerManager CameraRigObject;
		if((CameraRigObject = FindObjectOfType<SteamVR_ControllerManager>()) != null) {
			sphere.controllerRight = CameraRigObject.right;
        	sphere.controllerLeft = CameraRigObject.left;
		}		
	}
}
