using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[ExecuteInEditMode]
public class SimpleVirtualHandController : MonoBehaviour {

    void Awake()
    {

        // Controller only ever needs to be setup once
        SimpleVirtualHand virtualhand = GetComponent<SimpleVirtualHand>();
        if(virtualhand.controllerLeft != null && virtualhand.controllerRight != null) {
            return;
        }

        // Locates the camera rig and its child controllers
        SteamVR_ControllerManager CameraRigObject = FindObjectOfType<SteamVR_ControllerManager>();
        GameObject leftController = CameraRigObject.left;
        GameObject rightController = CameraRigObject.right;

        virtualhand.controllerLeft = leftController;
        virtualhand.controllerRight = rightController;

    }

}
