using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

[ExecuteInEditMode]
public class HOMERController : MonoBehaviour {

	// Use this for initialization
	void Awake() {
		HOMER homer = GetComponent<HOMER>();
		if(homer.controllerRight != null || homer.controllerLeft != null) {
			// Only needs to set up once so will return otherwise
			return;
		}
        GameObject leftController = null, rightController = null;
#if SteamVR_Legacy
        // Locates the camera rig and its child controllers
        SteamVR_ControllerManager CameraRigObject = FindObjectOfType<SteamVR_ControllerManager>();
        leftController = CameraRigObject.left;
        rightController = CameraRigObject.right;

        homer.controllerLeft = leftController;
        homer.controllerRight = rightController;
#elif SteamVR_2
        SteamVR_Behaviour_Pose[] controllers = FindObjectsOfType<SteamVR_Behaviour_Pose>();
        if (controllers.Length > 1) {
            leftController = controllers[0].inputSource.ToString() == "LeftHand" ? controllers[0].gameObject : controllers[1].inputSource.ToString() == "LeftHand" ? controllers[1].gameObject : null;
            rightController = controllers[0].inputSource.ToString() == "RightHand" ? controllers[0].gameObject : controllers[1].inputSource.ToString() == "RightHand" ? controllers[1].gameObject : null;
        } else if (controllers.Length == 1) {
            leftController = controllers[0].inputSource.ToString() == "LeftHand" ? controllers[0].gameObject : null;
            rightController = controllers[0].inputSource.ToString() == "RightHand" ? controllers[0].gameObject : null;
        } else {
            return;
        }
        homer.controllerLeft = leftController;
        homer.controllerRight = rightController;
#endif
    }
}
