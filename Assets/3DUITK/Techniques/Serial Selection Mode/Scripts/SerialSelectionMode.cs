using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Valve.VR;

public class SerialSelectionMode : MonoBehaviour {

     /* Serial Selection Mode implementation by Kieran May
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
    private SteamVR_TrackedObject trackedObj;
    private SteamVR_Controller.Device controller;
#elif SteamVR_2
    private SteamVR_Behaviour_Pose trackedObj;
    public SteamVR_Action_Boolean m_controllerPress;
    public SteamVR_Action_Boolean m_applicationMenu;
#else
    public GameObject trackedObj;
#endif

    public GameObject controllerRight;
    public GameObject controllerLeft;

    private GameObject mirroredCube;

    public GameObject laserPrefab;
    private GameObject laser;
    private Transform laserTransform;
    private Vector3 hitPoint;
    private Vector3 hitPoint2D;
    private bool pickUpObjectsActive = false;
    public Material outlineMaterial;

    public enum InteractionType { Selection, Manipulation_Movement };
    public InteractionType interactionType;

    public enum ControllerPicked { Left_Controller, Right_Controller };
    public ControllerPicked controllerPicked;

    public UnityEvent selectedObject; // Invoked when an object is selected

    private void ShowLaser(RaycastHit hit) {
        mirroredCube.SetActive(false);
        laser.SetActive(true);
        laserTransform.position = Vector3.Lerp(trackedObj.transform.position, hitPoint, .5f);
        laserTransform.LookAt(hitPoint);
        laserTransform.localScale = new Vector3(laserTransform.localScale.x, laserTransform.localScale.y, hit.distance);
    }
    private void ShowLaser() {
        laser.SetActive(true);
        mirroredCube.SetActive(true);
    }

    private void initializeControllers() {
        if (controllerPicked == ControllerPicked.Right_Controller) {
#if SteamVR_Legacy
            trackedObj = controllerRight.GetComponent<SteamVR_TrackedObject>();
#elif SteamVR_2
            trackedObj = controllerRight.GetComponent<SteamVR_Behaviour_Pose>();
#endif
        } else if (controllerPicked == ControllerPicked.Left_Controller) {
#if SteamVR_Legacy
            trackedObj = controllerLeft.GetComponent<SteamVR_TrackedObject>();
#elif SteamVR_2
            trackedObj = controllerLeft.GetComponent<SteamVR_Behaviour_Pose>();
#endif
        } else {
            print("Couldn't detect trackedObject, please specify the controller type in the settings.");
            Application.Quit();
        }

    }

    void Awake() {
        mirroredCube = this.transform.Find("Mirrored Cube").gameObject;
        initializeControllers();
    }

    void Start() {
        laser = Instantiate(laserPrefab);
        laserTransform = laser.transform;
    }

    void mirroredObject() {
        Vector3 controllerPos = trackedObj.transform.forward;
        float distance_formula_on_vector = Mathf.Sqrt(controllerPos.x * controllerPos.x + controllerPos.y * controllerPos.y + controllerPos.z * controllerPos.z);
        Vector3 mirroredPos = trackedObj.transform.position;

        mirroredPos.x = mirroredPos.x + (100f / (distance_formula_on_vector)) * controllerPos.x;
        mirroredPos.y = mirroredPos.y + (100f / (distance_formula_on_vector)) * controllerPos.y;
        mirroredPos.z = mirroredPos.z + (100f / (distance_formula_on_vector)) * controllerPos.z;

        mirroredCube.transform.position = mirroredPos;
        mirroredCube.transform.rotation = trackedObj.transform.rotation;
    }

    public enum ControllerState {
        TRIGGER_UP, TRIGGER_DOWN, NONE, APPLICATION_MENU
    }

    private ControllerState controllerEvents() {
#if SteamVR_Legacy
        if (controller.GetPressDown(Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger)) {
            return ControllerState.TRIGGER_DOWN;
        } if (controller.GetPressUp(Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger)) {
            return ControllerState.TRIGGER_UP;
        } if (controller.GetPressDown(SteamVR_Controller.ButtonMask.ApplicationMenu)) {
            return ControllerState.APPLICATION_MENU;
        }
#elif SteamVR_2
        if (m_controllerPress.GetStateDown(trackedObj.inputSource)) {
            return ControllerState.TRIGGER_DOWN;
        }
        if (m_controllerPress.GetStateUp(trackedObj.inputSource)) {
            return ControllerState.TRIGGER_UP;
        } if (m_applicationMenu.GetStateDown(trackedObj.inputSource)) {
            return ControllerState.APPLICATION_MENU;
        }
#endif

        return ControllerState.NONE;
    }

    private List<GameObject> selectedObjectsList = new List<GameObject>();
    private List<Material> rendererMaterialTrackerList = new List<Material>();

    void selectObject(GameObject obj) {
        Vector3 controllerPos = trackedObj.transform.forward;
        if (trackedObj != null && pickUpObjectsActive == false) {
            if (controllerEvents() == ControllerState.TRIGGER_DOWN) {
                if (obj != null && obj.name != "Mirrored Cube" && !selectedObjectsList.Contains(obj)) {
                    selectedObjectsList.Add(obj);
                    rendererMaterialTrackerList.Add(obj.transform.GetComponent<Renderer>().material);
                    obj.transform.GetComponent<Renderer>().material = outlineMaterial;
                    print("selected object:" + obj.name);
                    print("list size:" + selectedObjectsList.Count);
                    selectedObject.Invoke();
                } else {
                    for (int i=0; i<selectedObjectsList.Count; i++) {
                        selectedObjectsList[i].transform.GetComponent<Renderer>().material = rendererMaterialTrackerList[i];
                    }
                    selectedObjectsList.Clear();
                    print("Invalid selection, list cleared.");
                }
            }
        }
    }

    private bool objectsSelected = false;

    void activatePickupObjects() {
        if (controllerEvents() == ControllerState.APPLICATION_MENU) {
            pickUpObjectsActive = !pickUpObjectsActive;
            print("pick up objects set to:" + pickUpObjectsActive);
        }
        if (pickUpObjectsActive == true) {
            if (controllerEvents() == ControllerState.TRIGGER_DOWN && objectsSelected == false && interactionType == InteractionType.Manipulation_Movement) {
                for (int i = 0; i < selectedObjectsList.Count; i++) {
                    if (selectedObjectsList[i].layer != LayerMask.NameToLayer("Ignore Raycast")) {
                        selectedObjectsList[i].transform.SetParent(trackedObj.transform);
                        objectsSelected = true;
                    }
                }
            }
            if (controllerEvents() == ControllerState.TRIGGER_UP && objectsSelected == true) {
                for (int i = 0; i < selectedObjectsList.Count; i++) {
                    if (selectedObjectsList[i].layer != LayerMask.NameToLayer("Ignore Raycast")) {
                        selectedObjectsList[i].transform.SetParent(null);
                        objectsSelected = false;
                    }
                }
            }
        }
    }

    void Update() {
        #if SteamVR_Legacy
        controller = SteamVR_Controller.Input((int)trackedObj.index);
#endif
        activatePickupObjects();
        mirroredObject();
        ShowLaser();
        RaycastHit hit;
        if (Physics.Raycast(trackedObj.transform.position, trackedObj.transform.forward, out hit, 100)) {
            hitPoint = hit.point;
            //PickupObject(hit.transform.gameObject);
            ShowLaser(hit);
        }
        selectObject(hit.transform.gameObject);
    }
}
