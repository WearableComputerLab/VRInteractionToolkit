using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[ExecuteInEditMode]
public class RaycastingController : MonoBehaviour {

    void Awake()
    {

        // Controller only ever needs to be setup once
        Raycasting ray = GetComponent<Raycasting>();
        if(ray.controllerLeft != null && ray.controllerRight != null) {
            return;
        }

        // Locates the camera rig and its child controllers
        SteamVR_ControllerManager CameraRigObject = FindObjectOfType<SteamVR_ControllerManager>();
        GameObject leftController = CameraRigObject.left;
        GameObject rightController = CameraRigObject.right;

        ray.controllerLeft = leftController;
        ray.controllerRight = rightController;

    }

}
