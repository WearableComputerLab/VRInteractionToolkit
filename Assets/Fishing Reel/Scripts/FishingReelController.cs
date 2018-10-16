using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[ExecuteInEditMode]
public class FishingReelController : MonoBehaviour {

    void Awake()
    {

        // Controller only ever needs to be setup once
        FishingReel reel = GetComponent<FishingReel>();
        if(reel.controllerLeft != null && reel.controllerRight != null) {
            return;
        }

        // Locates the camera rig and its child controllers
        SteamVR_ControllerManager CameraRigObject = FindObjectOfType<SteamVR_ControllerManager>();
        GameObject leftController = CameraRigObject.left;
        GameObject rightController = CameraRigObject.right;

		reel.controllerLeft = leftController;
		reel.controllerRight = rightController;

    }

}
