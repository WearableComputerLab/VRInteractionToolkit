using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllersWithoutHeadSet : MonoBehaviour {

    // aim controller in direction you want camera to face and press on the grip. You will face that way and then can use controllers
    // looking at the game window without having to put headset on.

	private SteamVR_TrackedObject trackedObj1;
	private SteamVR_TrackedObject trackedObj2;
	private SteamVR_Camera camera;
	public Camera devCamera;
	private SteamVR_Controller.Device controller;
	private SteamVR_ControllerManager manager;

	// Use this for initialization
	void Start () {
		manager = this.GetComponent<SteamVR_ControllerManager> ();	
		trackedObj1 = manager.left.GetComponent<SteamVR_TrackedObject> ();
		trackedObj2 = manager.right.GetComponent<SteamVR_TrackedObject> ();
		camera = manager.GetComponentInChildren<SteamVR_Camera> ();
		// Disables camera to use our camera
		camera.GetComponent<Camera>().enabled = false;
		controller = SteamVR_Controller.Input((int)trackedObj1.index);
        // Enabling prefab camera
        //gameObject.AddComponent(devCamera);
	}
	
	// Update is called once per frame
	void Update () {
		if (controller == null) {
			// Shoudlnt have to do it like this fix
			controller = SteamVR_Controller.Input((int)trackedObj1.index);
		} else if (controller.GetPressDown(SteamVR_Controller.ButtonMask.Grip))
		{
			Debug.Log(gameObject.name + " Grip Press");
			Vector3 betweenBothControllers = (trackedObj1.transform.position + trackedObj2.transform.position)/2f;
            devCamera.transform.forward = trackedObj1.transform.forward;

            float distanceCameraBack = 0.002f;

            // Setting camera to be back an estimated arms distance
            Vector3 directionOfTravel = trackedObj1.transform.forward * -1f; 
            Vector3 finalDirection = directionOfTravel + directionOfTravel.normalized * distanceCameraBack;
            Vector3 targetPosition = betweenBothControllers + finalDirection;

            devCamera.transform.position = targetPosition;
            devCamera.transform.forward = trackedObj1.transform.forward;
		}
	}
}
