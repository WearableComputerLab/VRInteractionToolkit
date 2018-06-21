using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AperatureSelection : MonoBehaviour {

	// Uses a laser as the handle
	public GameObject laserPrefab;
	private GameObject laser;
	private Transform laserTransform;
	private Vector3 hitPoint;

	public SteamVR_TrackedObject controllerTrackedObj;

	private SteamVR_TrackedObject headsetTrackedObj;
	
	private float minimumDistanceOfIntersection = 2f;

	public GameObject testObjectToSeeIfIntersectionWorking;


	void OnEnable() {
		SteamVR_TrackedObject[] objects = this.GetComponentsInChildren<SteamVR_TrackedObject>();
		// Finding headset
		foreach(SteamVR_TrackedObject obj in objects) {
			if(obj.index == SteamVR_TrackedObject.EIndex.Hmd) {
				// This is the headset
				headsetTrackedObj = obj;
			}
		}
	}
	// Use this for initialization
	void Start () {
		laser = Instantiate(laserPrefab);
        laserTransform = laser.transform;
	}
	
	// Update is called once per frame
	void Update () {
		ShowLaser();
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


		// Has to be within 2
        if (distanceBetweenPoints < minimumDistanceOfIntersection)
        {
            testObjectToSeeIfIntersectionWorking.transform.position = localPoint2;
        }
		
		return localPoint2;
	}

	private void ShowLaser()
    {
        // This is to make it extend infinite. There is DEFINATELY an easier way to do this. Find it later!
        Vector3 theVector = this.transform.forward;
        hitPoint = this.transform.position;
        float distance_formula_on_vector = Mathf.Sqrt(theVector.x * theVector.x + theVector.y * theVector.y + theVector.z * theVector.z);
        // Using formula to find a point which lies at distance on a 3D line from vector and direction
        hitPoint.x = hitPoint.x + (100 / (distance_formula_on_vector)) * theVector.x;
        hitPoint.y = hitPoint.y + (100 / (distance_formula_on_vector)) * theVector.y;
        hitPoint.z = hitPoint.z + (100 / (distance_formula_on_vector)) * theVector.z;

        laser.SetActive(true);
        laserTransform.position = Vector3.Lerp(this.transform.position, hitPoint, .5f);
        laserTransform.LookAt(hitPoint);
        laserTransform.localScale = new Vector3(laserTransform.localScale.x, laserTransform.localScale.y,
           100);
    }
}
