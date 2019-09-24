using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Valve.VR;

public class HOMER : MonoBehaviour {

    /* HOMER implementation by Kieran May
     * University of South Australia
     * 
     * The developed HOMER algorithm was based off: (pg 34-35) https://people.cs.vt.edu/~bowman/3dui.org/course_notes/siggraph2001/basic_techniques.pdf 
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

    public GameObject controllerRight;
    public GameObject controllerLeft;
    public GameObject cameraHead; // t

    private GameObject mirroredCube;
    public GameObject laserPrefab;
    private GameObject laser;
    private Transform laserTransform;
    private Vector3 hitPoint;

    public enum InteractionType { Selection, Manipulation_Movement};
    public InteractionType interacionType;

    public enum ControllerPicked { Left_Controller, Right_Controller };
    public ControllerPicked controllerPicked;

    public UnityEvent selectedObjectEvent; // Invoked when an object is selected
    public UnityEvent droppedObject; // Invoked when an object is dropped
    public UnityEvent hovered; // Invoked when an object is hovered by technique
    public UnityEvent unHovered; // Invoked when an object is no longer hovered by the technique

    private void ShowLaser(RaycastHit hit) {
        mirroredCube.SetActive(false);
        laser.SetActive(true);
        laserTransform.position = Vector3.Lerp(trackedObj.transform.position, hitPoint, .5f);
        laserTransform.LookAt(hitPoint);
        laserTransform.localScale = new Vector3(laserTransform.localScale.x, laserTransform.localScale.y, hit.distance);
        if (interactionLayers == (interactionLayers | (1 << hit.transform.gameObject.layer))) {
            hoveredObject = hit.transform.gameObject;
            unHovered.Invoke();
            hovered.Invoke();
            InstantiateObject(hit.transform.gameObject);
        }
    }

    public enum ControllerState {
        TRIGGER_UP, TRIGGER_DOWN, NONE
    }

    private ControllerState controllerEvents() {
#if SteamVR_Legacy
        if (controller.GetPressDown(Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger)) {
            return ControllerState.TRIGGER_DOWN;
        }
        if (controller.GetPressUp(Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger)) {
            return ControllerState.TRIGGER_UP;
        }
#elif SteamVR_2
        if (m_controllerPress.GetStateDown(trackedObj.inputSource)) {
            return ControllerState.TRIGGER_DOWN;
        }
        if (m_controllerPress.GetStateUp(trackedObj.inputSource)) {
            return ControllerState.TRIGGER_UP;
        }
#endif
        return ControllerState.NONE;
    }

    float Disth = 0f;
    float Disto = 0f;
    bool objSelected = false;
    private GameObject virtualHand;
    public GameObject selectedObject;
    public GameObject handPrefab;
    private Transform oldParent;
    public GameObject hoveredObject;

    private void InstantiateObject(GameObject obj) {
        if (controllerEvents() == ControllerState.TRIGGER_DOWN) {
            virtualHand = Instantiate(new GameObject("hand"));
            virtualHand.transform.position = obj.transform.position;
            virtualHand.SetActive(true);
            selectedObject = obj;
            oldParent = selectedObject.transform.parent;
            objSelected = true;
            selectedObject.transform.SetParent(virtualHand.transform);
            laser.SetActive(false);
            selectedObjectEvent.Invoke();

            Disth = Vector3.Distance(trackedObj.transform.position, cameraHead.transform.position);
            Disto = Vector3.Distance(obj.transform.position, cameraHead.transform.position);
        }
    }

    private void HomerFormula() {
        //print("updating frame..");
        virtualHand.transform.localEulerAngles = trackedObj.transform.localEulerAngles;
        //each frame
        float Disthcurr = Vector3.Distance(trackedObj.transform.position, cameraHead.transform.position); // Physical hand distance
        float Distvh = Disthcurr * (Disto / Disth); // Virtual hand distance
        Vector3 thcurr = (trackedObj.transform.position - cameraHead.transform.position);
        Vector3 VirtualHandPos = cameraHead.transform.position + Distvh * (thcurr);
        virtualHand.transform.position = VirtualHandPos;
        virtualHand.transform.position = new Vector3(virtualHand.transform.position.x, virtualHand.transform.position.y, virtualHand.transform.position.z);

        if (controllerEvents() == ControllerState.TRIGGER_DOWN) {
            objSelected = false;
            Destroy(virtualHand);
            selectedObject.transform.SetParent(oldParent);
            droppedObject.Invoke();
        }
    }

    private void castRay() {
        ShowLaser();
        RaycastHit hit;
        if (Physics.Raycast(trackedObj.transform.position, trackedObj.transform.forward, out hit, 100)) {
            hitPoint = hit.point;
            ShowLaser(hit);
        } else {
            unHovered.Invoke();
        }
    }

    void moveMirroredCube() {
        //getControllerPosition();
        Vector3 mirroredPos = trackedObj.transform.position;
        Vector3 controllerPos = trackedObj.transform.forward;
        float distance_formula_on_vector = Mathf.Sqrt(controllerPos.x * controllerPos.x + controllerPos.y * controllerPos.y + controllerPos.z * controllerPos.z);

        mirroredPos.x = mirroredPos.x + (100f / (distance_formula_on_vector)) * controllerPos.x;
        mirroredPos.y = mirroredPos.y + (100f / (distance_formula_on_vector)) * controllerPos.y;
        mirroredPos.z = mirroredPos.z + (100f / (distance_formula_on_vector)) * controllerPos.z;
        mirroredCube.transform.position = mirroredPos;
        mirroredCube.transform.rotation = trackedObj.transform.rotation;
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
        //cameraHead = GameObject.Find("Camera (eye)");
        mirroredCube = this.transform.Find("Mirrored Cube").gameObject;
        initializeControllers();
    }

    // Use this for initialization
    void Start() {
        laser = Instantiate(laserPrefab);
        laserTransform = laser.transform;
    }

    // Update is called once per frame
    void Update() {
#if SteamVR_Legacy
        controller = SteamVR_Controller.Input((int)trackedObj.index);
#endif
        if (objSelected == false) {
            moveMirroredCube();
            castRay();
        } else if (objSelected == true) {
            HomerFormula();
        }
    }
}