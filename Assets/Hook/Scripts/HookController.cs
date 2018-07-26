using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class HookController : MonoBehaviour {

	public GameObject leftController;
	public GameObject rightController;

	// Use this for initialization
	void Awake() {
		// Locates the camera rig and its child controllers
        SteamVR_ControllerManager CameraRigObject = FindObjectOfType<SteamVR_ControllerManager>();
        leftController = CameraRigObject.left;
        rightController = CameraRigObject.right;

		if(leftController != null && leftController.GetComponent<Hook>() == null) {
			Hook hookComponent = leftController.AddComponent<Hook>() as Hook;
			hookComponent.trackedObj = leftController.GetComponent<SteamVR_TrackedObject>();
		} 
		
		if(rightController != null && rightController.GetComponent<Hook>() == null) {
			Hook hookComponent = rightController.AddComponent<Hook>() as Hook;
			hookComponent.trackedObj = rightController.GetComponent<SteamVR_TrackedObject>();
		}
	}
}
