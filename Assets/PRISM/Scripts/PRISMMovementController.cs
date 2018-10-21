using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PRISMMovementController : MonoBehaviour {

	public PRISMMovement left;
	public PRISMMovement right;


	// Use this for initialization
	void Start () {
		if(left.trackedObj == null && right.trackedObj == null) {
				// Locates the camera rig and its child controllers
			SteamVR_ControllerManager CameraRigObject = FindObjectOfType<SteamVR_ControllerManager>();
			GameObject leftController = CameraRigObject.left;
			GameObject rightController = CameraRigObject.right;

			left.trackedObj = leftController.GetComponent<SteamVR_TrackedObject>();
			right.trackedObj = rightController.GetComponent<SteamVR_TrackedObject>();
			
		}

		if(Application.isPlaying) {
			left.transform.parent = left.trackedObj.transform;
			right.transform.parent = right.trackedObj.transform;
		}
	}	
	
	// Update is called once per frame
	void Update () {
		
	}
}
