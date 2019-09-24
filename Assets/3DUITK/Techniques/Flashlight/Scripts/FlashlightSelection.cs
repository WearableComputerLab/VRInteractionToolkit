/*
 *  Script to be attatched to controllers to allow the flashlight
 *  for the VR technique for the Flaslight to pick up objects as described
 *  for the flaslight for the HTC Vive.
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

/*
 *  Keeping info on things I have done here for now
 *  So that each flashlight has no collision with eachother put them on seperate layers
*/
public class FlashlightSelection : MonoBehaviour {

#if SteamVR_Legacy
    public SteamVR_TrackedObject theController;

    private SteamVR_Controller.Device Controller {
        get {
            return SteamVR_Controller.Input((int)theController.index);
        }
    }
#elif SteamVR_2
    public SteamVR_Behaviour_Pose theController;
    public SteamVR_Action_Boolean m_controllerPress;
#else
    public GameObject theController;
#endif

    public LayerMask interactionLayers;

    // Allows to choose if the script purley selects or has full manipulation
    public enum InteractionType { Selection, Manipulation, Manipulation_UI};
    public InteractionType interactionType;
    public GameObject selection; // holds the selected object
    private GameObject trackedObj;
    private List<GameObject> collidingObjects;
    private GameObject objectInHand;
    public GameObject objectHoveredOver;

    public UnityEvent selectedObject; // Invoked when an object is selected
    public UnityEvent hovered; // Invoked when an object is hovered by technique
    public UnityEvent unHovered; // Invoked when an object is no longer hovered by the technique

    // Checks if holding object in hand
    public bool holdingObject() {
        return objectInHand != null;
    }

    void OnEnable() {
        collidingObjects = new List<GameObject>();
        trackedObj = this.transform.gameObject;
        var render = SteamVR_Render.instance;
        if (render == null) {
            enabled = false;
            return;
        }
    }

    void Awake() {
        //trackedObj = GetComponent<SteamVR_TrackedObject>();
        if (interactionType == InteractionType.Manipulation_UI) {
            this.gameObject.AddComponent<SelectionManipulation>();
            this.GetComponent<SelectionManipulation>().trackedObj = theController;
#if SteamVR_2
            this.GetComponent<SelectionManipulation>().m_controllerPress = m_controllerPress;
#endif
        }
    }

    private void SetCollidingObject(Collider col) {
        if (collidingObjects.Contains(col.gameObject)) {
            return;
        }
        collidingObjects.Add(col.gameObject);
    }


    public void OnTriggerEnter(Collider other) {
        if (interactionLayers == (interactionLayers | (1 << other.gameObject.layer))) {
            SetCollidingObject(other);
        }
    }


    public void OnTriggerStay(Collider other) {
        SetCollidingObject(other);
    }


    public void OnTriggerExit(Collider other) {
        if (!collidingObjects.Contains(other.gameObject)) {
            return;
        }
        collidingObjects.Remove(other.gameObject);
    }

    private GameObject getObjectHoveringOver() {
        List<double> distancesFromCenterOfCone = new List<double>();
        List<GameObject> viableObjects = new List<GameObject>();

        Vector3 forwardVectorFromRemote = trackedObj.transform.forward;
        Vector3 positionOfRemote = trackedObj.transform.position;

        foreach (GameObject potentialObject in collidingObjects) {

            // dont have to worry about executing twice as an object can only be on one layer
            if (interactionLayers == (interactionLayers | (1 << potentialObject.layer))) {
                // Object can only have one layer so can do calculation for object here
                Vector3 objectPosition = potentialObject.transform.position;

                // Using vector algebra to get shortest distance between object and vector 
                Vector3 forwardControllerToObject = trackedObj.transform.position - objectPosition;
                Vector3 controllerForward = trackedObj.transform.forward;
                float distanceBetweenRayAndPoint = Vector3.Magnitude(Vector3.Cross(forwardControllerToObject, controllerForward)) / Vector3.Magnitude(controllerForward);

                distancesFromCenterOfCone.Add(distanceBetweenRayAndPoint);
                viableObjects.Add(potentialObject);
            }

        }

        if (viableObjects.Count > 0 && distancesFromCenterOfCone.Count > 0) {
            // Find the smallest object by distance
            int indexOfSmallest = 0;
            double smallest = distancesFromCenterOfCone[0];
            for (int index = 0; index < distancesFromCenterOfCone.Count; index++) {
                if (distancesFromCenterOfCone[index] < smallest) {
                    indexOfSmallest = index;
                    smallest = distancesFromCenterOfCone[index];
                }
            }

            if (objectHoveredOver != viableObjects[indexOfSmallest]) {
                unHovered.Invoke();
            }

            return viableObjects[indexOfSmallest];
        }

        unHovered.Invoke();
        return null;
    }

