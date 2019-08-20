using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

[ExecuteInEditMode]
public class FlexiblePointerController : MonoBehaviour {
    void Awake() {
        // Check if already set up
        FlexiblePointer flexiblePointerComponent = this.GetComponent<FlexiblePointer>();
        if (flexiblePointerComponent.trackedObjL != null && flexiblePointerComponent.trackedObjR != null) {
            // Controllers already set so return
            return;
        }
        GameObject leftController = null, rightController = null;
#if SteamVR_Legacy
        // Locates the camera rig and its child controllers
        SteamVR_ControllerManager CameraRigObject = FindObjectOfType<SteamVR_ControllerManager>();
        leftController = CameraRigObject.left;
        rightController = CameraRigObject.right;
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
#endif
        // Setting controllers for flexible pointer.		
        if (flexiblePointerComponent != null) {
#if SteamVR_Legacy
            SteamVR_TrackedObject leftTracked = leftController.GetComponent<SteamVR_TrackedObject>();
            SteamVR_TrackedObject rightTracked = rightController.GetComponent<SteamVR_TrackedObject>();
#elif SteamVR_2
            SteamVR_Behaviour_Pose leftTracked = leftController.GetComponent<SteamVR_Behaviour_Pose>();
            SteamVR_Behaviour_Pose rightTracked = rightController.GetComponent<SteamVR_Behaviour_Pose>();
#else
            GameObject leftTracked = leftController.gameObject;
            GameObject rightTracked = rightController.gameObject;
#endif
            if (flexiblePointerComponent.trackedObjL == null && leftTracked != null) {
                flexiblePointerComponent.trackedObjL = leftTracked;
            }
            if (flexiblePointerComponent.trackedObjR == null && rightTracked != null) {
                flexiblePointerComponent.trackedObjR = rightTracked;
            }
        }
    }
}
