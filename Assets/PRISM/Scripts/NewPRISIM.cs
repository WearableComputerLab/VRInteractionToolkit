using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewPRISIM : MonoBehaviour {

	public SteamVR_TrackedObject trackedObj;
	//public GameObject theController;

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
	private float millisecondsDelayTime = 0;

	public float minS = 0.001f;
	public float scaledConstant = 0.5f;
	public float maxS = 2f;

	// OFFSET RECOVERY VARIABLES
	private float offset = 0;
	private float totalTimePassedWhenMaxThresholdExceeded = 0;



	// Offset recovery as specified by paper Time.time is in seconds
	private void offsetRecovery() {
		if(offset == 0) {
			// no offset so no need to recover
			return;
		}

		if(totalTimePassedWhenMaxThresholdExceeded == 0) {
			totalTimePassedWhenMaxThresholdExceeded = Time.time;
			return; // Just started recovery on next call will recover
		}

		float currentTime = Time.time;

		float distanceToMoveObjectTowardsController = 0;
		
		if(totalTimePassedWhenMaxThresholdExceeded < currentTime && currentTime < (totalTimePassedWhenMaxThresholdExceeded + 0.05f)) {
			// will recover offset by making offset 80% of itself
			distanceToMoveObjectTowardsController = offset * 0.2f;
		} else if ((totalTimePassedWhenMaxThresholdExceeded + 0.5) < currentTime && currentTime < (totalTimePassedWhenMaxThresholdExceeded + 1f)) {
			// will recover offset by making offset 50% of itself
			distanceToMoveObjectTowardsController = offset * 0.5f;

		} else if (currentTime > (totalTimePassedWhenMaxThresholdExceeded + 1f)) {
			// will completely recover offset making it 0
			distanceToMoveObjectTowardsController = offset;
			totalTimePassedWhenMaxThresholdExceeded = 0;
		}

		Vector3 direction = trackedObj.transform.position - objectInHand.transform.position;
		objectInHand.transform.position = objectInHand.transform.position + (direction.normalized * distanceToMoveObjectTowardsController);		
	}


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
		//objectInHand.transform.position = this.transform.position;
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

			xDirection = xDirection/Mathf.Abs(xDirection);
			yDirection = yDirection/Mathf.Abs(yDirection);
			zDirection = zDirection/Mathf.Abs(zDirection);

			float xMovement = distanceToMoveControllerObject(getDistanceTraveledX(), handSpeedOverTimePassed(getDistanceTraveledX()));
			float yMovement = distanceToMoveControllerObject(getDistanceTraveledY(), handSpeedOverTimePassed(getDistanceTraveledY()));
			float zMovement = distanceToMoveControllerObject(getDistanceTraveledZ(), handSpeedOverTimePassed(getDistanceTraveledZ()));
			//print(handSpeedOverTimePassed(getDistanceTraveledX()));
			// Moving object
			objectInHand.transform.position = new Vector3(objectInHand.transform.position.x + xMovement*xDirection, 
				objectInHand.transform.position.y + yMovement*yDirection, objectInHand.transform.position.z + zMovement*zDirection);
			
			// calculating offset
			offset = Vector3.Distance(objectInHand.transform.position, trackedObj.transform.position);
			
			float speed = handSpeedOverTimePassed(getDistanceTraveledSinceLastPosition());

			print(speed);
			print("Max S: " + maxS + ", Speed: " + speed);
			// recover offset if it exists
			if(maxS < speed) {
				print("here");
				offsetRecovery();
			}
		}
	}

	private float distanceToMoveControllerObject(float distanceHandMoved, float handSpeedOverTimePassed) {
		//print(distanceHandMoved);
		float k = 0;
		if(handSpeedOverTimePassed >= scaledConstant) {
			k = 1;
			//print(1);
		} else if (minS < handSpeedOverTimePassed && handSpeedOverTimePassed < scaledConstant) {
			k = handSpeedOverTimePassed / scaledConstant;
			//print("Here and k is: " + k);
		} else if (handSpeedOverTimePassed <= minS) {
			k = 0;
			//print("zero");
		}

		return k*distanceHandMoved;
	}

	private float millisecondsSinceLastUpdate() {
		return Time.deltaTime*1000f;
	}

	private float handSpeedOverTimePassed(float distanceTraveled) {
		return distanceTraveled / (timePassedTracker/1000);
	}

	private Vector3 getDirectionControllerMoving() {
		return (this.transform.position - lastPosition).normalized;
	}

	private float getDistanceTraveledSinceLastPosition() {
		return Vector3.Distance(this.transform.position, lastPosition);
	}

	private float getDistanceTraveledX() {
		return Mathf.Abs(this.transform.position.x - lastPosition.x);
	}

	private float getDistanceTraveledY() {
		return Mathf.Abs(this.transform.position.y - lastPosition.y);
	}

	private float getDistanceTraveledZ() {
		return Mathf.Abs(this.transform.position.z - lastPosition.z);
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
