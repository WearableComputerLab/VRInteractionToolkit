using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[ExecuteInEditMode]
public class FlashlightController : MonoBehaviour {

	public GameObject leftController;
	public GameObject rightController;
	public GameObject leftFlashlight;
	public GameObject rightFlashlight;
	
	// Runs in the editor
	void Awake() 
	{
		// If the controllers are null will try to set everything up. Otherwise will run.
		if(leftController == null && rightController == null) {
			// Locates the camera rig and its child controllers
			SteamVR_ControllerManager CameraRigObject = FindObjectOfType<SteamVR_ControllerManager>();
			leftController = CameraRigObject.left;
			rightController = CameraRigObject.right;
					
			if ((leftFlashlight.GetComponent<Flashlight>())!= null) {
				leftFlashlight.GetComponent<Flashlight>().trackedObj = leftController.GetComponent<SteamVR_TrackedObject>();
				leftFlashlight.GetComponent<Flashlight>().objectAttachedTo = leftController;

				FlashlightSelection leftSelection;
				if((leftSelection = leftFlashlight.GetComponent<FlashlightSelection>()) != null) {
					leftSelection.theController = leftController.GetComponent<SteamVR_TrackedObject>();
				}
			}
			if ((rightFlashlight.GetComponent<Flashlight>())!= null) {
				rightFlashlight.GetComponent<Flashlight>().trackedObj = rightController.GetComponent<SteamVR_TrackedObject>();
				rightFlashlight.GetComponent<Flashlight>().objectAttachedTo = rightController;
				
				FlashlightSelection rightSelection;
				if((rightSelection = rightFlashlight.GetComponent<FlashlightSelection>()) != null) {
					rightSelection.theController = rightController.GetComponent<SteamVR_TrackedObject>();
				}
			}
		}	
	}
}
