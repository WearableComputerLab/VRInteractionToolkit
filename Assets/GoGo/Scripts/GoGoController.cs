using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[ExecuteInEditMode]
public class GoGoController : MonoBehaviour {
	public GameObject leftHannd;
	public GameObject rightHand;

	private GameObject leftController;
	private GameObject rightController;

	void Awake()
    {
        // If the controllers are null will try to set everything up. Otherwise will run.
		if(leftController == null && rightController == null) {
			// Locates the camera rig and its child controllers
			SteamVR_ControllerManager CameraRigObject = FindObjectOfType<SteamVR_ControllerManager>();
			leftController = CameraRigObject.left;
			rightController = CameraRigObject.right;
			
			// Adding variables to shadow scripts
			GoGoShadow shadowLeft;
			if ((shadowLeft = leftHannd.GetComponent<GoGoShadow>())!= null) {
				shadowLeft.trackedObj = leftController.GetComponent<SteamVR_TrackedObject>();
				shadowLeft.theController = leftController;
				shadowLeft.theModel = leftController.transform.GetChild(0).gameObject;
				shadowLeft.cameraRig = CameraRigObject.gameObject;
			}
			GoGoShadow shadowRight;
			if ((shadowRight = rightHand.GetComponent<GoGoShadow>())!= null) {
				shadowRight.trackedObj = rightController.GetComponent<SteamVR_TrackedObject>();
				shadowRight.theController = rightController;
				shadowRight.theModel = rightController.transform.GetChild(0).gameObject;
				shadowRight.cameraRig = CameraRigObject.gameObject;
			}

			// Adding variables to interaction scripts
			GrabObject grabLeft;
			if ((grabLeft = leftHannd.GetComponent<GrabObject>())!= null) {
				grabLeft.trackedObj = leftController.GetComponent<SteamVR_TrackedObject>();
			}
			GrabObject grabRight;
			if ((grabRight = rightHand.GetComponent<GrabObject>())!= null) {
				grabRight.trackedObj = rightController.GetComponent<SteamVR_TrackedObject>();
			}
		}
    }
}
