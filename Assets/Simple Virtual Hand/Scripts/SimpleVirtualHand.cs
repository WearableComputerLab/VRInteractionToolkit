using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SimpleVirtualHand : MonoBehaviour {

    /* Simple Virtual Hand implementation by Kieran May
     * University of South Australia
     * 
     * */
    public GameObject controllerCollider;
    public LayerMask interactionLayers;

    public GameObject controllerRight;
    public GameObject controllerLeft;

    internal SteamVR_TrackedObject trackedObj;
    internal SteamVR_Controller.Device controller;

    public enum InteractionType { Selection, Manipulation_Movement, Manipulation_Full };
    public InteractionType interacionType;

    public enum ControllerPicked { Left_Controller, Right_Controller };
    public ControllerPicked controllerPicked;

    internal bool objSelected = false;
    public GameObject selectedObject;

    Vector3 oldHeadScale;
    Vector3 oldCameraRigScale;

    internal bool objectGrabbed = false;
    public static int grabbedAmount;

    private GameObject entered;
    private void OnTriggerEnter(Collider col) {
        if(isInteractionlayer(col.gameObject)) {
            print("Entered" + col.name);
            entered = col.gameObject;
        }
    }


    private bool isInteractionlayer(GameObject obj) {
        return interactionLayers == (interactionLayers | (1 << obj.layer));
    }

    void Awake() {
        if(controllerPicked == ControllerPicked.Right_Controller) {
            trackedObj = controllerRight.GetComponent<SteamVR_TrackedObject>();
        } else if(controllerPicked == ControllerPicked.Left_Controller) {
            trackedObj = controllerLeft.GetComponent<SteamVR_TrackedObject>();
        } else {
            print("Couldn't detect trackedObject, please specify the controller type in the settings.");
            Application.Quit();
        }
        controllerCollider.transform.parent = trackedObj.transform;
    }

    // Update is called once per frame
    void Update() {
        controller = SteamVR_Controller.Input((int)trackedObj.index);
    }
}
