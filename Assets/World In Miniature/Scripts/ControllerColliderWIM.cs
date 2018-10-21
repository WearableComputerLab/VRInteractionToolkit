using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerColliderWIM : MonoBehaviour {

    private WorldInMiniature worldInMin;

    private void OnTriggerStay(Collider col) {
        if(col.gameObject.layer == Mathf.Log(worldInMin.interactableLayer.value, 2)) {
            //Debug.Log("You have collided with " + col.name + " and activated OnTriggerStay");
            if (worldInMin.controllerO.GetTouch(SteamVR_Controller.ButtonMask.Trigger) && worldInMin.objectPicked == false) {
                Debug.Log("You have collided with " + col.name + " while holding down Touch");
                if (worldInMin.interacionType == WorldInMiniature.InteractionType.Manipulation_Movement) {
                    worldInMin.oldParent = col.gameObject.transform.parent;
                    col.attachedRigidbody.isKinematic = true;
                    col.gameObject.transform.SetParent(this.gameObject.transform);
                    worldInMin.selectedObject = col.gameObject;
                    worldInMin.objectPicked = true;
                } else if (worldInMin.interacionType == WorldInMiniature.InteractionType.Selection) {
                    worldInMin.oldParent = col.gameObject.transform.parent;
                    
                    worldInMin.selectedObject = col.gameObject;
                    //worldInMin.selectedObject.transform.GetComponent<Renderer>().material = worldInMin.outlineMaterial;
                    worldInMin.objectPicked = true;
                } else if (worldInMin.interacionType == WorldInMiniature.InteractionType.Manipulation_Full && this.GetComponent<SelectionManipulation>().inManipulationMode == false) {
                    worldInMin.objectPicked = true;
                    this.GetComponent<SelectionManipulation>().selectedObject = col.gameObject;
                }
            }
            if(worldInMin.controllerO.GetPressUp(SteamVR_Controller.ButtonMask.Trigger) && worldInMin.objectPicked == true) {
                if(worldInMin.interacionType == WorldInMiniature.InteractionType.Manipulation_Movement) {
                    this.GetComponent<SelectionManipulation>().selectedObject.transform.SetParent(null);
                    worldInMin.objectPicked = false;
                }
            }
        }
    }

    private GameObject manipulationIcons;

    // Use this for initialization
    void Start () {
        worldInMin = GameObject.Find("WorldInMiniature_Technique").GetComponent<WorldInMiniature>();
        if(worldInMin.interacionType == WorldInMiniature.InteractionType.Manipulation_Full) {
            this.gameObject.AddComponent<SelectionManipulation>();
            this.GetComponent<SelectionManipulation>().trackedObj = worldInMin.trackedObjO;
            manipulationIcons = GameObject.Find("Manipulation_Icons");
            this.GetComponent<SelectionManipulation>().manipulationIcons = manipulationIcons;
        }
    }
}
