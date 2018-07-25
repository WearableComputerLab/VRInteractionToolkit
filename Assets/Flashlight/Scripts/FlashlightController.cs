using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[ExecuteInEditMode]
public class FlashlightController : MonoBehaviour {

	public GameObject leftController;
	public GameObject rightController;
	public GameObject leftFlashlight;
	public GameObject rightFlashlight;

	void enableFlashlights() {
		// CODE BELOW RUNS AT START OF GAME
		// Attaches the flashlights to the controllers to work
		// turns on flashlight
		leftFlashlight.SetActive(true);
		// turns on flashlight
		rightFlashlight.SetActive(true);
		 
	}
	bool ran = false;

	
	// Runs in the editor
	void Awake() 
	{
		// If the controllers are null will try to set everything up. Otherwise will run.
		if(leftController == null && rightController == null) {
			// Locates the camera rig and its child controllers
			SteamVR_ControllerManager CameraRigObject = FindObjectOfType<SteamVR_ControllerManager>();
			leftController = CameraRigObject.left;
			rightController = CameraRigObject.right;
			// Get child flashlight controllers and set their component info (if corresponding controllers exist)
			foreach (Transform child in transform)
			{	
				print("take");
				if(child.name == "LeftFlashlight" && leftController != null)
				{
					attatchComponents(leftController, child.gameObject);
					leftFlashlight = child.gameObject;
				}
				if (child.name == "RightFlashlight" && rightController != null)
				{
					print("here");
					attatchComponents(rightController, child.gameObject);
					rightFlashlight = child.gameObject;
				}
			}
			if(leftFlashlight != null) {leftFlashlight.transform.parent = leftController.transform;}
			if(rightFlashlight != null) {rightFlashlight.transform.parent = rightController.transform;}
			return;
		}
		enableFlashlights();	
	}

	private void attatchComponents(GameObject controller, GameObject flashlightObject) {
		FlashlightSelection comp = flashlightObject.GetComponent<FlashlightSelection>();
		Flashlight flashComp = flashlightObject.GetComponent<Flashlight>();

		// Setting up required components
		SteamVR_TrackedObject controllerComponent = controller.GetComponent<SteamVR_TrackedObject>();
		comp.theController = controllerComponent;
		flashComp.objectAttachedTo = controller;
		
	}
}
