using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewPRISIM : MonoBehaviour {

	private SteamVR_TrackedObject trackedObj;
	public GameObject theController;

	private GameObject collidingObject;
	private GameObject objectInHand;

	// Track last position of controller to get the direction it is moving
	private Vector3 lastPosition;

	private SteamVR_Controller.Device Controller
	{
		get { return SteamVR_Controller.Input((int)trackedObj.index); }
	}

	public float minV = 0.1f;
	public float scaledMotionVeclocity = 0.2f;
	public float maxV = 0.4f;
	public float scaledMotionMultiplier = 0.5f;

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
        collidingObject = null;
    }

	// Need to change this one
	private void pickUpObject()
    {
        
        objectInHand = collidingObject;
        collidingObject = null;

		// Set objectsPosition to hand
		objectInHand.transform.position = this.transform.position;
	}

	private void ReleaseObject()
    {  
        objectInHand = null;
    }
	
	// Use this for initialization
	void Start () {
		lastPosition = this.transform.position;
	}
	
	private void updateLastPosition() {
		lastPosition = this.transform.position;
	}

	private Vector3 getDirectionControllerMoving() {
		return (this.transform.position - lastPosition).normalized;
	}

	private float getDistanceTraveledSinceLastPosition() {
		return Vector3.Distance(this.transform.position, lastPosition);
	}

	private float getDistanceTraveledX() {
		return this.transform.position.x - lastPosition.x;
	}

	private float getDistanceTraveledY() {
		return this.transform.position.y - lastPosition.y;
	}

	private float getDistanceTraveledZ() {
		return this.transform.position.z - lastPosition.z;
	}

	// Update is called once per frame
	void Update () {
		if (Controller.GetHairTriggerDown())
        {
            if (collidingObject)
            {
                pickUpObject();
            }
        }


        if (Controller.GetHairTriggerUp())
        {
            if (objectInHand)
            {
                ReleaseObject();
            }
        }

		if(objectInHand != null) {
			float distanceTraveled = getDistanceTraveledSinceLastPosition();
			Vector3 directionTraveled = getDirectionControllerMoving();
			// Move objectInHandWithHand
		}
		updateLastPosition();
	}
}
