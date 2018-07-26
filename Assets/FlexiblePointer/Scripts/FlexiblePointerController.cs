using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class FlexiblePointerController : MonoBehaviour {
	void Awake() {
		// Check if already set up
		FlexiblePointer flexiblePointerComponent = this.GetComponent<FlexiblePointer>();
		if(flexiblePointerComponent.trackedObj1 != null && flexiblePointerComponent.trackedObj2 != null) {
			// Controllers already set so return
			return;
		}
		// Locates the camera rig and its child controllers
        SteamVR_ControllerManager CameraRigObject = FindObjectOfType<SteamVR_ControllerManager>();
        GameObject leftController = CameraRigObject.left;
        GameObject rightController = CameraRigObject.right;
		
		// Setting controllers for flexible pointer.		
		if(flexiblePointerComponent != null) {
			SteamVR_TrackedObject leftTracked = leftController.GetComponent<SteamVR_TrackedObject>();
			SteamVR_TrackedObject rightTracked = rightController.GetComponent<SteamVR_TrackedObject>();
			if(flexiblePointerComponent.trackedObj1 == null && leftTracked != null) {				
				flexiblePointerComponent.trackedObj1 = leftTracked;
			}
			if(flexiblePointerComponent.trackedObj2 == null && rightTracked != null) {
				flexiblePointerComponent.trackedObj2 = rightTracked;
			}			
		}
	}
}
