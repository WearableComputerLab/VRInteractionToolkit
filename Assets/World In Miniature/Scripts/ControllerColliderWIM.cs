using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerColliderWIM : MonoBehaviour {

    private WorldInMiniature worldInMin;

	private List<GameObject> listOfChildrenR = new List<GameObject>();
	private void enableRigidBody(GameObject obj){
		if (null == obj)
			return;
		foreach (Transform child in obj.transform){
			if (null == child)
				continue;
			if (child.gameObject.GetComponent<Rigidbody> () != null) {
				child.gameObject.GetComponent<Rigidbody> ().isKinematic = true;
			}
			listOfChildrenR.Add(child.gameObject);
			enableRigidBody(child.gameObject);
		}
	}

	private List<GameObject> listOfChildrenR2 = new List<GameObject>();
	private void disableRigidBody(GameObject obj){
		if (null == obj)
			return;
		foreach (Transform child in obj.transform){
			if (null == child)
				continue;
			if (child.gameObject.GetComponent<Rigidbody> () != null) {
				child.gameObject.GetComponent<Rigidbody> ().isKinematic = false;
			}
			listOfChildrenR2.Add(child.gameObject);
			disableRigidBody(child.gameObject);
		}
	}

    private void OnTriggerStay(Collider col) {
		if(col.gameObject.layer == Mathf.Log(worldInMin.interactableLayer.value, 2)) {
            //Debug.Log("You have collided with " + col.name + " and activated OnTriggerStay");
            if (worldInMin.controllerO.GetPress(SteamVR_Controller.ButtonMask.Trigger) && worldInMin.objectPicked == false) {
                Debug.Log("You have collided with " + col.name + " while holding down Touch");
                if (worldInMin.interacionType == WorldInMiniature.InteractionType.Manipulation_Movement) {
					listOfChildrenR.Clear();
                    worldInMin.oldParent = col.gameObject.transform.parent;
                    col.attachedRigidbody.isKinematic = true;
                    col.gameObject.transform.SetParent(this.gameObject.transform);
                    worldInMin.selectedObject = col.gameObject;
                    worldInMin.currentObjectCollided = col.gameObject;
                    worldInMin.objectPicked = true;
					//disableRigidBody(worldInMin.worldInMinParent);
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
                worldInMin.currentObjectCollided = null;

                if(worldInMin.interacionType == WorldInMiniature.InteractionType.Manipulation_Movement) {
					//col.attachedRigidbody.isKinematic = false;
					//enableRigidBody (worldInMin.worldInMinParent);
                    if(worldInMin.interacionType == WorldInMiniature.InteractionType.Manipulation_Full) {
                        this.GetComponent<SelectionManipulation>().selectedObject.transform.SetParent(null);
                    }
                    worldInMin.objectPicked = false;
                }
            }
        }
    }

	/*private bool isMoving() {
		if (worldInMin.selectedObject != null && worldInMin.selectedObject.GetComponent<Rigidbody> () != null) {
			print ("moving:" + worldInMin.selectedObject.GetComponent<Rigidbody> ().velocity);
			return !worldInMin.selectedObject.transform.GetComponent<Rigidbody> ().IsSleeping ();
		}
		return false;
	}

	void Update() {
		print (isMoving ());
	}*/

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
