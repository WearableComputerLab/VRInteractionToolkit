/*
 *  Absolute and relative mapping is in the form of a Raycast (However It could 
 *  be adapted to a simple hand technique). When you press the set button (touchpad) the movement 
 *  of the virtual controller relative to your real controller is scaled with a ration of 10:1. 
 *  By doing this you can be precise when selecting small or distant objects with the ray. This is
 *  because the distance you have to move your hand across an object is amplified 10x due to the ratio.
 *  
 *  Copyright(C) 2018  Ian Hanan
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Valve.VR;

public class ARMLaser : MonoBehaviour {

#if SteamVR_Legacy
    private SteamVR_TrackedObject trackedObj;

    private SteamVR_Controller.Device Controller {
        get {
            return SteamVR_Controller.Input((int)trackedObj.index);
        }
    }

#elif SteamVR_2
    private SteamVR_Behaviour_Pose trackedObj;
    public SteamVR_Action_Boolean m_controllerPress;
    public SteamVR_Action_Boolean m_touchpadPress;

#else
    public GameObject trackedObj;
#endif

    public LayerMask interactionLayers;

    public GameObject theController;
    public GameObject laserPrefab;
    private GameObject laser;
    private Transform laserTransform;
    private Vector3 hitPoint;
    private bool ARMOn = false;
    private Vector3 lastDirectionPointing;
    private Quaternion lastRotation;
    private Vector3 lastPosition;
    public GameObject theModel;

    public enum InteractionType { Selection, Manipulation, Manipulation_UI};

    public InteractionType interactionType = InteractionType.Selection;
    public GameObject lastSelectedObject; // holds the selected object

    public GameObject currentlyPointingAt;

    public UnityEvent selectedObject; // Invoked when an object is selected
    public UnityEvent hovered; // Invoked when an object is hovered by technique
    public UnityEvent unHovered; // Invoked when an object is no longer hovered by the technique


    // Using the hack from gogo shadow - will have to fix them all once find a better way
    void makeModelChild() {
        if (this.transform.childCount == 0) {
            if (theModel.GetComponent<SteamVR_RenderModel>() != null) { // The steamVR_RenderModel is generated after code start so we cannot parent right away or it wont generate. 
                if (theModel.transform.childCount > 0) {
                    theModel.transform.parent = this.transform;
                    // Due to the transfer happening at a random time down the line we need to re-align the model inside the shadow controller to 0 so nothing is wonky.
                    theModel.transform.localPosition = Vector3.zero;
                    theModel.transform.localRotation = Quaternion.identity;
                }
            } else {
                // If it is just a custom model we can immediately parent
                theModel.transform.parent = this.transform;
                // Due to the transfer happening at a random time down the line we need to re-align the model inside the shadow controller to 0 so nothing is wonky.
                theModel.transform.localPosition = Vector3.zero;
                theModel.transform.localRotation = Quaternion.identity;
            }
        }
    }

    private void ShowLaser(RaycastHit hit) {
        laser.SetActive(true);
        laserTransform.position = Vector3.Lerp(this.transform.position, hitPoint, .5f);
        laserTransform.LookAt(hitPoint);
        laserTransform.localScale = new Vector3(laserTransform.localScale.x, laserTransform.localScale.y,
            hit.distance);


        // highlighting the object
        if (interactionLayers == (interactionLayers | (1 << hit.transform.gameObject.layer))) {
            if (currentlyPointingAt == null) {
                // no object previouslly was highlighted so just highlight this one
                currentlyPointingAt = hit.transform.gameObject;
                hovered.Invoke();
            } else if (hit.transform.gameObject != currentlyPointingAt) {
                // unhighlight previous one and highlight this one
                unHovered.Invoke();
                currentlyPointingAt = hit.transform.gameObject;
                hovered.Invoke();
            }
        } else {
            unHovered.Invoke();
        }
    }

    private void ShowLaser() {
        // removing highlight from previously highlighted object
        if (currentlyPointingAt != null) {
            // remove highlight from previously highlighted object 
            unHovered.Invoke();
            currentlyPointingAt = null;
        }

        // This is to make it extend infinite. There is DEFINATELY an easier way to do this. Find it later!
        Vector3 theVector = this.transform.forward;
        hitPoint = this.transform.position;
        float distance_formula_on_vector = Mathf.Sqrt(theVector.x * theVector.x + theVector.y * theVector.y + theVector.z * theVector.z);
        // Using formula to find a point which lies at distance on a 3D line from vector and direction
        hitPoint.x = hitPoint.x + (100 / (distance_formula_on_vector)) * theVector.x;
        hitPoint.y = hitPoint.y + (100 / (distance_formula_on_vector)) * theVector.y;
        hitPoint.z = hitPoint.z + (100 / (distance_formula_on_vector)) * theVector.z;

        laser.SetActive(true);
        laserTransform.position = Vector3.Lerp(this.transform.position, hitPoint, .5f);
        laserTransform.LookAt(hitPoint);
        laserTransform.localScale = new Vector3(laserTransform.localScale.x, laserTransform.localScale.y,
           100);
    }

    void Awake() {
#if SteamVR_Legacy
        trackedObj = theController.GetComponent<SteamVR_TrackedObject>();
#elif SteamVR_2
        trackedObj = theController.GetComponent<SteamVR_Behaviour_Pose>();
#endif
        if (interactionType == InteractionType.Manipulation_UI) {
            this.gameObject.AddComponent<SelectionManipulation>();
            this.GetComponent<SelectionManipulation>().trackedObj = trackedObj;
        }
    }

    // Use this for initialization
    void Start() {

        laser = Instantiate(laserPrefab);
        laserTransform = laser.transform;

        lastDirectionPointing = trackedObj.transform.forward;
        lastRotation = trackedObj.transform.rotation;
        lastPosition = trackedObj.transform.position;
    }

    void toggleARM() {
        if (!ARMOn) {
            lastDirectionPointing = trackedObj.transform.forward;
            lastRotation = trackedObj.transform.rotation;
            lastPosition = trackedObj.transform.position;
        }
        ARMOn = !ARMOn;
    }

    void updatePositionAndRotationToFollowController() {
        this.transform.position = trackedObj.transform.position;
        Quaternion rotationOfDevice = trackedObj.transform.rotation;
        if (ARMOn) {

            // scaled down by factor of 10
            this.transform.rotation = Quaternion.Lerp(lastRotation, trackedObj.transform.rotation, 0.1f);
            this.transform.position = Vector3.Lerp(lastPosition, trackedObj.transform.position, 0.1f);
            print("On");
        } else {
            this.transform.rotation = trackedObj.transform.rotation;
            this.transform.position = trackedObj.transform.position;
        }
    }

    public enum ControllerState {
        TRIGGER_DOWN, TOUCHPAD_DOWN, NONE
    }

    private ControllerState controllerEvents() {
#if SteamVR_Legacy
        if (Controller.GetPressDown(SteamVR_Controller.ButtonMask.Touchpad)) {
            return ControllerState.TOUCHPAD_DOWN;
        }
        if (Controller.GetHairTriggerDown()) {
            return ControllerState.TRIGGER_DOWN;
        }
#elif SteamVR_2
        if (m_controllerPress.GetStateDown(trackedObj.inputSource)) {
            return ControllerState.TRIGGER_DOWN;
        }
        if (m_touchpadPress.GetStateDown(trackedObj.inputSource)) {
            return ControllerState.TOUCHPAD_DOWN;
        }
#endif
        return ControllerState.NONE;
    }

    // Update is called once per frame
    void Update() {
        makeModelChild();
        updatePositionAndRotationToFollowController();

        RaycastHit hit;
        if (Physics.Raycast(this.transform.position, this.transform.forward, out hit, 100)) {
            hitPoint = hit.point;
            ShowLaser(hit);
        } else {
            ShowLaser();
        }

        if (controllerEvents() == ControllerState.TOUCHPAD_DOWN) {
            toggleARM();
        }

        // If remote trigger pulled
        if (controllerEvents() == ControllerState.TRIGGER_DOWN) {
            if (currentlyPointingAt != null) { // If pointing at an object
                if (interactionType == InteractionType.Selection) {
                    lastSelectedObject = currentlyPointingAt;
                } else if (interactionType == InteractionType.Manipulation) {
                    // No manipualtion implemented for this currently
                    lastSelectedObject = currentlyPointingAt;
                } else if (interactionType == InteractionType.Manipulation_UI) {
                    lastSelectedObject = currentlyPointingAt;
                    this.GetComponent<SelectionManipulation>().selectedObject = lastSelectedObject;
                }
                selectedObject.Invoke();
            }
        }

    }
}
