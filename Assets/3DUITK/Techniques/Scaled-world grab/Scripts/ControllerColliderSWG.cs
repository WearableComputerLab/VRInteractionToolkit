using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Valve.VR;

public class ControllerColliderSWG : MonoBehaviour {

    public ScaledWorldGrab scaledWorldGrab;

    private LayerMask interactionLayers;

    public UnityEvent selectedObject; // Invoked when an object is selected
    public UnityEvent droppedObject; // Invoked when an object is dropped
    public UnityEvent hovered; // Invoked when an object is hovered by technique
    public UnityEvent unHovered; // Invoked when an object is no longer hovered by the technique

    public GameObject scaleSelected = null;

    private void OnTriggerStay(Collider col) {
        this.interactionLayers = scaledWorldGrab.interactionLayers;
        if(!isInteractionlayer(col.gameObject)) {

            print("returning");
            return;
        }
        print(col.transform.name);
        if(scaledWorldGrab.objSelected && col.gameObject == scaledWorldGrab.selectedObject) {

            if(scaledWorldGrab.controllerEvents() == ScaledWorldGrab.ControllerState.TRIGGER_DOWN && scaledWorldGrab.objectGrabbed == false) {
                scaleSelected = scaledWorldGrab.selectedObject;
                unHovered.Invoke();
                selectedObject.Invoke();
                if(scaledWorldGrab.interacionType == ScaledWorldGrab.InteractionType.Manipulation_Movement) {
                    col.gameObject.transform.SetParent(scaledWorldGrab.trackedObj.gameObject.transform);
                    scaledWorldGrab.objectGrabbed = true;
                } else if(scaledWorldGrab.interacionType == ScaledWorldGrab.InteractionType.Selection) {
                    scaledWorldGrab.tempObjectStored = col.gameObject;
                    print("Selected object in pure selection mode:" + col.gameObject.name);
                    return;
                }
            }

        }
    }

    private void OnTriggerEnter(Collider col) {
        if(isInteractionlayer(col.gameObject)) {
            hovered.Invoke();
        }
    }

    private void OnTriggerExit(Collider col) {
        if(isInteractionlayer(col.gameObject)) {
            unHovered.Invoke();
        }
    }

    private bool isInteractionlayer(GameObject obj) {
        return interactionLayers == (interactionLayers | (1 << obj.layer));
    }

    // Use this for initialization
    void Start() {
        scaledWorldGrab = GameObject.Find("ScaledWorldGrab_Technique").GetComponent<ScaledWorldGrab>();
    }

    // Update is called once per frame
    void Update() {
        if(scaledWorldGrab.controllerEvents() == ScaledWorldGrab.ControllerState.TRIGGER_UP) {
            if(scaleSelected != null) {
                print("scale selected: " + scaleSelected);
                scaleSelected.gameObject.transform.SetParent(null);
                droppedObject.Invoke();
                scaledWorldGrab.objectGrabbed = false;
                scaledWorldGrab.resetProperties();
                scaleSelected = null;
            }
        }
    }
}
