using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WorldInMiniature : MonoBehaviour {

    /* World In Miniature implementation by Kieran May
     * University of South Australia
     * 
     * */

    private SteamVR_TrackedObject trackedObj;
    private SteamVR_Controller.Device controller;
    public GameObject worldInMinParent;
    GameObject[] allSceneObjects;
    public GameObject[] ignorableObjects;
    public GameObject cameraHead;
    private bool WiMAactive = false;
    private List<string> ignorableObjectsString = new List<string>{ "[CameraRig]", "Directional Light", "background"};
    private float scaleAmount = 20f;

    void createWiM() {
        if (controller.GetPressDown(SteamVR_Controller.ButtonMask.ApplicationMenu)) {
            if (WiMAactive == false) {
                WiMAactive = true;
                print("Create world clone");
                for (int i = 0; i < allSceneObjects.Length; i++) {
                    if (!ignorableObjectsString.Contains(allSceneObjects[i].name)) {
                        GameObject cloneObject = Instantiate(allSceneObjects[i], new Vector3(0f, 0f, 0f), Quaternion.identity) as GameObject;
                        cloneObject.transform.SetParent(worldInMinParent.transform, false);
                        if (cloneObject.gameObject.GetComponent<Rigidbody>() == null) {
                            cloneObject.gameObject.AddComponent<Rigidbody>();
                        }
                        cloneObject.gameObject.GetComponent<Rigidbody>().isKinematic = true;
                        //cloneObject.gameObject.AddComponent<Collider>();
                        //cloneObject.GetComponent<Collider>().attachedRigidbody.isKinematic = true;
                        cloneObject.transform.localScale = new Vector3(allSceneObjects[i].transform.localScale.x / scaleAmount, allSceneObjects[i].transform.localScale.y / scaleAmount, allSceneObjects[i].transform.localScale.z / scaleAmount);
                        cloneObject.transform.localRotation = Quaternion.identity;
                        if (cloneObject.transform.GetComponent<Renderer>() != null) {
                            cloneObject.transform.GetComponent<Renderer>().material.color = Color.red;
                        }
                        float posX = allSceneObjects[i].transform.position.x / scaleAmount;
                        float posY = allSceneObjects[i].transform.position.y / scaleAmount;
                        float posZ = allSceneObjects[i].transform.position.z / scaleAmount;
                        cloneObject.transform.localPosition = new Vector3(posX, posY, posZ);
                    }
                }
                //worldInMinParent.transform.SetParent(null);
                //worldInMinParent.transform.localEulerAngles = new Vector3(0f, cameraHead.transform.localEulerAngles.y-45f, 0f);
                worldInMinParent.transform.localEulerAngles = new Vector3(0f, trackedObj.transform.localEulerAngles.y - 45f, 0f);
                //worldInMinParent.transform.localPosition -= new Vector3(0f, worldInMinParent.transform.position.y / 1.25f, 0f);
            } else if (WiMAactive == true) {
                WiMAactive = false;
                foreach (Transform child in worldInMinParent.transform) {
                    Destroy(child.gameObject);
                }
                worldInMinParent.transform.localPosition = new Vector3(0f, 0f, 0f);
                worldInMinParent.transform.SetParent(trackedObj.transform);
                resetAllProperties();
            }
        }
    }

    GameObject selectedObject;
    private bool objectPicked = false;
    Transform oldParent;

    private void OnTriggerStay(Collider col) {
        if (col.gameObject.tag == "PickableObject") {
            //Debug.Log("You have collided with " + col.name + " and activated OnTriggerStay");
            if (controller.GetTouch(SteamVR_Controller.ButtonMask.Trigger) && objectPicked == false) {
                Debug.Log("You have collided with " + col.name + " while holding down Touch");
                oldParent = col.gameObject.transform.parent;
                col.attachedRigidbody.isKinematic = true;
                col.gameObject.transform.SetParent(this.gameObject.transform);
                selectedObject = col.gameObject;
                objectPicked = true;
            }
        }
    }

    private void resetAllProperties() {
        worldInMinParent.transform.localScale = new Vector3(1f, 1f, 1f);
        worldInMinParent.transform.localPosition = new Vector3(0f, 0f, 0f);
        worldInMinParent.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
    }

	// Use this for initialization
	void Start () {
        allSceneObjects = SceneManager.GetActiveScene().GetRootGameObjects();
        //allSceneObjects = FindObjectsOfType<GameObject>();
        worldInMinParent.transform.SetParent(trackedObj.transform);
        resetAllProperties();
    }

    private void Awake() {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
    }

    // Update is called once per frame
    void Update () {
        controller = SteamVR_Controller.Input((int)trackedObj.index);
        createWiM();
        if (controller.GetTouchUp(SteamVR_Controller.ButtonMask.Trigger) && selectedObject == true) {
            selectedObject.transform.SetParent(oldParent);
            //print("changed pos:" + selectedObject.transform.localPosition);
            GameObject realObject = GameObject.Find(selectedObject.name);
            realObject.transform.localPosition = selectedObject.transform.localPosition;
            realObject.transform.localEulerAngles = selectedObject.transform.localEulerAngles;
            objectPicked = false;
        }

        }
}
