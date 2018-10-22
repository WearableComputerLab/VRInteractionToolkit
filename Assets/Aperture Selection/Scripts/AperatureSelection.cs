using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AperatureSelection : MonoBehaviour {

	// Uses a laser as the handle
	public GameObject laserPrefab;
	private GameObject laser;
	private Transform laserTransform;
	private Vector3 hitPoint;

	public GameObject laserContainer;

	public GameObject aperatureVolume;

	public SteamVR_TrackedObject controllerTrackedObj;

	public SteamVR_TrackedObject headsetTrackedObj;
	
	private float minimumDistanceOfIntersection = 2f;

	public float amplificationOfLength = 4f; // multiple the distance so that the cone can reach further


	void OnEnable() {
		     
	}

	void translateConeDistanceAlongForward(float theDistance) {
        aperatureVolume.transform.position = headsetTrackedObj.transform.position+headsetTrackedObj.transform.forward*theDistance;
    }

	// Use this for initialization
	void Start () {
		// Turning on flashlight for apeature
		aperatureVolume.SetActive(true);
		
		// Setting flashlight as a child of head
		aperatureVolume.transform.parent = headsetTrackedObj.transform.parent.transform;
		aperatureVolume.transform.localEulerAngles = new Vector3(0, 180, 0);


		laser = Instantiate(laserPrefab);
        laserTransform = laser.transform;
		laser.transform.parent = laserContainer.transform;

		// Translates the cone so that whatever size it is as long as it is at position 0,0,0 if contoller it will jump to the origin point for flashlight
		if(aperatureVolume.GetComponent<Renderer>().bounds.size.z != 0) {
			translateConeDistanceAlongForward(aperatureVolume.GetComponent<Renderer>().bounds.size.z/2f);
		}  
	}
	
	// Update is called once per frame
	void Update () {
		ShowLaser();
		getIntersectionLocation();

	}

	Vector3 getIntersectionLocation() {
		// gets the intersection between the controller laser and the forward of the headset
		// assuming 1 is pointing controller for test
        Vector3 d1 = controllerTrackedObj.transform.forward;
        Vector3 d2 = headsetTrackedObj.transform.forward;

        Vector3 p1 = controllerTrackedObj.transform.position;
        Vector3 p2 = headsetTrackedObj.transform.position;

        // as these two vectors will probably create skew lines (on different planes) have to calculate the points on the lines that are
        // closest to eachother and then getting the midpoint between them giving a fake 'intersection'
        // This is achieved by utilizing parts of the fromula to find the shortest distance between two skew lines
        Vector3 n1 = Vector3.Cross(d1, (Vector3.Cross(d2, d1)));
        Vector3 n2 = Vector3.Cross(d2, (Vector3.Cross(d1, d2)));

        // Figuring out point 1
        Vector3 localPoint1 = p1 + ((Vector3.Dot((p2 - p1), n2)) / (Vector3.Dot(d1, n2))) * d1;

        // Figuring out point 2
        Vector3 localPoint2 = p2 + ((Vector3.Dot((p1 - p2), n1)) / (Vector3.Dot(d2, n1))) * d2;


        float distanceBetweenPoints = Vector3.Distance(localPoint1, localPoint2);

		float lengthOfCone = Vector3.Distance(headsetTrackedObj.transform.position, localPoint2);
		lengthOfCone = lengthOfCone*amplificationOfLength;

		// Reszing the volume to match the location
		aperatureVolume.transform.localScale = new Vector3(aperatureVolume.transform.localScale.x, aperatureVolume.transform.localScale.y, lengthOfCone*100);
		translateConeDistanceAlongForward(lengthOfCone+0.1f);

		return localPoint2;
	}

	private void ShowLaser()
    {
        // This is to make it extend infinite. There is DEFINATELY an easier way to do this. Find it later!
        Vector3 theVector = controllerTrackedObj.transform.forward;
        hitPoint = controllerTrackedObj.transform.position;
        float distance_formula_on_vector = Mathf.Sqrt(theVector.x * theVector.x + theVector.y * theVector.y + theVector.z * theVector.z);
        // Using formula to find a point which lies at distance on a 3D line from vector and direction
        hitPoint.x = hitPoint.x + (100 / (distance_formula_on_vector)) * theVector.x;
        hitPoint.y = hitPoint.y + (100 / (distance_formula_on_vector)) * theVector.y;
        hitPoint.z = hitPoint.z + (100 / (distance_formula_on_vector)) * theVector.z;

        laser.SetActive(true);
        laserTransform.position = Vector3.Lerp(controllerTrackedObj.transform.position, hitPoint, .5f);
        laserTransform.LookAt(hitPoint);
        laserTransform.localScale = new Vector3(laserTransform.localScale.x, laserTransform.localScale.y,
           100);
    }
}
