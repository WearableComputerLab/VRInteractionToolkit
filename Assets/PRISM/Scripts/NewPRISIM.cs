using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewPRISIM : MonoBehaviour {

	private SteamVR_TrackedObject trackedObj;
	public GameObject theController;

	private GameObject collidingObject;
	private GameObject objectInHand;

	

	private SteamVR_Controller.Device Controller
	{
		get { return SteamVR_Controller.Input((int)trackedObj.index); }
	}

	// Track last position of controller to get the direction it is moving
	private Vector3 lastPosition;
	// keeping track of time passed resets every 500ms
	private float timePassedTracker;
	private float millisecondsDelayTime = 500;
	private float actualTimePassedOnLastPosition;

	public float minS = 0.1f;
	public float scaledConstant = 0.2f;
	public float maxS = 0.4f;
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
	
	// Only updates if millisecondDelayTime (500ms) has passed
	private void updateLastPosition() {
		if(timePassedTracker >= millisecondsDelayTime) {
			moveObjectInHand();
			lastPosition = this.transform.position;
			actualTimePassedOnLastPosition = timePassedTracker;
			timePassedTracker = 0;
		}
		timePassedTracker = timePassedTracker += millisecondsSinceLastUpdate();		
	}

	private void moveObjectInHand() {
		if(objectInHand != null && lastPosition != null) {
			Vector3 currentPosOfObjInHand = objectInHand.transform.position;
			Vector3 directionMoving = getDirectionControllerMoving();
			
			float xDirection = directionMoving.x;
			float yDirection = directionMoving.y;
			float zDirection = directionMoving.z;

			float xMovement = distanceToMoveControllerObject(getDistanceTraveledX(), handSpeedOverTimePassed(getDistanceTraveledX()));
			float yMovement = distanceToMoveControllerObject(getDistanceTraveledY(), handSpeedOverTimePassed(getDistanceTraveledY()));
			float zMovement = distanceToMoveControllerObject(getDistanceTraveledZ(), handSpeedOverTimePassed(getDistanceTraveledZ()));

			// Moving object
			objectInHand.transform.position = new Vector3(objectInHand.transform.position.x + xMovement*xDirection, 
				objectInHand.transform.position.y + yMovement*yDirection, objectInHand.transform.position.z + zMovement*zDirection);
		}
	}

	private float distanceToMoveControllerObject(float distanceHandMoved, float handSpeedOverTimePassed) {
		float k = 0;
		if(handSpeedOverTimePassed >= scaledConstant) {
			k = 1;
		} else if (minS < handSpeedOverTimePassed && handSpeedOverTimePassed < scaledConstant) {
			k = handSpeedOverTimePassed / scaledConstant;
		} else if (handSpeedOverTimePassed <= minS) {
			k = 0;
		}
		return k*distanceHandMoved;
	}

	private float millisecondsSinceLastUpdate() {
		return Time.deltaTime*1000f;
	}

	private float handSpeedOverTimePassed(float distanceTraveled) {
		return distanceTraveled / actualTimePassedOnLastPosition;
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
		updateLastPosition();
	}
}
