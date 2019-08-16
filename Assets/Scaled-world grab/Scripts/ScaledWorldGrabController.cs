using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

[ExecuteInEditMode]
public class ScaledWorldGrabController : MonoBehaviour {

    void Awake() {
        ScaledWorldGrab grab = GetComponent<ScaledWorldGrab>();
        if (grab.controllerRight != null || grab.controllerLeft != null) {
            // Only needs to set up once so will return otherwise
            return;
        }

        GameObject leftController = null, rightController = null, head = null, rig = null;
#if SteamVR_Legacy
        SteamVR_ControllerManager CameraRigObject = FindObjectOfType<SteamVR_ControllerManager>();
        if ((CameraRigObject = FindObjectOfType<SteamVR_ControllerManager>()) != null) {
            leftController = CameraRigObject.left;
            rightController = CameraRigObject.right;
            grab.controllerRight = rightController;
            grab.controllerLeft = leftController;
            grab.cameraHead = FindObjectOfType<SteamVR_Camera>().gameObject;
            grab.cameraRig = CameraRigObject.gameObject;
        }
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
            head = controllers[0].transform.parent.GetComponentInChildren<Camera>().gameObject;
            rig = controllers[0].transform.parent.gameObject;
        }
        grab.controllerRight = rightController;
        grab.controllerLeft = leftController;
        grab.cameraHead = head;
        grab.cameraRig = rig;
#endif
    }
}

