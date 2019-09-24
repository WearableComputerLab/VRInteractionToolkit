using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Valve.VR;

public class SpindleInteractor : MonoBehaviour {

#if SteamVR_Legacy
    public SteamVR_TrackedObject trackedObj1;
    public SteamVR_TrackedObject trackedObj2;

    private SteamVR_Controller.Device Controller1
    {
        get { return SteamVR_Controller.Input((int)trackedObj1.index); }
    }
        private SteamVR_Controller.Device Controller2
    {
        get { return SteamVR_Controller.Input((int)trackedObj2.index); }
    }
#elif SteamVR_2
    public SteamVR_Action_Boolean m_controllerPress;
    public SteamVR_Behaviour_Pose trackedObj1;
    public SteamVR_Behaviour_Pose trackedObj2;
#else
    public GameObject trackedObj1;
    public GameObject trackedObj2;
#endif

    public LayerMask interactionLayers;



    private float distanceBetweenControllersOnPickup;
    private Vector3 objectScaleOnPickup;



    // Pickup Vars
    public GameObject collidingObject;
    public GameObject objectInHand;
    private bool pickedUpWith1 = false;
    private bool pickedUpWith2 = false;

    
    public UnityEvent selectedObject; // Invoked when an object is selected

    public UnityEvent hovered; // Invoked when an object is hovered by technique
    public UnityEvent unHovered; // Invoked when an object is no longer hovered by the technique
    public UnityEvent droppedObject;


    public enum ControllerState {
        TRIGGER_UP1, TRIGGER_DOWN1, TRIGGER_UP2, TRIGGER_DOWN2, NONE
    }

    private ControllerState controllerEvents() {
#if SteamVR_Legacy
        if (Controller1.GetHairTriggerDown()) {
            return ControllerState.TRIGGER_DOWN1;
        }
        if (Controller1.GetHairTriggerUp()) {
            return ControllerState.TRIGGER_UP1;
        }
        if (Controller2.GetHairTriggerDown()) {
            return ControllerState.TRIGGER_DOWN2;
        }
        if (Controller2.GetHairTriggerUp()) {
            return ControllerState.TRIGGER_UP2;
        }
#elif SteamVR_2
        if (m_controllerPress.GetStateDown(trackedObj1.inputSource)) {
            return ControllerState.TRIGGER_DOWN1;
        }
        if (m_controllerPress.GetStateUp(trackedObj1.inputSource)) {
            return ControllerState.TRIGGER_UP1;
        }
        if (m_controllerPress.GetStateDown(trackedObj2.inputSource)) {
            return ControllerState.TRIGGER_DOWN2;
        }
        if (m_controllerPress.GetStateUp(trackedObj2.inputSource)) {
            return ControllerState.TRIGGER_UP2;
        }
#endif
        return ControllerState.NONE;
    }

    void adjustScale()
    {

        float currentDistanceBetweenControllers = Vector3.Distance(trackedObj1.transform.position, trackedObj2.transform.position);
        float changeInDistance = (distanceBetweenControllersOnPickup - currentDistanceBetweenControllers) * -1;
        objectInHand.transform.localScale = new Vector3(objectScaleOnPickup.x + changeInDistance, objectScaleOnPickup.y + changeInDistance, objectScaleOnPickup.z + changeInDistance);
    }

    void pickupWithController()
    {
        if(trackedObj1 == null && trackedObj2 == null)
        {
            return;
        }
        if (controllerEvents() == ControllerState.TRIGGER_DOWN1 || controllerEvents() == ControllerState.TRIGGER_DOWN2) {
            if (collidingObject)
            {
                GrabObject();
                distanceBetweenControllersOnPickup = Vector3.Distance(trackedObj1.transform.position, trackedObj2.transform.position);
                objectScaleOnPickup = objectInHand.transform.localScale;
            }
            if(controllerEvents() == ControllerState.TRIGGER_DOWN1) {
                pickedUpWith1 = true;
            } else
            {
                pickedUpWith2 = true;
            }         
        }

        if (controllerEvents() == ControllerState.TRIGGER_UP1 || controllerEvents() == ControllerState.TRIGGER_UP2) {
            if (objectInHand)
            {
                ReleaseObject();
            }
            if (controllerEvents() == ControllerState.TRIGGER_UP1)
            {
                pickedUpWith1 = false;
            }
            else
            {
                pickedUpWith2 = false;
            }
        }
    }

	// Update is called once per frame
	void Update () {

        if(pickedUpWith1)
        {
            pickupWithController();
            adjustScale();

        } else  if (pickedUpWith2)
        {
            pickupWithController();
            adjustScale();

        } else if(!pickedUpWith1 && !pickedUpWith2)
        {
            pickupWithController();
            if(!pickedUpWith1)
            {
                pickupWithController();
            }
        }
    }

    // Pickup methods
    private void SetCollidingObject(Collider other)
    {

		if (collidingObject || !other.GetComponent<Rigidbody>() || interactionLayers != (interactionLayers | (1 << other.gameObject.layer)))
        {
            return;
        }
        collidingObject = other.gameObject;
		hovered.Invoke ();
    }


    public void OnTriggerEnter(Collider other)
    {
        SetCollidingObject(other);
    }


    public void OnTriggerStay(Collider other)
    {
        SetCollidingObject(other);
    }


    public void OnTriggerExit(Collider other)
    {
		if (!collidingObject || interactionLayers != (interactionLayers | (1 << other.gameObject.layer)))
        {
            return;
        }
		unHovered.Invoke ();
        collidingObject = null;
    }

    private void GrabObject()
    {

        objectInHand = collidingObject;
        selectedObject.Invoke();
        collidingObject = null;

        var joint = AddFixedJoint();
        objectInHand.transform.position = this.transform.position;
        joint.connectedBody = objectInHand.GetComponent<Rigidbody>();
    }

    private FixedJoint AddFixedJoint()
    {
        FixedJoint fx = gameObject.AddComponent<FixedJoint>();
        fx.breakForce = Mathf.Infinity;
        fx.breakTorque = Mathf.Infinity;
        return fx;
    }

    private void ReleaseObject()
    {
        if (GetComponent<FixedJoint>())
        {

            GetComponent<FixedJoint>().connectedBody = null;
            Destroy(GetComponent<FixedJoint>());
#if SteamVR_Legacy
            objectInHand.GetComponent<Rigidbody>().velocity = Controller1.velocity;
            objectInHand.GetComponent<Rigidbody>().angularVelocity = Controller1.angularVelocity;
#elif SteamVR_2
            objectInHand.GetComponent<Rigidbody>().velocity = trackedObj1.GetVelocity();
            objectInHand.GetComponent<Rigidbody>().angularVelocity = trackedObj1.GetAngularVelocity();
#endif

        }
        droppedObject.Invoke();
        objectInHand = null;
    }
}
