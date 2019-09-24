using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

[ExecuteInEditMode]
public class ImagePlane_FramingHands_Controller : MonoBehaviour {


    void Awake() {

        // Controller only ever needs to be setup once
        ImagePlane_FramingHands hands = GetComponent<ImagePlane_FramingHands>();
        if (hands.controllerLeft != null && hands.controllerRight != null) {
            return;
        }
        GameObject leftController = null, rightController = null, head = null, rig = null;
#if SteamVR_Legacy
        SteamVR_ControllerManager CameraRigObject = FindObjectOfType<SteamVR_ControllerManager>();
        if ((CameraRigObject = FindObjectOfType<SteamVR_ControllerManager>()) != null) {
            leftController = CameraRigObject.left;
            rightController = CameraRigObject.right;
            hands.controllerRight = rightController;
            hands.controllerLeft = leftController;
            hands.cameraHead = FindObjectOfType<SteamVR_Camera>().gameObject;
            hands.cameraRig = CameraRigObject.gameObject;
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
        hands.controllerRight = rightController;
        hands.controllerLeft = leftController;
        hands.cameraHead = head;
        hands.cameraRig = rig;
#endif
    }
}
