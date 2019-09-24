using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Valve.VR;

public class GrabObject : MonoBehaviour {
#if SteamVR_Legacy
    public SteamVR_TrackedObject trackedObj;
    private SteamVR_Controller.Device Controller {
        get {
            return SteamVR_Controller.Input((int)trackedObj.index);
        }
    }
#elif SteamVR_2
    public SteamVR_Behaviour_Pose trackedObj;
    public SteamVR_Action_Boolean m_controllerPress;
#endif

    public LayerMask interactionLayers;

    // Allows to choose if the script purley selects or has full manipulation
    public enum InteractionType { Selection, Manipulation };
    public InteractionType interactionType;
    public GameObject selection = null; // holds the selected object

    public GameObject collidingObject;
    private GameObject objectInHand;

    public UnityEvent selectedObject; // Invoked when an object is selected

    public UnityEvent hovered; // Invoked when an object is hovered by technique
    public UnityEvent unHovered; // Invoked when an object is no longer hovered by the technique


    void OnEnable() {
        var render = SteamVR_Render.instance;
        if (render == null) {
            enabled = false;
            return;
        }
    }

    void Awake() {
        //trackedObj = GetComponent<SteamVR_TrackedObject>();
    }

    private void SetCollidingObject(Collider col) {

        if (collidingObject || !col.GetComponent<Rigidbody>()) {
            return;
        }

        collidingObject = col.gameObject;
    }


    public void OnTriggerEnter(Collider other) {
        SetCollidingObject(other);
        if (interactionLayers == (interactionLayers | (1 << other.gameObject.layer)) && objectInHand == null) {
            hovered.Invoke();
        }
    }


    public void OnTriggerStay(Collider other) {
        SetCollidingObject(other);
    }


    public void OnTriggerExit(Collider other) {
        if (!collidingObject) {
            return;
        }
        if (interactionLayers == (interactionLayers | (1 << other.gameObject.layer))) {
            unHovered.Invoke();
        }

        collidingObject = null;
    }

    private void pickUpObject() {
        objectInHand = collidingObject;

        collidingObject = null;

        var joint = AddFixedJoint();
        joint.connectedBody = objectInHand.GetComponent<Rigidbody>();
    }

    private FixedJoint AddFixedJoint() {
        FixedJoint fx = gameObject.AddComponent<FixedJoint>();
        fx.breakForce = 20000;
        fx.breakTorque = 20000;
        return fx;
    }

    private void ReleaseObject() {

        if (GetComponent<FixedJoint>()) {

            GetComponent<FixedJoint>().connectedBody = null;
            Destroy(GetComponent<FixedJoint>());
#if SteamVR_Legacy
            objectInHand.GetComponent<Rigidbody>().velocity = Controller.velocity;
            objectInHand.GetComponent<Rigidbody>().angularVelocity = Controller.angularVelocity;
#elif SteamVR_2
            objectInHand.GetComponent<Rigidbody>().velocity = trackedObj.GetVelocity();
            objectInHand.GetComponent<Rigidbody>().angularVelocity = trackedObj.GetAngularVelocity();
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
        if (m_controllerPress.GetStateDown(trackedObj.inputSource)) {
            return ControllerState.TRIGGER_DOWN;
        }
        if (m_controllerPress.GetStateUp(trackedObj.inputSource)) {
            return ControllerState.TRIGGER_UP;
        }
#endif
        return ControllerState.NONE;
    }

    // Update is called once per frame
    void Update() {
        if (controllerEvents() == ControllerState.TRIGGER_DOWN) {
            if (collidingObject && interactionLayers == (interactionLayers | (1 << collidingObject.gameObject.layer))) {
                selectedObject.Invoke();
                if (interactionType == InteractionType.Selection) {
                    // Pure selection
                    print("selected " + collidingObject);
                    selection = collidingObject;
                } else {
                    // Manipulation
                    pickUpObject();
                    selection = collidingObject;
                }

            }
        }

        if (controllerEvents() == ControllerState.TRIGGER_UP) {
            if (objectInHand) {
                ReleaseObject();
            }
        }
    }
}
