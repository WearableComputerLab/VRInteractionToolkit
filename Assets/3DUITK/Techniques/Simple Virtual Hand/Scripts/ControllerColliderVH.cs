using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Valve.VR;

public class ControllerColliderVH : MonoBehaviour {

    public SimpleVirtualHand simpleVirtualHand;

    private LayerMask interactionLayers;

    public UnityEvent selectedObject; // Invoked when an object is selected
    public UnityEvent droppedObject; // Invoked when an object is dropped
    public UnityEvent hovered; // Invoked when an object is hovered by technique
    public UnityEvent unHovered; // Invoked when an object is no longer hovered by the technique

    public GameObject scaleSelected = null;

    public enum ControllerState {
        UP, DOWN, NONE
    }

    private ControllerState controllerEvents() {
#if SteamVR_Legacy
        if (simpleVirtualHand.controller.GetPressDown(SteamVR_Controller.ButtonMask.Trigger)) {
            return ControllerState.DOWN;
        }
        if (simpleVirtualHand.controller.GetPressUp(SteamVR_Controller.ButtonMask.Trigger)) {
            return ControllerState.DOWN;
        }
#elif SteamVR_2
        if (simpleVirtualHand.m_controllerPress.GetStateDown(simpleVirtualHand.trackedObj.inputSource)) {
            return ControllerState.DOWN;
        }
        if (simpleVirtualHand.m_controllerPress.GetStateUp(simpleVirtualHand.trackedObj.inputSource)) {
            return ControllerState.UP;
        }

#endif

        return ControllerState.NONE;
    }

    private void OnTriggerStay(Collider col) {
        this.interactionLayers = simpleVirtualHand.interactionLayers;
        if(!isInteractionlayer(col.gameObject)) {

            print("returning");
            return;
        }
        if(controllerEvents() == ControllerState.DOWN && simpleVirtualHand.objectGrabbed == false) {
            scaleSelected = simpleVirtualHand.selectedObject;
            unHovered.Invoke();
            selectedObject.Invoke();
            if(simpleVirtualHand.interacionType == SimpleVirtualHand.InteractionType.Manipulation_Movement) {
                col.gameObject.transform.SetParent(simpleVirtualHand.trackedObj.gameObject.transform);
                simpleVirtualHand.objectGrabbed = true;
                simpleVirtualHand.selectedObject = col.gameObject;
            } else if(simpleVirtualHand.interacionType == SimpleVirtualHand.InteractionType.Selection) {
                simpleVirtualHand.selectedObject = col.gameObject;
                print("Selected object in pure selection mode:" + col.gameObject.name);
                return;
            }
        } else if(controllerEvents() == ControllerState.DOWN && simpleVirtualHand.objectGrabbed == true) {
            simpleVirtualHand.selectedObject.gameObject.transform.SetParent(null);
            print("Object dropped..");
            simpleVirtualHand.objectGrabbed = false;
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
        simpleVirtualHand = GameObject.Find("SimpleVirtualHand_Technique").GetComponent<SimpleVirtualHand>();
    }
}
