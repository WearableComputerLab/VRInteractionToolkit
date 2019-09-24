using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Valve.VR;

public class AperatureSelectionSelector : MonoBehaviour {
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
#endif

    public LayerMask interactionLayers;

    // Allows to choose if the script purley selects or has full manipulation
    public enum InteractionType { Selection, Manipulation };
    public InteractionType interactionType;
    public GameObject selection; // holds the selected object


    private GameObject trackedObj;
    private List<GameObject> collidingObjects;
    private GameObject objectInHand;
    public GameObject objectHoveredOver;

    public UnityEvent selectedObject; // Invoked when an object is selected
    public UnityEvent hovered; // Invoked when an object is hovered by technique
    public UnityEvent unHovered; // Invoked when an object is no longer hovered by the technique

    public GameObject orientationPlates;

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

    }

    private void SetCollidingObject(Collider col) {
        if (collidingObjects.Contains(col.gameObject) || !col.GetComponent<Rigidbody>()) {
            return;
        }
        collidingObjects.Add(col.gameObject);
    }


    public void OnTriggerEnter(Collider other) {
        SetCollidingObject(other);
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

    // Attempts to get object in selection by its orientation, if it fails will return null
    public GameObject getByOrientation() {
        // TODO: add orientational check
        return null;
    }

    private GameObject getObjectHoveringOver() {
        // Attempt to select the object by its orientation, if that fails it will return null and in that case select via 
        // closest object cone algorithm below it
        GameObject orientationSelection;
        if ((orientationSelection = getByOrientation()) != null) {
            return orientationSelection;
        }

        List<double> distancesFromCenterOfCone = new List<double>();
        List<GameObject> viableObjects = new List<GameObject>();

        Vector3 forwardVectorFromRemote = trackedObj.transform.forward;
        Vector3 positionOfRemote = trackedObj.transform.position;

        foreach (GameObject potentialObject in collidingObjects) {

            // Only check for objects on the interaction layer
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
                selectedObject.Invoke();
                if (interactionType == InteractionType.Selection) {
                    // Pure selection
                    selection = objectHoveredOver;
                } else if (interactionType == InteractionType.Manipulation) {
                    //Manipulation
                    GrabObject();
                }
                selection = objectHoveredOver;
            }
        }


        if (controllerEvents() == ControllerState.TRIGGER_UP) {
            if (objectInHand) {
                ReleaseObject();
            }
        }
    }
}
