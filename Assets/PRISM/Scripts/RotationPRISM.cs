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
		lastHandRotation = this.transform.rotation;
	}

	// Only updates if millisecondDelayTime (500ms) has passed
	private void updateLastRotation() {
		if(timePassedTracker >= millisecondsDelayTime) {
			rotateObjectInHand();
			lastHandRotation = this.transform.rotation;
			actualTimePassedOnLastPosition = timePassedTracker;
			timePassedTracker = 0;
		}
		timePassedTracker = timePassedTracker += Time.deltaTime*1000f;		
	}

	private void rotateObjectInHand() {
		if(objectInHand != null && lastHandRotation != null) {

		}
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
	private void getQdiff() {

	}

	// Converts the angle represented by Qdiff from radians to degrees and Eq.
	private void getAngleRotatedInTimePassed() {

	}

	// Simply divides the angle by 200 ms (the time between Qt and Qt−1) to obtain the rotational
	// speed of the hand
	private void getRotationSpeed() {

	}

	// Is used to determine the control display ratio to
	// be used. The inverse of the control display ratio, k, is used to scale rotation
	private void getK() {

	}

	// The quaternion representation of the angle the hand has rotated (Qdiff)
	// is scaled by raising it to the power k, where k is a real number between 0 and 1.
	private void getNewOrientation() {

	}

	private float millisecondsSinceLastUpdate() {
		return Time.deltaTime*1000f;
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
	}
}
