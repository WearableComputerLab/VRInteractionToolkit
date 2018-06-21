using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationPRISM : MonoBehaviour {
	private SteamVR_TrackedObject trackedObj;
	public GameObject theController;

	private GameObject collidingObject;
	private GameObject objectInHand;

	private Quaternion lastHandRotation;

	private SteamVR_Controller.Device Controller
	{
		get { return SteamVR_Controller.Input((int)trackedObj.index); }
	}

	// keeping track of time passed resets every 500ms
	private float timePassedTracker;
	private float millisecondsDelayTime = 200;
	private float actualTimePassedOnLastPosition;

	private float rotationMinS;
	private float rotationScalingConstant;
	private float rotationMaxS;

	
	// Use this for initialization
	void Start () {
		lastHandRotation = this.transform.rotation;
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
		updateLastRotation();
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
		objectInHand.transform.position = this.transform.position;
	}

	private void ReleaseObject()
    {  
        objectInHand = null;
    }

	// Only updates if millisecondDelayTime (500ms) has passed
	private void updateLastRotation() {
		if(timePassedTracker >= millisecondsDelayTime) {
			rotateObjectInHand();
			lastHandRotation = this.transform.rotation;
			actualTimePassedOnLastPosition = timePassedTracker;
			timePassedTracker = 0;
		}
		timePassedTracker = timePassedTracker += millisecondsSinceLastUpdate();		
	}

	private void rotateObjectInHand() {
		if(objectInHand != null && lastHandRotation != null) {
			objectInHand.transform.rotation = getNewOrientation();
		}
	}
	
	private float millisecondsSinceLastUpdate() {
		return Time.deltaTime*1000f;
	}

	/** 
		INFORMATION ABOUT EQUATIONS/METHODS BELOW:

		Qdiff is the quaternion representing the angle the hand has rotated in
			the last 200 ms.*

		Qt is the quaternion representing the current hand orientation

		Qt−1 is the quaternion representing the hand orientation 200 ms before
			the current time

		Qnew is the quaternion representing the new orientation of the object

		Qobject is the quaternion representing the current orientation of the
			controlled object

		A is the angle (in degrees) the hand has rotated in the last 200 ms.*
		RS is rotational speed of the hand in degrees/s	
	 */

	// Calculates the rotational difference between the current(Qt) and
	// last orientation (Qt−1) of the hand, in the form of Qdiff. Note, to find the quaternion
	// needed to rotate from q1 to q2, q2 is divided by q1
	private Quaternion getQdiff() {
		return trackedObj.transform.rotation*Quaternion.Inverse(lastHandRotation);
	}

	// Converts the angle represented by Qdiff from radians to degrees and Eq.
	// (We just used unity's built in angle calculator)
	private float getAngleRotatedInTimePassed() {
		return Quaternion.Angle(trackedObj.transform.rotation, lastHandRotation);
	}

	// Simply divides the angle by 200 ms (the time between Qt and Qt−1) to obtain the rotational
	// speed of the hand
	private float getRotationSpeed() {
		return getAngleRotatedInTimePassed() / 0.20f;  // IS IN SECONDS CHECK IF NEED TO CHANGE FORMAT
	}

	// Is used to determine the control display ratio to
	// be used. The inverse of the control display ratio, k, is used to scale rotation
	private float getK() {
		if(getRotationSpeed() >= rotationScalingConstant) {
			return 1;
		} else if (rotationMinS < getRotationSpeed() && getRotationSpeed() < rotationScalingConstant){
			return getRotationSpeed() / rotationScalingConstant;
		} else if (getRotationSpeed() <= rotationMinS){
			return 0;
		}
		return 0; // CHECK IF THATS RIGHT
	}

	// The quaternion representation of the angle the hand has rotated (Qdiff)
	// is scaled by raising it to the power k, where k is a real number between 0 and 1.
	private Quaternion getNewOrientation() {
		// My interpetation of the description of algorithm but using unity methods
		return Quaternion.RotateTowards(objectInHand.transform.rotation, getQdiff(), getK());
		// TODO check if works
	}
}