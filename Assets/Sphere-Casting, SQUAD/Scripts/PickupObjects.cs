using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupObjects : MonoBehaviour {

    /* Sphere-Casting implementation by Kieran May
    * University of South Australia
    * 
    * -This class handles detecting & picking up objects based on the Sphere's radius
    * */

    public List<GameObject> selectableObjects = new List<GameObject>();
    public static GameObject currentObject;
	public LayerMask interactableLayer;

    /*public PickupObjects() {
        print("declared");
    }*/

    private bool pickedUpObject = false; //ensure only 1 object is picked up at a time
    private GameObject tempObjectStored;

    public void PickupObject(SteamVR_Controller.Device controller, SteamVR_TrackedObject trackedObj, List<GameObject> obj) {
        if (trackedObj != null) {
            if (controller.GetPressDown(SteamVR_Controller.ButtonMask.Trigger) && pickedUpObject == false) {
                for (int i = 0; i < obj.Count; i++) {
					if (obj[i].layer != LayerMask.NameToLayer("Ignore Raycast") && obj[i].layer == Mathf.Log(interactableLayer.value, 2)) {
                        obj[i].transform.SetParent(trackedObj.transform);
                        pickedUpObject = true;
                    }
                }
            }
            if (controller.GetPressUp(SteamVR_Controller.ButtonMask.Trigger) && pickedUpObject == true) {
                for (int i = 0; i < obj.Count; i++) {
					if (obj[i].layer != LayerMask.NameToLayer("Ignore Raycast") && obj[i].layer == Mathf.Log(interactableLayer.value, 2)) {
                        obj[i].transform.SetParent(null);
                        pickedUpObject = false;
                        /*if (i == obj.Count-1) {
                            clearList();
                        }*/
                    }
                }
            }
        }
        //clearList();
    }

    public void clearList() {
        selectableObjects.Clear();
    }

    public List<GameObject> getSelectableObjects() {
        return selectableObjects;
    }

    public int selectableObjectsCount() {
        return selectableObjects.Count;
    }

    private void OnTriggerStay(Collider collider) {
        currentObject = collider.gameObject;
        selectableObjects.Add(collider.gameObject);
    }

}
