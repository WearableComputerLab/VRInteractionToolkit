using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SpindleController : MonoBehaviour {

	private enum SelectionController {
        LeftController,
        RightController
    } 

	SelectionController selectionController = SelectionController.RightController;

	// Use this for initialization
	void Awake() {
		SteamVR_ControllerManager CameraRigObject = FindObjectOfType<SteamVR_ControllerManager>();
		GameObject leftController = CameraRigObject.left;
		GameObject rightController = CameraRigObject.right;

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
	}
}
