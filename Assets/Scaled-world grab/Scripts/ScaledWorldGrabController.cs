using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ScaledWorldGrabController : MonoBehaviour {

	void Awake() {
		ScaledWorldGrab grab = GetComponent<ScaledWorldGrab>();
		if(grab.controllerRight != null || grab.controllerLeft != null) {
			// Only needs to set up once so will return otherwise
			return;
		}

		// Locates the camera rig and its child controllers
		SteamVR_ControllerManager CameraRigObject;
		if((CameraRigObject = FindObjectOfType<SteamVR_ControllerManager>()) != null) {
			grab.controllerRight = CameraRigObject.right;
        	grab.controllerLeft = CameraRigObject.left;
			grab.cameraHead = FindObjectOfType<SteamVR_Camera>().gameObject;
			grab.cameraRig = CameraRigObject.gameObject;
		}		
	}
}
