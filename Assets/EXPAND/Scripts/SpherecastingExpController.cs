using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SpherecastingExpController : MonoBehaviour {

	void Awake() {
		SphereCastingExp expand = GetComponent<SphereCastingExp>();
		if(expand.controllerRight != null || expand.controllerLeft != null) {
			// Only needs to set up once so will return otherwise
			return;
		}

		// Locates the camera rig and its child controllers
		SteamVR_ControllerManager CameraRigObject;
		if((CameraRigObject = FindObjectOfType<SteamVR_ControllerManager>()) != null) {
			expand.controllerRight = CameraRigObject.right;
        	expand.controllerLeft = CameraRigObject.left;
		}		
	}
}
