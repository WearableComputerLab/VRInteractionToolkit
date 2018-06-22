using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationPRISM : MonoBehaviour {
	public SteamVR_TrackedObject trackedObj;

	private GameObject collidingObject;
	private GameObject objectInHand;

	private Quaternion lastHandRotation;

	private SteamVR_Controller.Device Controller
	{
		get { return SteamVR_Controller.Input((int)trackedObj.index); }
	}

	// keeping track of time passed resets every 500ms
	private float timePassedTracker;
	private float millisecondsDelayTime = 0;

	public float rotationMinS = 0.015f;
	public float rotationScalingConstant = 0.5f;
	public float rotationMaxS = 2f;

	
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

		/* 
		if(objectInHand) {
			objectInHand.transform.position = trackedObj.transform.position;
		}
		*/
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

	// Only updates if millisecondDelayTime (500ms) has passed
	private void updateLastRotation() {
		if(timePassedTracker >= millisecondsDelayTime) {
			rotateObjectInHand();
			lastHandRotation = this.transform.rotation;
			timePassedTracker = 0;
		}
		timePassedTracker = timePassedTracker += millisecondsSinceLastUpdate();		
	}

	private void rotateObjectInHand() {
		if(objectInHand != null && lastHandRotation != null) {
			Quaternion rotationToMoveTo = getNewOrientation();
			print("Old rotation: (" + objectInHand.transform.rotation.x + ", " + objectInHand.transform.rotation.y + ", " + objectInHand.transform.rotation.z + ")");
			print("New rotation: (" + rotationToMoveTo.x + ", " + rotationToMoveTo.y + ", " + rotationToMoveTo.z + ")");
			objectInHand.transform.eulerAngles = rotationToMoveTo.eulerAngles;
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

	// Simply divides the angle by 200 ms (we are using our actual time passed converted to seconds)  (the time between Qt and Qt−1) to obtain the rotational
	// speed of the hand
	private float getRotationSpeed() {
		float speed = getAngleRotatedInTimePassed() / (timePassedTracker);
		return getAngleRotatedInTimePassed() / (timePassedTracker);  // IS IN SECONDS CHECK IF NEED TO CHANGE FORMAT
	}

	// Is used to determine the control display ratio to
	// be used. The inverse of the control display ratio, k, is used to scale rotation
	private float getK() {
		if(getRotationSpeed() >= rotationScalingConstant) {
			print(1);
			return 1;
		} else if (rotationMinS < getRotationSpeed() && getRotationSpeed() < rotationScalingConstant){
			float scaledK = getRotationSpeed() / rotationScalingConstant;
			print("scaledk: " + scaledK);
			return getRotationSpeed() / rotationScalingConstant;
		} else if (getRotationSpeed() <= rotationMinS){
			print(0);
			return 0;
		}
		return 0; // CHECK IF THATS RIGHT
	}

	// The quaternion representation of the angle the hand has rotated (Qdiff)
	// is scaled by raising it to the power k, where k is a real number between 0 and 1.
	private Quaternion getNewOrientation() {
		// My interpetation of the description of algorithm but using unity methods
		float kValue = getK();
		if(kValue == 0) {
			// In the paper if the k value is 0 it relys on that causing the below equation to have an infinite (broken result)
			// making the change nothing. However because our equation is different to theirs due to our implementation
			// our result will give the same 1-1 mapping as if k value was 1. Therefore we must manuelly give a result
			// for if k value is 0
			return objectInHand.transform.rotation;
		}
		//return Quaternion.RotateTowards(objectInHand.transform.rotation, trackedObj.transform.rotation, kValue*Time.deltaTime);
		return powered(trackedObj.transform.rotation*Quaternion.Inverse(lastHandRotation), kValue) * objectInHand.transform.rotation;
	}

	private Quaternion powered(Quaternion theQuaternion, float power) {
		// TODO: Clean up this powered function

		Quaternion ln = theQuaternion;
		float r = (float) Mathf.Sqrt(ln.x*ln.x+ln.y*ln.y+ln.z*ln.z);
		float t  = r>0.00001f? (float)System.Math.Atan2(r,ln.w)/r: 0f;
		ln.w=0.5f*(float)Mathf.Log(ln.w*ln.w+ln.x*ln.x+ln.y*ln.y+ln.z*ln.z);
		ln.x*=t;
    	ln.y*=t;
    	ln.z*=t;

		Quaternion scale = ln;
		scale.w*=power;
    	scale.x*=power;
    	scale.y*=power;
    	scale.z*=power;

		Quaternion exp = scale;
		r  = (float) Mathf.Sqrt(exp.x*exp.x+exp.y*exp.y+exp.z*exp.z);
    	float et = (float) Mathf.Exp(exp.w);
    	float s  = r>=0.00001f? et*(float)Mathf.Sin(r)/r: 0f;
		exp.w=et*(float)Mathf.Cos(r);
    	exp.x*=s;
    	exp.y*=s;
    	exp.z*=s;

		return exp;
	}
}