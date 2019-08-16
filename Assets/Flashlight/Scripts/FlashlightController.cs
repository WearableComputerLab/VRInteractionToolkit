using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

[ExecuteInEditMode]
public class FlashlightController : MonoBehaviour {

	public GameObject leftController;
	public GameObject rightController;
	public GameObject leftFlashlight;
	public GameObject rightFlashlight;
	
	// Runs in the editor
	void Awake() 
	{
		// If the controllers are null will try to set everything up. Otherwise will run.
		if(leftController == null && rightController == null) {
            // Locates the camera rig and its child controllers
#if SteamVR_Legacy
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


            if ((leftFlashlight.GetComponent<Flashlight>())!= null) {
#if SteamVR_Legacy
                leftFlashlight.GetComponent<Flashlight>().trackedObj = leftController.GetComponent<SteamVR_TrackedObject>();
#elif SteamVR_2
                leftFlashlight.GetComponent<Flashlight>().trackedObj = leftController.GetComponent<SteamVR_Behaviour_Pose>();
#endif
                leftFlashlight.GetComponent<Flashlight>().objectAttachedTo = leftController;

				FlashlightSelection leftSelection;
				if((leftSelection = leftFlashlight.GetComponent<FlashlightSelection>()) != null) {
#if SteamVR_Legacy
                leftSelection.theController = leftController.GetComponent<SteamVR_TrackedObject>();
#elif SteamVR_2
                leftSelection.theController = leftController.GetComponent<SteamVR_Behaviour_Pose>();
#endif
                }
            }
			if ((rightFlashlight.GetComponent<Flashlight>())!= null) {
#if SteamVR_Legacy
                rightFlashlight.GetComponent<Flashlight>().trackedObj = rightController.GetComponent<SteamVR_TrackedObject>();
#elif SteamVR_2
                rightFlashlight.GetComponent<Flashlight>().trackedObj = rightController.GetComponent<SteamVR_Behaviour_Pose>();
#endif
                rightFlashlight.GetComponent<Flashlight>().objectAttachedTo = rightController;
				
				FlashlightSelection rightSelection;
				if((rightSelection = rightFlashlight.GetComponent<FlashlightSelection>()) != null) {
#if SteamVR_Legacy
                rightSelection.theController = rightController.GetComponent<SteamVR_TrackedObject>();
#elif SteamVR_2
                rightSelection.theController = rightController.GetComponent<SteamVR_Behaviour_Pose>();
#endif
                }
            }
		}	
	}
}
