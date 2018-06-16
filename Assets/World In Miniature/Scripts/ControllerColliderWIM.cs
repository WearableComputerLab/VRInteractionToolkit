using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerColliderWIM : MonoBehaviour {

    private WorldInMiniature worldInMin;

    private void OnTriggerStay(Collider col) {
        if (col.gameObject.tag == "PickableObject") {
            //Debug.Log("You have collided with " + col.name + " and activated OnTriggerStay");
            if (worldInMin.controllerO.GetTouch(SteamVR_Controller.ButtonMask.Trigger) && worldInMin.objectPicked == false) {
                Debug.Log("You have collided with " + col.name + " while holding down Touch");
                if (worldInMin.interacionType == WorldInMiniature.InteractionType.Manipulation_Full || worldInMin.interacionType == WorldInMiniature.InteractionType.Manipulation_Movement) {
                    worldInMin.oldParent = col.gameObject.transform.parent;
                    col.attachedRigidbody.isKinematic = true;
                    col.gameObject.transform.SetParent(this.gameObject.transform);
                    worldInMin.selectedObject = col.gameObject;
                    worldInMin.objectPicked = true;
                } else if (worldInMin.interacionType == WorldInMiniature.InteractionType.Selection) {
                    worldInMin.oldParent = col.gameObject.transform.parent;
                    
                    worldInMin.selectedObject = col.gameObject;
                    worldInMin.selectedObject.transform.GetComponent<Renderer>().material = worldInMin.outlineMaterial;
                    worldInMin.objectPicked = true;
                }
            }
        }
    }

    // Use this for initialization
    void Start () {
        worldInMin = GameObject.Find("WorldInMiniature_Technique").GetComponent<WorldInMiniature>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
