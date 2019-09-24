using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

[ExecuteInEditMode]
public class HookController : MonoBehaviour {

	public GameObject leftController;
	public GameObject rightController;
	public Hook leftHook;
	public Hook rightHook;


	// Use this for initialization
	void Awake() {
		if(leftHook.trackedObj == null && rightHook.trackedObj == null){
            // Locates the camera rig and its child controllers
#if SteamVR_Legacy
			SteamVR_ControllerManager CameraRigObject = FindObjectOfType<SteamVR_ControllerManager>();
			leftController = CameraRigObject.left;
			rightController = CameraRigObject.right;

            leftHook.trackedObj = leftController.GetComponent<SteamVR_TrackedObject>();
			rightHook.trackedObj = rightController.GetComponent<SteamVR_TrackedObject>();
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

            leftHook.trackedObj = leftController.GetComponent<SteamVR_Behaviour_Pose>();
		    rightHook.trackedObj = rightController.GetComponent<SteamVR_Behaviour_Pose>();

#endif

        }
    }
}
