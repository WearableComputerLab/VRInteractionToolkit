using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Valve.VR;
public class WorldInMiniature : MonoBehaviour {

    /* World In Miniature implementation by Kieran May
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

#if SteamVR_Legacy
    internal SteamVR_TrackedObject trackedObj;
    internal SteamVR_TrackedObject trackedObjO; //tracked object other
    private SteamVR_Controller.Device controller;
    internal SteamVR_Controller.Device controllerO; //controller other
#elif SteamVR_2
    internal SteamVR_Behaviour_Pose trackedObj;
    internal SteamVR_Behaviour_Pose trackedObjO; //tracked object other
    public SteamVR_Action_Boolean m_controllerPress;
    public SteamVR_Action_Boolean m_menuButton;
    public SteamVR_Action_Vector2 m_touchpadAxis;
    public SteamVR_Action_Boolean m_touchpadTouch;
#else
    public GameObject trackedObj;
    public GameObject trackedObjO;
#endif

    public GameObject worldInMinParent;
    GameObject[] allSceneObjects;
    public static bool WiMrunning = false;
    public bool WiMactive = false;
    public List<string> ignorableObjectsString = new List<string> { "[CameraRig]", "Directional Light", "background" };
    public float scaleAmount = 20f;
    public LayerMask interactableLayer;
    public Material outlineMaterial;

    public enum InteractionType { Selection, Manipulation_Movement };
    public InteractionType interacionType;

    public enum ControllerPicked { Left_Controller, Right_Controller };
    public ControllerPicked controllerPicked;

    public GameObject controllerRight;
    public GameObject controllerLeft;
    public GameObject cameraHead;
    private int counter = 0;

    private List<GameObject> listOfChildren = new List<GameObject>();
    private void findClonedObject(GameObject obj) {
        if (null == obj)
            return;
        foreach (Transform child in obj.transform) {
            if (null == child)
                continue;
            if (child.gameObject.GetComponent<Rigidbody>() != null) {
                child.gameObject.GetComponent<Rigidbody>().isKinematic = true;
            }
            if (child.GetComponent<ObjectID>() != null) {
                listOfIDs[child.GetComponent<ObjectID>().ID - 1].GetComponent<ObjectID>().clonedObject = child.gameObject;
            }
            //listOfIDs[child.gameObject.GetComponent<ObjectID>().ID].GetComponent<ObjectID>().clonedObject = child.gameObject;
            //listOfIDs[child.gameObject.GetComponent]
            //listOfIDs[counter].GetComponent<ObjectID>().clonedObject = child.gameObject;
            counter++;
            listOfChildren.Add(child.gameObject);
            findClonedObject(child.gameObject);
        }
    }

    private List<GameObject> listOfIDs = new List<GameObject>();
    private void setIDObject(GameObject obj) {
        if (null == obj)
            return;
        foreach (Transform child in obj.transform) {
            if (null == child)
                continue;
            //if (child.gameObject.GetComponent<Rigidbody> () != null) {
            this.GetComponent<WIM_IDHandler>().addID(child.gameObject);
            //}
            listOfIDs.Add(child.gameObject);
            setIDObject(child.gameObject);
        }
    }

    public enum ControllerState {
        TRIGGER_UP, TRIGGER_DOWN, TRIGGER_PRESS, APPLICATION_MENU, NONE, TOUCHPAD_TOUCH
    }

    public ControllerState controllerEvents() {
#if SteamVR_Legacy
        if (controller.GetPressDown(Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger)) {
            return ControllerState.TRIGGER_DOWN;
        }
        if (controller.GetPressUp(Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger)) {
            return ControllerState.TRIGGER_UP;

        }
        if (controllerO.GetPress(Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger)) {
            return ControllerState.TRIGGER_PRESS;
        }
        if (controllerO.GetPressUp(Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger)) {
            return ControllerState.TRIGGER_UP;

        }
        if (controller.GetPressDown(SteamVR_Controller.ButtonMask.ApplicationMenu)) {
            return ControllerState.APPLICATION_MENU;
        }
        if (controller.GetTouch(SteamVR_Controller.ButtonMask.Touchpad)) {
            return ControllerState.TOUCHPAD_TOUCH;
        }
        if (controllerO.GetPressUp(SteamVR_Controller.ButtonMask.Trigger)) {
            return ControllerState.TRIGGER_UP;
        }
#elif SteamVR_2
        if (m_controllerPress.GetStateDown(trackedObj.inputSource)) {
            return ControllerState.TRIGGER_DOWN;
        } if (m_controllerPress.GetStateUp(trackedObj.inputSource)) {
            return ControllerState.TRIGGER_UP;
        } if (m_controllerPress.GetState(trackedObjO.inputSource)) {
            return ControllerState.TRIGGER_PRESS;
        } if (m_controllerPress.GetStateUp(trackedObjO.inputSource)) {
            return ControllerState.TRIGGER_UP;
        } if (m_menuButton.GetStateDown(trackedObj.inputSource)) {
            return ControllerState.APPLICATION_MENU;
        } if (m_controllerPress.GetStateDown(trackedObjO.inputSource)) {
            return ControllerState.TRIGGER_DOWN;
        } if (m_touchpadTouch.GetState(trackedObj.inputSource)) {
            return ControllerState.TOUCHPAD_TOUCH;
        }

#endif

        return ControllerState.NONE;
    }

    void createWiM() {
        if (controllerEvents() == ControllerState.APPLICATION_MENU) {
            if (WiMactive == false) {
                WiMactive = true;
                WiMrunning = true;
                print("Create world clone");
                for (int i = 0; i < allSceneObjects.Length; i++) {
                    if (!ignorableObjectsString.Contains(allSceneObjects[i].name)) {
                        GameObject cloneObject = Instantiate(allSceneObjects[i], new Vector3(0f, 0f, 0f), Quaternion.identity) as GameObject;
                        //cloneObject.transform.name = allSceneObjects[i].name;
                        cloneObject.transform.SetParent(worldInMinParent.transform, false);
                        if (cloneObject.gameObject.GetComponent<Rigidbody>() == null) {
                            cloneObject.gameObject.AddComponent<Rigidbody>();
                        }/* else {
                            if (cloneObject.gameObject.GetComponent<Rigidbody>().isKinematic == false) {

                            }
                        }*/
                        cloneObject.gameObject.GetComponent<Rigidbody>().isKinematic = true;
                        //cloneObject.gameObject.AddComponent<Collider>();
                        //cloneObject.GetComponent<Collider>().attachedRigidbody.isKinematic = true;
                        cloneObject.transform.localScale = new Vector3(allSceneObjects[i].transform.lossyScale.x / scaleAmount, allSceneObjects[i].transform.lossyScale.y / scaleAmount, allSceneObjects[i].transform.lossyScale.z / scaleAmount);
                        cloneObject.transform.localRotation = Quaternion.identity;
                        if (cloneObject.transform.GetComponent<Renderer>() != null) {
                            //cloneObject.transform.GetComponent<Renderer>().material.color = Color.red;
                        }
                        float posX = allSceneObjects[i].transform.position.x / scaleAmount;
                        float posY = allSceneObjects[i].transform.position.y / scaleAmount;
                        float posZ = allSceneObjects[i].transform.position.z / scaleAmount;
                        cloneObject.transform.localPosition = new Vector3(posX, posY, posZ);
                    }
                }
                findClonedObject(worldInMinParent);
                //worldInMinParent.transform.SetParent(null);
                //worldInMinParent.transform.localEulerAngles = new Vector3(0f, cameraHead.transform.localEulerAngles.y-45f, 0f);
                worldInMinParent.transform.localEulerAngles = new Vector3(0f, trackedObj.transform.localEulerAngles.y - 45f, 0f);
                worldInMinParent.transform.Rotate(0, tiltAroundY, 0);
                //worldInMinParent.transform.localPosition -= new Vector3(0f, worldInMinParent.transform.position.y / 1.25f, 0f);
            } else if (WiMactive == true) {
                WiMactive = false;
                WiMrunning = false;
                foreach (Transform child in worldInMinParent.transform) {
                    Destroy(child.gameObject);
                }
                worldInMinParent.transform.localPosition = new Vector3(0f, 0f, 0f);
                worldInMinParent.transform.SetParent(trackedObj.transform);
                resetAllProperties();
            }
        }
    }

    public GameObject selectedObject;

    public GameObject currentObjectCollided;


    internal bool objectPicked = false;
    internal Transform oldParent;

    private void resetAllProperties() {
        worldInMinParent.transform.localScale = new Vector3(1f, 1f, 1f);
        worldInMinParent.transform.localPosition = new Vector3(0f, 0f, 0f);
        worldInMinParent.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
    }

    // Use this for initialization
    void Start() {

        allSceneObjects = SceneManager.GetActiveScene().GetRootGameObjects();
        for (int i = 0; i < allSceneObjects.Length; i++) {
            setIDObject(allSceneObjects[i]);
        }

        worldInMinParent.transform.SetParent(trackedObj.transform);
        resetAllProperties();

        //adding colliders and collider scripts to controllers for WIM if they don't allready exist
        SphereCollider col;
        if ((col = trackedObj.transform.gameObject.GetComponent<SphereCollider>()) == null) {

            col = trackedObj.transform.gameObject.AddComponent<SphereCollider>();
            col.isTrigger = true;
            col.radius = 0.05f;
            col.center = new Vector3(0f, -0.05f, 0f);
            trackedObj.transform.gameObject.AddComponent<ControllerColliderWIM>();
        }
        SphereCollider col0;
        if ((col0 = trackedObjO.transform.gameObject.GetComponent<SphereCollider>()) == null) {

            col0 = trackedObjO.transform.gameObject.AddComponent<SphereCollider>();
            col0.isTrigger = true;
            col0.radius = 0.05f;
            col0.center = new Vector3(0f, -0.05f, 0f);
            trackedObjO.transform.gameObject.AddComponent<ControllerColliderWIM>();
        }
    }

    public GameObject findRealObject(GameObject selectedObject) {
        for (int i = 0; i < listOfIDs.Count; i++) {
            //print("looking for:" + )
            if (selectedObject.GetComponent<ObjectID>().ID == listOfIDs[i].GetComponent<ObjectID>().ID) {
                return listOfIDs[i];
            }
        }
        return null;
    }

    private void initializeControllers() {
        if (controllerPicked == ControllerPicked.Right_Controller) {
#if SteamVR_Legacy
            trackedObj = controllerRight.GetComponent<SteamVR_TrackedObject>();
            trackedObjO = controllerLeft.GetComponent<SteamVR_TrackedObject>();
#elif SteamVR_2
            trackedObj = controllerRight.GetComponent<SteamVR_Behaviour_Pose>();
            trackedObjO = controllerLeft.GetComponent<SteamVR_Behaviour_Pose>();
#endif
        } else if (controllerPicked == ControllerPicked.Left_Controller) {
#if SteamVR_Legacy
            trackedObj = controllerLeft.GetComponent<SteamVR_TrackedObject>();
            trackedObjO = controllerRight.GetComponent<SteamVR_TrackedObject>();
#elif SteamVR_2
            trackedObj = controllerLeft.GetComponent<SteamVR_Behaviour_Pose>();
            trackedObjO = controllerRight.GetComponent<SteamVR_Behaviour_Pose>();
#endif
        } else {
            print("Couldn't detect trackedObject, please specify the controller type in the settings.");
            Application.Quit();
        }
    }

    void Awake() {
        worldInMinParent = this.transform.Find("WorldInMinParent").gameObject;
        initializeControllers();
    }

    public bool isMoving() {
        if (realObject != null && realObject.transform.GetComponent<Rigidbody>() != null) {
            return !realObject.transform.GetComponent<Rigidbody>().IsSleeping();
        }
        return false;
    }

    private GameObject realObject;
    private float tiltAroundY = 0f;
    public float tiltSpeed = 2f; //2x quicker than normal
    private bool startedMoving = false;
    // Update is called once per frame
    void Update() {
#if SteamVR_Legacy
        controller = SteamVR_Controller.Input((int)trackedObj.index);
        controllerO = SteamVR_Controller.Input((int)trackedObjO.index);
#endif

        //print (isMoving ());
        if (WiMactive == true && isMoving() == true && selectedObject != null && selectedObject.GetComponent<ObjectID>() != null && realObject != null && realObject.GetComponent<ObjectID>() != null && selectedObject.GetComponent<ObjectID>().ID == realObject.GetComponent<ObjectID>().ID) {
            startedMoving = true;
            selectedObject.transform.localPosition = realObject.transform.localPosition;
            selectedObject.transform.localEulerAngles = realObject.transform.localEulerAngles;
        } else if (isMoving() == false && startedMoving == true) {
            startedMoving = false;
        }

        createWiM();
        if (WiMactive == true) {
#if SteamVR_Legacy
            tiltAroundY = controller.GetAxis(Valve.VR.EVRButtonId.k_EButton_Axis0).y;
#elif SteamVR_2
            tiltAroundY = m_touchpadAxis.GetAxis(trackedObj.inputSource).y;
#endif
            if (controllerEvents() == ControllerState.TOUCHPAD_TOUCH) {
                worldInMinParent.transform.Rotate(0, tiltAroundY * tiltSpeed, 0);
            }
        }
        if (controllerEvents() == ControllerState.TRIGGER_UP && selectedObject == true) {
            selectedObject.transform.SetParent(oldParent);
            realObject = findRealObject(selectedObject);
            realObject.transform.localPosition = selectedObject.transform.localPosition;
            realObject.transform.localEulerAngles = selectedObject.transform.localEulerAngles;
            objectPicked = false;
        }

    }
}