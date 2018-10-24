using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class WIMController : MonoBehaviour {

	void Awake() {
		WorldInMiniature wim = GetComponent<WorldInMiniature>();
		if(wim.controllerLeft != null && wim.controllerRight != null
			&& wim.cameraHead != null) {
			return;
		}

		// Locates the camera rig and its child controllers
		SteamVR_ControllerManager CameraRigObject = FindObjectOfType<SteamVR_ControllerManager>();
		GameObject leftController = CameraRigObject.left;
		GameObject rightController = CameraRigObject.right;

		wim.controllerLeft = leftController;
		wim.controllerRight = rightController;
		wim.cameraHead = FindObjectOfType<SteamVR_Camera>().gameObject;
	}
}
