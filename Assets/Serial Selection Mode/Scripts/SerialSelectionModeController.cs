using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SerialSelectionModeController : MonoBehaviour {

	// Use this for initialization
	void Awake() {
		SerialSelectionMode serial = GetComponent<SerialSelectionMode>();
		if(serial.controllerRight != null || serial.controllerLeft != null) {
			// Only needs to set up once so will return otherwise
			return;
		}

		// Locates the camera rig and its child controllers
		SteamVR_ControllerManager CameraRigObject;
		if((CameraRigObject = FindObjectOfType<SteamVR_ControllerManager>()) != null) {
			serial.controllerRight = CameraRigObject.right;
        	serial.controllerLeft = CameraRigObject.left;
		}		
	}
}