    private void GrabObject() {
        if (!objectHoveredOver.GetComponent<Rigidbody>()) {
            return;
        }

        objectInHand = objectHoveredOver;

        if (objectHoveredOver != null) {
            collidingObjects.Remove(objectInHand);

            var joint = AddFixedJoint();
            objectInHand.GetComponent<Rigidbody>().velocity = Vector3.zero; // Setting velocity to 0 so can catch without breakforce effecting it
            joint.connectedBody = objectInHand.GetComponent<Rigidbody>();
        }
    }

    private FixedJoint AddFixedJoint() {

        FixedJoint fx = gameObject.AddComponent<FixedJoint>();
        fx.breakForce = 1000;
        fx.breakTorque = Mathf.Infinity;
        return fx;
    }

    private void ReleaseObject() {

        if (GetComponent<FixedJoint>()) {

            GetComponent<FixedJoint>().connectedBody = null;
            Destroy(GetComponent<FixedJoint>());


            // TODO: Fix this so that it throws with the correct velocity applied
            //objectInHand.GetComponent<Rigidbody>().velocity = GetComponent<Rigidbody>().velocity;
            //objectInHand.GetComponent<Rigidbody>().angularVelocity = GetComponent<Rigidbody>().angularVelocity;


            //objectInHand.GetComponent<Rigidbody>().velocity = Controller.velocity;
            //objectInHand.GetComponent<Rigidbody>().angularVelocity = Controller.angularVelocity;
#if SteamVR_Legacy
            objectInHand.GetComponent<Rigidbody>().velocity = Controller.velocity * Vector3.Distance(Controller.transform.pos, objectInHand.transform.position);
            objectInHand.GetComponent<Rigidbody>().angularVelocity = Controller.angularVelocity;
#elif SteamVR_2
            objectInHand.GetComponent<Rigidbody>().velocity = theController.GetVelocity() * Vector3.Distance(theController.transform.position, objectInHand.transform.position);
            objectInHand.GetComponent<Rigidbody>().angularVelocity = theController.GetAngularVelocity();
#endif

        }

        objectInHand = null;
    }


    public enum ControllerState {
        TRIGGER_UP, TRIGGER_DOWN, NONE
    }

    private ControllerState controllerEvents() {
#if SteamVR_Legacy
        if (Controller.GetHairTriggerDown()) {
            return ControllerState.TRIGGER_DOWN;
        }
        if (Controller.GetHairTriggerUp()) {
            return ControllerState.TRIGGER_UP;
        }
#elif SteamVR_2
        if (m_controllerPress.GetStateDown(theController.inputSource)) {
            return ControllerState.TRIGGER_DOWN;
        } if (m_controllerPress.GetStateUp(theController.inputSource)) {
            return ControllerState.TRIGGER_UP;
        }
#endif
        return ControllerState.NONE;
    }

    // Update is called once per frame
    void Update() {

        objectHoveredOver = getObjectHoveringOver();
        hovered.Invoke();

        print(collidingObjects.Count);
        if (controllerEvents() == ControllerState.TRIGGER_DOWN) {
            if (collidingObjects.Count > 0) {
                if (interactionType == InteractionType.Selection) {
                    // Pure selection
                    print("selected " + objectHoveredOver);

                } else if (interactionType == InteractionType.Manipulation) {
                    //Manipulation
                    GrabObject();
                } else if (interactionType == InteractionType.Manipulation_UI && this.GetComponent<SelectionManipulation>().inManipulationMode == false) {
                    this.GetComponent<SelectionManipulation>().selectedObject = objectHoveredOver;
                }
                selection = objectHoveredOver;
                selectedObject.Invoke();
            }
        }


        if (controllerEvents() == ControllerState.TRIGGER_UP) {
            if (objectInHand) {
                ReleaseObject();
            }
        }
    }
}
