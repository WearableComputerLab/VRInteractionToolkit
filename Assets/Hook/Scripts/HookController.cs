using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class HookController : MonoBehaviour {

	public GameObject leftController;
	public GameObject rightController;
	public Hook leftHook;
	public Hook rightHook;


	// Use this for initialization
	void Awake() {
		if(leftHook.trackedObj == null && rightHook.trackedObj == null){
			// Locates the camera rig and its child controllers
			SteamVR_ControllerManager CameraRigObject = FindObjectOfType<SteamVR_ControllerManager>();
			leftController = CameraRigObject.left;
			rightController = CameraRigObject.right;

			leftHook.trackedObj = leftController.GetComponent<SteamVR_TrackedObject>();
			rightHook.trackedObj = rightController.GetComponent<SteamVR_TrackedObject>();
		}		
	}
}
