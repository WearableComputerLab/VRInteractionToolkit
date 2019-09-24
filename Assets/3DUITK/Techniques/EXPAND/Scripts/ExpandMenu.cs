using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class ExpandMenu : MonoBehaviour {

     /* EXPAND menu implementation by Kieran May
     * University of South Australia
     * 
     *  Copyright(C) 2019 Kieran May
	 *
	 *  This program is free software: you can redistribute it and/or modify
	 *  it under the terms of the GNU General Public License as published by
	 *  the Free Software Foundation, either version 3 of the License, or
	 *  (at your option) any later version.
	 * 
	 *  This program is distributed in the hope that it will be useful,
	 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
	 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
	 *  GNU General Public License for more details.
	 *
	 *  You should have received a copy of the GNU General Public License
	 *  along with this program.If not, see<http://www.gnu.org/licenses/>.
	 */


    public List<GameObject> selectableObjects = new List<GameObject>();
    private GameObject[] pickedObjects;
    private GameObject panel;
    internal GameObject cameraHead;

    public LayerMask interactableLayer;
    private bool pickedUpObject = false; //ensure only 1 object is picked up at a time
    private GameObject tempObjectStored;

    private GameObject pickedObj2D = null;
    private GameObject pickedObj = null;
    private int imageSlots = 0;
    private float[,] positions = new float[,] { { -0.3f, 0.2f }, { -0.1f, 0.2f }, { 0.1f, 0.2f }, { 0.3f, 0.2f },
                                                { -0.3f, 0.0f }, { -0.1f, 0.0f }, { 0.1f, 0.0f }, { 0.3f, 0.0f },
                                                { -0.3f, -0.2f }, { -0.1f, -0.2f  }, { 0.1f, -0.2f  }, { 0.3f, -0.2f  },
                                                { -0.3f, -0.4f }, { -0.1f, -0.4f }, { 0.1f, -0.4f }, { 0.3f, -0.4f },
                                                { -0.3f, -0.6f  }, { -0.1f, -0.6f  }, { 0.1f, -0.6f }, { 0.3f, -0.6f },
                                                { -0.3f, -0.8f }, { -0.1f, -0.8f }, { 0.1f, -0.8f }, { 0.3f, -0.8f }};
    internal SphereCastingExp sphereCasting;
    public float scaleAmount = 10f;
    void generate2DObjects(List<GameObject> pickedObject) {
        pickedObjects = new GameObject[pickedObject.Count];
        pickedObject.CopyTo(pickedObjects);
        print("generate2DObjectsSIZE:" + pickedObjects.Length);
        if (pickedObjects.Length == 0) {
            return;
        }
        panel.transform.SetParent(null);
        print("Amount of objects selected:" + pickedObject.Count);
        for (int i = 0; i < pickedObject.Count && pickedObject[i].layer == Mathf.Log(interactableLayer.value, 2) && i < 27; i++) {
            print("object:" + pickedObject[i].name + " | count:" + (i + 1));
            pickedObj = pickedObject[i];
            pickedObj2D = Instantiate(pickedObject[i], new Vector3(0f, 0f, 0f), Quaternion.identity) as GameObject;
            pickedObj2D.transform.SetParent(panel.transform, false);
            if (pickedObj2D.GetComponent<Rigidbody>() == null) {
                pickedObj2D.gameObject.AddComponent<Rigidbody>();
            }
            pickedObj2D.GetComponent<Rigidbody>().isKinematic = true;
            pickedObj2D.transform.localScale = new Vector3(pickedObject[i].transform.lossyScale.x / scaleAmount, pickedObject[i].transform.lossyScale.y / scaleAmount, pickedObject[i].transform.lossyScale.z / scaleAmount);
            pickedObj2D.transform.localRotation = Quaternion.identity;

            int pos = 0;
            float posX = 0;
            float posY = 0;
            imageSlots++;
            pos = imageSlots - 1;
            posX = positions[pos, 0];
            posY = positions[pos, 1];
            pickedObj2D.transform.localPosition = new Vector3(posX, posY, 0f);
        }
    }

    public bool isActive() {
        return panel.activeInHierarchy;
    }

    private GameObject pickedObject;
    private GameObject lastPickedObject;
    private Material oldPickedObjectMaterial;
    public Material selectedMaterial;

    public void selectObject(GameObject obj) {
        if (sphereCasting.controllerEvents() == SphereCastingExp.ControllerState.TRIGGER_DOWN && pickedObject == null && obj.transform.parent == panel.transform && obj.name != "TriangleQuadObject") {
            print("Trigger down pressed..");
            string objName = obj.name.Substring(0, obj.name.Length - 7);
            //print("obj picked:" + objName);
            pickedObject = GameObject.Find(objName);
            lastPickedObject = pickedObject;
            print("Final picked object:" + objName);
            if (sphereCasting.interactionType == SphereCastingExp.InteractionType.Selection) {
                sphereCasting.selectedObject = pickedObject;
            } else if (sphereCasting.interactionType == SphereCastingExp.InteractionType.Manipulation_Movement) {
                sphereCasting.selectedObject = pickedObject;
                pickedObject.transform.SetParent(sphereCasting.trackedObj.transform);
            } else if (sphereCasting.interactionType == SphereCastingExp.InteractionType.Manipulation_UI && sphereCasting.GetComponent<SelectionManipulation>().inManipulationMode == false) {
                sphereCasting.selectedObject = pickedObject;
                sphereCasting.GetComponent<SelectionManipulation>().selectedObject = obj;
            }
            if (pickedObject.transform.GetComponent<Renderer>() != null) {
                oldPickedObjectMaterial = pickedObject.transform.GetComponent<Renderer>().material;
                //pickedObject.transform.GetComponent<Renderer>().material = selectedMaterial;
            }
            disableEXPAND();
        }
    }

    public void enableEXPAND(List<GameObject> obj) {
        if (sphereCasting.trackedObj != null) {
            if (sphereCasting.controllerEvents() == SphereCastingExp.ControllerState.TRIGGER_DOWN && pickedUpObject == false) {
                print("Trigger down pressed..");
                SphereCastingExp.inMenu = true;
                panel.SetActive(true);
                generate2DObjects(obj);
                if (lastPickedObject != null) {
                    lastPickedObject.transform.GetComponent<Renderer>().material = oldPickedObjectMaterial;
                }
            }
        }
    }

    private void destroyChildGameObjects() {
        foreach (Transform child in panel.transform) {
            GameObject.Destroy(child.gameObject);
        }
    }

    public void disableEXPAND() {
        clearList();
        panel.transform.SetParent(cameraHead.transform);
        panel.transform.localPosition = new Vector3(0f, 0f, 1f);
        panel.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
        panel.transform.localScale = new Vector3(1f, 1f, 1f);
        panel.SetActive(false);
        SphereCastingExp.inMenu = false;
        pickedObject = null;
        destroyChildGameObjects();
        print("Expand disabled..");

    }

    public void enableEXPAND() {
        panel.SetActive(true);
        SphereCastingExp.inMenu = true;
    }

    private void Awake() {
        panel = GameObject.Find("menuPanel");
    }

    private void Start() {
        disableEXPAND();
    }

    public void clearList() {
        imageSlots = 0;
        selectableObjects.Clear();
    }

    public List<GameObject> getSelectableObjects() {
        return selectableObjects;
    }

    public int selectableObjectsCount() {
        return selectableObjects.Count;
    }

    private void OnTriggerStay(Collider collider) {
        if (collider.gameObject.layer != LayerMask.NameToLayer("Ignore Raycast") && collider.gameObject.layer == Mathf.Log(interactableLayer.value, 2)) {
            selectableObjects.Add(collider.gameObject);
        }
    }

}
