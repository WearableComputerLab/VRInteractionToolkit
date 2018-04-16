using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpandMenu : MonoBehaviour {

    /* EXPAND menu implementation by Kieran May
    * University of South Australia
    * 
    * TODO
    * -Fix zooming when object selected
    * */

    public List<GameObject> selectableObjects = new List<GameObject>();
    private GameObject[] pickedObjects;
    public GameObject panel;
    public GameObject cameraHead;

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

    private float scaleAmount = 10f;
    void generate2DObjects(List<GameObject> pickedObject) {
        pickedObjects = new GameObject[pickedObject.Count];
        pickedObject.CopyTo(pickedObjects);
        print("generate2DObjectsSIZE:" + pickedObjects.Length);
        panel.transform.SetParent(null);
        print("Amount of objects selected:" + pickedObject.Count);
        for (int i = 0; i < pickedObject.Count; i++) {
            print("object:" + pickedObject[i].name + " | count:" + (i + 1));
            pickedObj = pickedObject[i];
            pickedObj2D = Instantiate(pickedObject[i], new Vector3(0f, 0f, 0f), Quaternion.identity) as GameObject;
            pickedObj2D.transform.SetParent(panel.transform, false);
            pickedObj2D.gameObject.AddComponent<Rigidbody>();
            pickedObj2D.GetComponent<Collider>().attachedRigidbody.isKinematic = true;
            //pickedObj2D.transform.localScale = new Vector3(0.0625f, 0.0625f, 0f);
            pickedObj2D.transform.localScale = new Vector3(pickedObject[i].transform.localScale.x / scaleAmount, pickedObject[i].transform.localScale.y / scaleAmount, pickedObject[i].transform.localScale.z / scaleAmount);
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

    public void selectObject(SteamVR_Controller.Device controller, GameObject obj) {
        //print("picked object:" + pickedObject);
        if (controller.GetTouchDown(SteamVR_Controller.ButtonMask.Trigger) && pickedObject == null && obj.transform.parent == panel.transform && obj.name != "TriangleQuadObject") {
            //pickedObject = obj;
            string objName = obj.name.Substring(0, obj.name.Length - 7);
            //print("obj picked:" + objName);
            pickedObject = GameObject.Find(objName);
            lastPickedObject = pickedObject;
            print("Final picked object:" + objName);
            oldPickedObjectMaterial = pickedObject.transform.GetComponent<Renderer>().material;
            pickedObject.transform.GetComponent<Renderer>().material = selectedMaterial;
            disableEXPAND();
        }
    }

    public void enableEXPAND(SteamVR_Controller.Device controller, SteamVR_TrackedObject trackedObj, List<GameObject> obj) {
        if (trackedObj != null) {
            if (controller.GetTouchDown(SteamVR_Controller.ButtonMask.Trigger) && pickedUpObject == false) {
                //cameraHead.GetComponent<Camera>().fieldOfView = 20;
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
        panel.SetActive(false);
        SphereCastingExp.inMenu = false;
        pickedObject = null;
        destroyChildGameObjects();
        //cameraHead.GetComponent<Camera>().fieldOfView = 60;
        print("Expand disabled..");

    }

    public void enableEXPAND() {
        panel.SetActive(true);
        SphereCastingExp.inMenu = true;
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
        if (collider.gameObject.layer != LayerMask.NameToLayer("Ignore Raycast")) {
            selectableObjects.Add(collider.gameObject);
        }
    }

}
