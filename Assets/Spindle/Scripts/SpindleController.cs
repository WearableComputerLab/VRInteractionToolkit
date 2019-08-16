using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

[ExecuteInEditMode]
public class SpindleController : MonoBehaviour {

	private enum SelectionController {
        LeftController,
        RightController
    } 

	SelectionController selectionController = SelectionController.RightController;

	// Use this for initialization
	void Awake() {
        GameObject leftController = null, rightController = null;
#if SteamVR_Legacy
        SteamVR_ControllerManager CameraRigObject = FindObjectOfType<SteamVR_ControllerManager>();
		leftController = CameraRigObject.left;
		rightController = CameraRigObject.right;

		Spindle spindleComponent = this.GetComponent<Spindle>();

		if(spindleComponent.trackedObj1 == null || spindleComponent.trackedObj2 == null) {
			print("here");
			SteamVR_TrackedObject trackedL = leftController.GetComponent<SteamVR_TrackedObject>();
			SteamVR_TrackedObject trackedR = rightController.GetComponent<SteamVR_TrackedObject>();
			spindleComponent.trackedObj1 = trackedL;
			spindleComponent.trackedObj2 = trackedR;

			SpindleInteractor interactionPointComponent = this.GetComponentInChildren<SpindleInteractor>();
			interactionPointComponent.trackedObj1 = trackedL;
			interactionPointComponent.trackedObj2 = trackedR;			
		}
#elif SteamVR_2
        SteamVR_Behaviour_Pose[] controllers = FindObjectsOfType<SteamVR_Behaviour_Pose>();
        Spindle spindleComponent = this.GetComponent<Spindle>();
        if (controllers.Length > 1) {
            leftController = controllers[0].inputSource.ToString() == "LeftHand" ? controllers[0].gameObject : controllers[1].inputSource.ToString() == "LeftHand" ? controllers[1].gameObject : null;
            rightController = controllers[0].inputSource.ToString() == "RightHand" ? controllers[0].gameObject : controllers[1].inputSource.ToString() == "RightHand" ? controllers[1].gameObject : null;
        } else {
            leftController = controllers[0].inputSource.ToString() == "LeftHand" ? controllers[0].gameObject : null;
            rightController = controllers[0].inputSource.ToString() == "RightHand" ? controllers[0].gameObject : null;
        }
        if(spindleComponent.trackedObj1 == null || spindleComponent.trackedObj2 == null) {
        	SteamVR_Behaviour_Pose trackedL = leftController.GetComponent<SteamVR_Behaviour_Pose>();
			SteamVR_Behaviour_Pose trackedR = rightController.GetComponent<SteamVR_Behaviour_Pose>();
			spindleComponent.trackedObj1 = trackedL;
			spindleComponent.trackedObj2 = trackedR;

			SpindleInteractor interactionPointComponent = this.GetComponentInChildren<SpindleInteractor>();
			interactionPointComponent.trackedObj1 = trackedL;
			interactionPointComponent.trackedObj2 = trackedR;
        }
#endif



    }
}
