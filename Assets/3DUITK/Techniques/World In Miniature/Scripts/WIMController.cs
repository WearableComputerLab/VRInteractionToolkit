using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

[ExecuteInEditMode]
public class WIMController : MonoBehaviour {

    void Awake() {
        WorldInMiniature wim = GetComponent<WorldInMiniature>();
        if (wim.controllerLeft != null && wim.controllerRight != null
            && wim.cameraHead != null) {
            return;
        }
        GameObject leftController = null, rightController = null, cameraHead = null;
#if SteamVR_Legacy
        // Locates the camera rig and its child controllers
        SteamVR_ControllerManager CameraRigObject = FindObjectOfType<SteamVR_ControllerManager>();
        leftController = CameraRigObject.left;
        rightController = CameraRigObject.right;

        wim.controllerLeft = leftController;
        wim.controllerRight = rightController;
        wim.cameraHead = FindObjectOfType<SteamVR_Camera>().gameObject;
#elif SteamVR_2
    SteamVR_Behaviour_Pose[] controllers = FindObjectsOfType<SteamVR_Behaviour_Pose>();
        if (controllers.Length > 1) {
            leftController = controllers[0].inputSource.ToString() == "LeftHand" ? controllers[0].gameObject : controllers[1].inputSource.ToString() == "LeftHand" ? controllers[1].gameObject : null;
            rightController = controllers[0].inputSource.ToString() == "RightHand" ? controllers[0].gameObject : controllers[1].inputSource.ToString() == "RightHand" ? controllers[1].gameObject : null;
        } else {
            leftController = controllers[0].inputSource.ToString() == "LeftHand" ? controllers[0].gameObject : null;
            rightController = controllers[0].inputSource.ToString() == "RightHand" ? controllers[0].gameObject : null;
        }
        if (controllers[0] != null) {
            cameraHead = controllers[0].transform.parent.GetComponentInChildren<Camera>().gameObject;
        }
        wim.controllerLeft = leftController;
        wim.controllerRight = rightController;
        wim.cameraHead = cameraHead;
#endif
    }
}
