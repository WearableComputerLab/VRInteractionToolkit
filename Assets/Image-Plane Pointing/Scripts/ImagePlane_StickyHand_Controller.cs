using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ImagePlane_StickyHand_Controller : MonoBehaviour {

	void Awake() {
		ImagePlane_StickyHand hands = GetComponent<ImagePlane_StickyHand>();
        if(hands.controllerLeft != null && hands.controllerRight != null) {
            return;
        }

        // Locates the camera rig and its child controllers
        SteamVR_ControllerManager CameraRigObject = FindObjectOfType<SteamVR_ControllerManager>();
        GameObject leftController = CameraRigObject.left;
        GameObject rightController = CameraRigObject.right;

		hands.controllerLeft = leftController;
		hands.controllerRight = rightController;
		hands.cameraRig = CameraRigObject.gameObject;
		hands.cameraHead = FindObjectOfType<SteamVR_Camera>().gameObject;
	}
}
