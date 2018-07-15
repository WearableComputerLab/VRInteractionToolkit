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
    private SteamVR_TrackedObject trackedObjO; //tracked object other
    private SteamVR_Controller.Device controller;
    internal SteamVR_Controller.Device controllerO; //controller other
    private GameObject worldInMinParent;
    GameObject[] allSceneObjects;
    private GameObject cameraHead;
    private bool WiMAactive = false;
    private List<string> ignorableObjectsString = new List<string>{ "[CameraRig]", "Directional Light", "background"};
    private float scaleAmount = 20f;
    public Material outlineMaterial;

    public enum InteractionType { Selection, Manipulation_Movement, Manipulation_Full };
    public InteractionType interacionType;

    public enum ControllerPicked { Left_Controller, Right_Controller };
    public ControllerPicked controllerPicked;

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
                worldInMinParent.transform.Rotate(0, tiltAroundY, 0);
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

    internal GameObject selectedObject;
    internal bool objectPicked = false;
    internal Transform oldParent;

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

    void Awake() {
        GameObject controllerRight = GameObject.Find(CONSTANTS.rightController);
        GameObject controllerLeft = GameObject.Find(CONSTANTS.leftController);
        cameraHead = GameObject.Find(CONSTANTS.cameraEyes);

        worldInMinParent = this.transform.Find("WorldInMinParent").gameObject;
        if (controllerPicked == ControllerPicked.Right_Controller) {
            trackedObj = controllerRight.GetComponent<SteamVR_TrackedObject>();
            trackedObjO = controllerLeft.GetComponent<SteamVR_TrackedObject>();
        } else if (controllerPicked == ControllerPicked.Left_Controller) {
            trackedObj = controllerLeft.GetComponent<SteamVR_TrackedObject>();
            trackedObjO = controllerRight.GetComponent<SteamVR_TrackedObject>();
        } else {
            print("Couldn't detect trackedObject, please specify the controller type in the settings.");
            Application.Quit();
        }
    }
    private float tiltAroundY = 0f;
    public float tiltSpeed = 2f; //2x quicker than normal
    // Update is called once per frame
    void Update () {
        controller = SteamVR_Controller.Input((int)trackedObj.index);
        controllerO = SteamVR_Controller.Input((int)trackedObjO.index);
        createWiM();
        if (WiMAactive == true) {
            tiltAroundY = controller.GetAxis(Valve.VR.EVRButtonId.k_EButton_Axis0).y;
            if (controller.GetTouch(SteamVR_Controller.ButtonMask.Touchpad)) {
                worldInMinParent.transform.Rotate(0, tiltAroundY* tiltSpeed, 0);
            }
        }
        if (controllerO.GetTouchUp(SteamVR_Controller.ButtonMask.Trigger) && selectedObject == true) {
            selectedObject.transform.SetParent(oldParent);
            //print("changed pos:" + selectedObject.transform.localPosition);
            GameObject realObject = GameObject.Find(selectedObject.name);
            realObject.transform.localPosition = selectedObject.transform.localPosition;
            realObject.transform.localEulerAngles = selectedObject.transform.localEulerAngles;
            objectPicked = false;
        }

        }
}