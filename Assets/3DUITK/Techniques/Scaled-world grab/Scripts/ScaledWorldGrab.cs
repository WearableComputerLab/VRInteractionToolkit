using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Valve.VR;

public class ScaledWorldGrab : MonoBehaviour {

    /* Scaled-world grab implementation by Kieran May
     * University of South Australia
     * 
     * The Scaled-world grab algorithm used was based off: (pg 37) https://people.cs.vt.edu/~bowman/3dui.org/course_notes/siggraph2001/basic_techniques.pdf 
     * The initial selection technique used in this implementation is ray-casting
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
    internal SteamVR_Controller.Device controller;
#elif SteamVR_2
    internal SteamVR_Behaviour_Pose trackedObj;
    public SteamVR_Action_Boolean m_controllerPress;
    public SteamVR_Action_Boolean m_menuButton;
#else
    public GameObject trackedObj;
#endif

    public GameObject controllerCollider;
    public LayerMask interactionLayers;

    public GameObject controllerRight;
    public GameObject controllerLeft;


    private GameObject mirroredCube;
    public GameObject laserPrefab;
    private GameObject laser;
    private Transform laserTransform;
    private Vector3 hitPoint;

    public enum InteractionType { Selection, Manipulation_Movement};
    public InteractionType interacionType;
    internal GameObject tempObjectStored;

    public enum ControllerPicked { Left_Controller, Right_Controller };
    public ControllerPicked controllerPicked;

    private void ShowLaser(RaycastHit hit) {
        if (isInteractionlayer(hit.transform.gameObject))
        {
            mirroredCube.SetActive(false);
            laser.SetActive(true);
            laserTransform.position = Vector3.Lerp(trackedObj.transform.position, hitPoint, .5f);
            laserTransform.LookAt(hitPoint);
            laserTransform.localScale = new Vector3(laserTransform.localScale.x, laserTransform.localScale.y, hit.distance);
            InstantiateObject(hit.transform.gameObject);
        }
    }


    internal bool objSelected = false;
    public GameObject cameraHead;
    public  GameObject cameraRig;
    //private GameObject virtualHand;
    public GameObject selectedObject;
    private Transform oldParent;
    float Disteh;
    float Disteo;
    float scaleAmount;

    Vector3 oldHeadScale;
    Vector3 oldCameraRigScale;

    //Tham's scale method
    public void ScaleAround(Transform target, Transform pivot, Vector3 scale) {
        Transform pivotParent = pivot.parent;
        Vector3 pivotPos = pivot.position;
        pivot.parent = target;
        target.localScale = scale;
        target.position += pivotPos - pivot.position;
        pivot.parent = pivotParent;
    }

    public enum ControllerState {
        TRIGGER_UP, TRIGGER_DOWN, APPLICATION_MENU, NONE
    }

    public ControllerState controllerEvents() {
#if SteamVR_Legacy
        if (controller.GetHairTriggerDown()) {
            return ControllerState.TRIGGER_DOWN;
        } if (controller.GetHairTriggerUp()) {
            return ControllerState.TRIGGER_UP;
        } if (controller.GetPressDown(SteamVR_Controller.ButtonMask.ApplicationMenu)) {
            return ControllerState.APPLICATION_MENU;
        }
#elif SteamVR_2
        if (m_controllerPress.GetStateDown(trackedObj.inputSource)) {
            return ControllerState.TRIGGER_DOWN;
        } if (m_controllerPress.GetStateUp(trackedObj.inputSource)) {
            return ControllerState.TRIGGER_UP;
        } if (m_menuButton.GetStateDown(trackedObj.inputSource)) {
            return ControllerState.APPLICATION_MENU;
        }
#endif
        return ControllerState.NONE;
    }

    private void InstantiateObject(GameObject obj) {
        if (controllerEvents() == ControllerState.TRIGGER_DOWN) {
            if (!objSelected && obj.transform.name != "Mirrored Cube") {             
                selectedObject = obj;
                oldParent = selectedObject.transform.parent;
                objSelected = true;
                laser.SetActive(false);
                cameraHeadLocalScaleOriginal = cameraHead.transform.localScale;
                cameraRigLocalScaleOriginal = cameraRig.transform.localScale;
                cameraRigLocalPositionOriginal = cameraRig.transform.localPosition;
                Disteh = Vector3.Distance(cameraHead.transform.position, trackedObj.transform.position);
                Disteo = Vector3.Distance(cameraHead.transform.position, obj.transform.position);
                print("cameraHead:"+ cameraHead.transform.position);
                print("hand:" + trackedObj.transform.position);
                print("object:" + obj.transform.localPosition);

                scaleAmount = Disteo / Disteh;
                print("scale amount:" + scaleAmount);
                oldHeadScale = cameraHead.transform.localScale;
                oldCameraRigScale = cameraRig.transform.localScale;
                //cameraHead.transform.localScale = new Vector3(2f, 2f, 2f);
                ScaleAround(cameraRig.transform, cameraHead.transform, new Vector3(scaleAmount, scaleAmount, scaleAmount));

                Vector3 eyeProportion = cameraHead.transform.localScale / scaleAmount;
                //Keep eye distance proportionate to original position
                cameraHead.transform.localScale = eyeProportion;
                //ScaleAround(cameraRig.transform, cameraHead.transform, new Vector3(4f, 4f, 4f));
                //selectedObject.transform.SetParent(trackedObj.transform);
            } else if (objSelected == true) {
                resetProperties();
            }
        }
    }

    internal bool objectGrabbed = false;
    public static int grabbedAmount;

    private GameObject entered;
    private void OnTriggerEnter(Collider col) {
        if(isInteractionlayer(col.gameObject)) {
            print("Entered" + col.name);
            entered = col.gameObject;
        }        
    }

    private void OnTriggerExit(Collider col) {
        if(isInteractionlayer(col.gameObject)) {

        }
    }

    private bool isInteractionlayer(GameObject obj) {
        return interactionLayers == (interactionLayers | (1 << obj.layer));
    }

    private Vector3 cameraHeadLocalScaleOriginal;
    private Vector3 cameraRigLocalScaleOriginal;
    private Vector3 cameraRigLocalPositionOriginal;

    internal void resetProperties() {
        objSelected = false;
        selectedObject.transform.SetParent(oldParent);
        cameraHead.transform.localScale = cameraHeadLocalScaleOriginal;
        cameraRig.transform.localScale = cameraRigLocalScaleOriginal;
        cameraRig.transform.localPosition = cameraRigLocalPositionOriginal;
    }

    private void WorldGrab() {
        /*
        //print("updating frame..");
        virtualHand.transform.localEulerAngles = trackedObj.transform.localEulerAngles;
        //each frame
        float Disthcurr = Vector3.Distance(trackedObj.transform.position, cameraHead.transform.position); // Physical hand distance
        float Distvh = Disthcurr * (Disto / Disth); // Virtual hand distance
        Vector3 thcurr = (trackedObj.transform.position - cameraHead.transform.position);
        Vector3 VirtualHandPos = cameraHead.transform.position + Distvh * (thcurr);
        virtualHand.transform.position = VirtualHandPos;
        */
        if (controllerEvents() == ControllerState.APPLICATION_MENU) { // temp
            //Resetting everything back to normal
            resetProperties();
        }
    }

    private void castRay() {
        mirroredObject();
        ShowLaser();
        RaycastHit hit;
        if (Physics.Raycast(trackedObj.transform.position, trackedObj.transform.forward, out hit, 100)) {
            hitPoint = hit.point;
            ShowLaser(hit);
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

    private void ShowLaser() {
        laser.SetActive(true);
        mirroredCube.SetActive(true);
        Vector3 theVector = trackedObj.transform.forward;
        hitPoint = trackedObj.transform.position;
        float distance_formula_on_vector = Mathf.Sqrt(theVector.x * theVector.x + theVector.y * theVector.y + theVector.z * theVector.z);
        // Using formula to find a point which lies at distance on a 3D line from vector and direction
        hitPoint.x = hitPoint.x + (100 / (distance_formula_on_vector)) * theVector.x;
        hitPoint.y = hitPoint.y + (100 / (distance_formula_on_vector)) * theVector.y;
        hitPoint.z = hitPoint.z + (100 / (distance_formula_on_vector)) * theVector.z;

        laser.SetActive(true);
        laserTransform.position = Vector3.Lerp(trackedObj.transform.position, hitPoint, .5f);
        laserTransform.LookAt(hitPoint);
        laserTransform.localScale = new Vector3(laserTransform.localScale.x, laserTransform.localScale.y,
           100);
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
        controllerCollider.transform.parent = trackedObj.transform;
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
        if(controllerEvents() == ControllerState.TRIGGER_UP && objectGrabbed && objSelected) {          
            selectedObject.gameObject.transform.SetParent(null);
            objectGrabbed = false;
            resetProperties();            
        }
        if (objSelected == false) {
            castRay();
        } else if (objSelected) {
            WorldGrab();
        }
    }
}
