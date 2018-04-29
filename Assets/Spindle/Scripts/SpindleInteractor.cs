using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpindleInteractor : MonoBehaviour {

    public SteamVR_TrackedObject trackedObj1;
    public SteamVR_TrackedObject trackedObj2;

    private SteamVR_Controller.Device Controller1
    {
        get { return SteamVR_Controller.Input((int)trackedObj1.index); }
    }

    private float distanceBetweenControllersOnPickup;
    private Vector3 objectScaleOnPickup;

    private SteamVR_Controller.Device Controller2
    {
        get { return SteamVR_Controller.Input((int)trackedObj2.index); }
    }

    // Pickup Vars
    private GameObject collidingObject;
    private GameObject collidingObjectHighlighted;
    private GameObject objectInHand;
    private bool pickedUpWith1 = false;
    private bool pickedUpWith2 = false;

    // Use this for initialization
    void Start () {
		
	}
	
    void adjustScale()
    {

        float currentDistanceBetweenControllers = Vector3.Distance(trackedObj1.transform.position, trackedObj2.transform.position);
        float changeInDistance = (distanceBetweenControllersOnPickup - currentDistanceBetweenControllers) * -1;
        objectInHand.transform.localScale = new Vector3(objectScaleOnPickup.x + changeInDistance, objectScaleOnPickup.y + changeInDistance, objectScaleOnPickup.z + changeInDistance);
    }

    void pickupWithController(SteamVR_Controller.Device theController)
    {
        if(theController == null)
        {
            return;
        }
        if (theController.GetHairTriggerDown())
        {
            if (collidingObject)
            {
                GrabObject();
                distanceBetweenControllersOnPickup = Vector3.Distance(trackedObj1.transform.position, trackedObj2.transform.position);
                objectScaleOnPickup = objectInHand.transform.localScale;
            }
            if(theController.Equals(Controller1))
            {
                pickedUpWith1 = true;
            } else
            {
                pickedUpWith2 = true;
            }         
        }

        if (theController.GetHairTriggerUp())
        {
            if (objectInHand)
            {
                ReleaseObject();
            }
            if (theController.Equals(Controller1))
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
            pickupWithController(Controller1);
            adjustScale();

        } else  if (pickedUpWith2)
        {
            pickupWithController(Controller2);
            adjustScale();

        } else if(!pickedUpWith1 && !pickedUpWith2)
        {
            pickupWithController(Controller1);
            if(!pickedUpWith1)
            {
                pickupWithController(Controller2);
            }
        }
    }

    // Pickup methods
    private void SetCollidingObject(Collider col)
    {

        if (collidingObject || !col.GetComponent<Rigidbody>())
        {
            return;
        }

        collidingObject = col.gameObject;
    }


    public void OnTriggerEnter(Collider other)
    {
        collidingObjectHighlighted = other.transform.gameObject.transform.GetChild(0).gameObject;
        if (collidingObjectHighlighted != null)
        {
            collidingObjectHighlighted.SetActive(true);
        }
        SetCollidingObject(other);
    }


    public void OnTriggerStay(Collider other)
    {
        SetCollidingObject(other);
    }


    public void OnTriggerExit(Collider other)
    {
        if (!collidingObject)
        {
            return;
        }

        if (collidingObjectHighlighted != null)
        {
            collidingObjectHighlighted.SetActive(false);
            collidingObjectHighlighted = null;
        }

        collidingObject = null;
    }

    private void GrabObject()
    {

        objectInHand = collidingObject;
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

            objectInHand.GetComponent<Rigidbody>().velocity = Controller1.velocity;
            objectInHand.GetComponent<Rigidbody>().angularVelocity = Controller1.angularVelocity;
        }

        objectInHand = null;
    }
}
