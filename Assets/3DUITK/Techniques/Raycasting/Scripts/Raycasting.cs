using UnityEngine;
using UnityEngine.Events;
using Valve.VR;

public class Raycasting : MonoBehaviour {

    /* Raycasting implementation by Kieran May
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
#else
    public GameObject trackedObj;
#endif
    public LayerMask interactionLayers;

    public GameObject controllerRight = null;
    public GameObject controllerLeft = null;



    public GameObject laserPrefab;
    private GameObject laser;
    private Transform laserTransform;
    private Vector3 hitPoint;
    private GameObject mirroredCube;

    public enum InteractionType {Selection, Manipulation_Movement, Manipulation_Full};
    public InteractionType interacionType;

    public enum ControllerPicked { Left_Controller, Right_Controller };
    public ControllerPicked controllerPicked;

    internal bool objectSelected = false;

    public UnityEvent selectedObject; // Invoked when an object is selected

    public UnityEvent droppedObject; // Invoked when object is dropped

	public UnityEvent hovered; // Invoked when an object is hovered by technique
	public UnityEvent unHovered; // Invoked when an object is no longer hovered by the technique

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

    public enum ControllerState {
        UP, DOWN, NONE
    }

    private ControllerState controllerEvents() {
#if SteamVR_Legacy
        if (controller.GetPressDown(Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger)) {
            return ControllerState.DOWN;
        }
        if (controller.GetPressUp(Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger)) {
            return ControllerState.UP;
        }
#elif SteamVR_2
        if (m_controllerPress.GetStateDown(trackedObj.inputSource)) {
            return ControllerState.DOWN;
        }
        if (m_controllerPress.GetStateUp(trackedObj.inputSource)) {
            return ControllerState.UP;
        }

#endif

        return ControllerState.NONE;
    }

    private bool pickedUpObject = false; //ensure only 1 object is picked up at a time
    public GameObject lastSelectedObject;
    public void PickupObject(GameObject obj) {
        if (interactionLayers != (interactionLayers | (1 << obj.layer))) {
            // object is wrong layer so return immediately 
            return;
        }
        if(lastSelectedObject != obj) {
            // is a different object from the currently highlighted so unhover
            unHovered.Invoke();
        }
        hovered.Invoke();
        Vector3 controllerPos = trackedObj.transform.forward;
        if (trackedObj != null) {
            if (controllerEvents() == ControllerState.DOWN && pickedUpObject == false) {
                if (interacionType == InteractionType.Manipulation_Movement) {
                    obj.transform.SetParent(trackedObj.transform);
                    lastSelectedObject = obj; // Storing the object as an instance variable instead of using the obj parameter fixes glitch of it not properly resetting on TriggerUp
                    pickedUpObject = true;
                } else if (interacionType == InteractionType.Manipulation_Full && this.GetComponent<SelectionManipulation>().inManipulationMode == false) {
                    lastSelectedObject = obj;
                    objectSelected = true;
                    this.GetComponent<SelectionManipulation>().selectedObject = obj;
                } else if (interacionType == InteractionType.Selection) {
                    lastSelectedObject = obj;
                    objectSelected = true;
                }
                selectedObject.Invoke();
            }
            if (controllerEvents() == ControllerState.UP && pickedUpObject == true) {
                if (interacionType == InteractionType.Manipulation_Movement) {
                    lastSelectedObject.transform.SetParent(null);
                    pickedUpObject = false;
                    droppedObject.Invoke();
                }
                objectSelected = false;               
            }
        }
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

    private GameObject manipulationIcons;

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
        if (interacionType == InteractionType.Manipulation_Full) {

            //this.gameObject.AddComponent<ColorPicker>();
            //this.GetComponent<ColorPicker>().trackedObj = trackedObj;
            this.gameObject.AddComponent<SelectionManipulation>();
            this.GetComponent<SelectionManipulation>().trackedObj = trackedObj;
            manipulationIcons = GameObject.Find("Manipulation_Icons");
            this.GetComponent<SelectionManipulation>().manipulationIcons = manipulationIcons;
        }

    }

    void Start() {
        laser = Instantiate(laserPrefab);
        laserTransform = laser.transform;
    }

    void Update() {
#if SteamVR_Legacy
        controller = SteamVR_Controller.Input((int)trackedObj.index);
#endif
        mirroredObject();
        ShowLaser();
        RaycastHit hit;
        if (Physics.Raycast(trackedObj.transform.position, trackedObj.transform.forward, out hit, 100)) {
            hitPoint = hit.point;
            PickupObject(hit.transform.gameObject);
            ShowLaser(hit);
        }
    }

}
