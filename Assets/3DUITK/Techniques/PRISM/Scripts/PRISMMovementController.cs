using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

[ExecuteInEditMode]
public class PRISMMovementController : MonoBehaviour {

    public PRISMMovement left;
    public PRISMMovement right;


    // Use this for initialization
    void Start() {
        GameObject leftController = null, rightController = null;
#if SteamVR_Legacy
        if (left.trackedObj == null && right.trackedObj == null) {
            // Locates the camera rig and its child controllers
            SteamVR_ControllerManager CameraRigObject = FindObjectOfType<SteamVR_ControllerManager>();
            leftController = CameraRigObject.left;
            rightController = CameraRigObject.right;

            left.trackedObj = leftController.GetComponent<SteamVR_TrackedObject>();
            right.trackedObj = rightController.GetComponent<SteamVR_TrackedObject>();
        }
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
        left.trackedObj = leftController.GetComponent<SteamVR_Behaviour_Pose>();
        right.trackedObj = rightController.GetComponent<SteamVR_Behaviour_Pose>();
#endif
        if (Application.isPlaying) {
            left.transform.parent = left.trackedObj.transform;
            right.transform.parent = right.trackedObj.transform;
        }
    }
}
