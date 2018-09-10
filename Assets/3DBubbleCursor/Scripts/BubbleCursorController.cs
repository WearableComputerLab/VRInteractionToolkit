using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class BubbleCursorController : MonoBehaviour {

	void Awake() 
	{		
		BubbleCursor bubble = this.GetComponent<BubbleCursor>();
		if(bubble == null || bubble.cameraHead != null) {
			// Only needs to set up once so will return otherwise
			return;
		}

		// Locates the camera rig and its child controllers
		SteamVR_ControllerManager CameraRigObject;
		if((CameraRigObject= FindObjectOfType<SteamVR_ControllerManager>()) != null) {
			bubble.controllerRight = CameraRigObject.right;
        	bubble.controllerLeft = CameraRigObject.left;
			bubble.cameraHead = FindObjectOfType<SteamVR_Camera>().gameObject;
		}			
	}
}
