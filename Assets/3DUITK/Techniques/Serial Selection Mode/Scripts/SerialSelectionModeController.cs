using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

[ExecuteInEditMode]
public class SerialSelectionModeController : MonoBehaviour {

    // Use this for initialization
    void Awake() {
        SerialSelectionMode serial = GetComponent<SerialSelectionMode>();
        if (serial.controllerRight != null || serial.controllerLeft != null) {
            // Only needs to set up once so will return otherwise
            return;
        }

        GameObject leftController = null, rightController = null;
#if SteamVR_Legacy
        SteamVR_ControllerManager CameraRigObject = FindObjectOfType<SteamVR_ControllerManager>();
        if ((CameraRigObject = FindObjectOfType<SteamVR_ControllerManager>()) != null) {
            leftController = CameraRigObject.left;
            rightController = CameraRigObject.right;
            serial.controllerRight = rightController;
            serial.controllerLeft = leftController;
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
        serial.controllerRight = rightController;
        serial.controllerLeft = leftController;
#endif
    }

}
