using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Valve.VR;

public class PRISMMovement : MonoBehaviour {

#if SteamVR_Legacy
    	public SteamVR_TrackedObject trackedObj;
    	private SteamVR_Controller.Device Controller
	{
		get { return SteamVR_Controller.Input((int)trackedObj.index); }
	}
#elif SteamVR_2
    public SteamVR_Behaviour_Pose trackedObj;
    public SteamVR_Action_Boolean m_controllerPress;
#else
    public GameObject trackedObj;
#endif

    public LayerMask interactionLayers;

	//public GameObject theController;

	public GameObject collidingObject;
	public GameObject objectInHand = null;

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

	
    public UnityEvent selectedObject; // Invoked when an object is selected

    public UnityEvent hovered; // Invoked when an object is hovered by technique
    public UnityEvent unHovered; // Invoked when an object is no longer hovered by the technique
    

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


	private void SetCollidingObject(Collider other)
    {
		
		if (collidingObject || !other.GetComponent<Rigidbody>() || interactionLayers != (interactionLayers | (1 << other.gameObject.layer)))
        {
            return;
        }
        collidingObject = other.gameObject;		
		hovered.Invoke();
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
		unHovered.Invoke();
        collidingObject = null;
    }

	// Need to change this one
	private void pickUpObject()
    {
        
        objectInHand = collidingObject;
		Rigidbody bod;
		if((bod = objectInHand.GetComponent<Rigidbody>()) != null) {
			bod.isKinematic = true;
		}
		selectedObject.Invoke();
        collidingObject = null;
	}

	private void ReleaseObject()
    {  
		Rigidbody bod;
		if((bod = objectInHand.GetComponent<Rigidbody>()) != null) {
			bod.isKinematic = false;
		}
        objectInHand = null;
    }
	
	// Use this for initialization
	void Start () {
	}
	
	// Only updates if millisecondDelayTime (500ms) has passed
	private void updateLastPosition() {
		if(timePassedTracker >= millisecondsDelayTime) {
			moveObjectInHand();
			lastPosition = trackedObj.transform.position;

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
		return (trackedObj.transform.position - lastPosition).normalized;
	}

	private float getDistanceTraveledSinceLastPosition() {
		return Vector3.Distance(trackedObj.transform.position, lastPosition);
	}

	private float getDistanceTraveledX() {
		return Mathf.Abs(trackedObj.transform.position.x - lastPosition.x);
	}

	private float getDistanceTraveledY() {
		return Mathf.Abs(trackedObj.transform.position.y - lastPosition.y);
	}

	private float getDistanceTraveledZ() {
		return Mathf.Abs(trackedObj.transform.position.z - lastPosition.z);
	}

    public enum ControllerState {
        TRIGGER_UP, TRIGGER_DOWN, NONE
    }

    private ControllerState controllerEvents() {
#if SteamVR_Legacy
        if (Controller.GetHairTriggerDown()) {
            return ControllerState.TRIGGER_DOWN;
        } if (Controller.GetHairTriggerUp()) {
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
    void Update () {
		if (controllerEvents() == ControllerState.TRIGGER_DOWN)
        {	
            if (collidingObject)
            {
                pickUpObject();
            }
        }


        if (controllerEvents() == ControllerState.TRIGGER_UP)
        {
            if (objectInHand)
            {
                ReleaseObject();
            }
        }
		updateLastPosition();
	}
}
