using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

[ExecuteInEditMode]
public class GoGoController : MonoBehaviour {
	public GameObject leftHannd;
	public GameObject rightHand;

	private GameObject leftController;
	private GameObject rightController;

	void Awake()
    {
        // If the controllers are null will try to set everything up. Otherwise will run.
		if(leftController == null && rightController == null) {
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
            // Adding variables to shadow scripts
            GoGoShadow shadowLeft;
			if ((shadowLeft = leftHannd.GetComponent<GoGoShadow>())!= null) {
#if SteamVR_Legacy
                shadowLeft.trackedObj = leftController.GetComponent<SteamVR_TrackedObject>();
#elif SteamVR_2
                shadowLeft.trackedObj = leftController.GetComponent<SteamVR_Behaviour_Pose>();
#endif

                shadowLeft.theController = leftController;
				shadowLeft.theModel = leftController.transform.GetChild(0).gameObject;
				shadowLeft.cameraRig = leftController.transform.parent.gameObject;
			}
			GoGoShadow shadowRight;
			if ((shadowRight = rightHand.GetComponent<GoGoShadow>())!= null) {
#if SteamVR_Legacy
                shadowRight.trackedObj = rightController.GetComponent<SteamVR_TrackedObject>();
#elif SteamVR_2
                shadowRight.trackedObj = rightController.GetComponent<SteamVR_Behaviour_Pose>();
#endif

                shadowRight.theController = rightController;
				shadowRight.theModel = rightController.transform.GetChild(0).gameObject;
				shadowRight.cameraRig = rightController.transform.parent.gameObject;
            }

			// Adding variables to interaction scripts
			GrabObject grabLeft;
			if ((grabLeft = leftHannd.GetComponent<GrabObject>())!= null) {
#if SteamVR_Legacy
                grabLeft.trackedObj = leftController.GetComponent<SteamVR_TrackedObject>();
#elif SteamVR_2
                grabLeft.trackedObj = leftController.GetComponent<SteamVR_Behaviour_Pose>();
#endif

            }
            GrabObject grabRight;
			if ((grabRight = rightHand.GetComponent<GrabObject>())!= null) {
#if SteamVR_Legacy
                grabRight.trackedObj = rightController.GetComponent<SteamVR_TrackedObject>();
#elif SteamVR_2
                grabRight.trackedObj = rightController.GetComponent<SteamVR_Behaviour_Pose>();
#endif

            }
        }
    }
}
