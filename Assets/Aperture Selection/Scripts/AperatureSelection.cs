using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class AperatureSelection : MonoBehaviour {

#if SteamVR_Legacy
    public SteamVR_TrackedObject controllerTrackedObj;
    public SteamVR_TrackedObject headsetTrackedObj;
#elif SteamVR_2
    public SteamVR_Behaviour_Pose controllerTrackedObj;
	public GameObject headsetTrackedObj;
#else
    public GameObject controllerTrackedObj;
    public GameObject headsetTrackedObj;
#endif

    // Uses a laser as the handle
    public GameObject laserPrefab;
    private GameObject laser;
    private Transform laserTransform;
    private Vector3 hitPoint;

    public GameObject laserContainer;

    public GameObject aperatureVolume;

    private float minimumDistanceOfIntersection = 2f;

    public float amplificationOfLength = 5f; // multiple the distance so that the cone can reach further

    public GameObject orientationPlates;

    void setPlatesRotation() {
        orientationPlates.transform.localEulerAngles = new Vector3(orientationPlates.transform.localEulerAngles.x,
                                    orientationPlates.transform.localEulerAngles.y, controllerTrackedObj.transform.localEulerAngles.z * -1);
    }

    void translateConeDistanceAlongForward(float theDistance) {
        aperatureVolume.transform.position = headsetTrackedObj.transform.position + headsetTrackedObj.transform.forward * theDistance;
    }

    // Use this for initialization
    void Start() {
        // Turning on flashlight for apeature
        aperatureVolume.SetActive(true);

        // Setting flashlight as a child of head
        aperatureVolume.transform.parent = headsetTrackedObj.transform.parent.transform;
        aperatureVolume.transform.localEulerAngles = new Vector3(0, 180, 0);


        laser = Instantiate(laserPrefab);
        laserTransform = laser.transform;
        laser.transform.parent = laserContainer.transform;

        // Translates the cone so that whatever size it is as long as it is at position 0,0,0 if contoller it will jump to the origin point for flashlight
        if (aperatureVolume.GetComponent<Renderer>().bounds.size.z != 0) {
            translateConeDistanceAlongForward(aperatureVolume.GetComponent<Renderer>().bounds.size.z / 2f);
        }
    }

    // Update is called once per frame
    void Update() {
        ShowLaser();
        setSizeAperature();
        setPlatesRotation();
    }

    void setSizeAperature() {

        // Getting distance between controller and headset
        float distance = Vector3.Distance(controllerTrackedObj.transform.position, headsetTrackedObj.transform.position) * amplificationOfLength;

        aperatureVolume.transform.localScale = new Vector3(aperatureVolume.transform.localScale.x, aperatureVolume.transform.localScale.y, distance * 100);
        translateConeDistanceAlongForward(distance + 0.01f);

    }

    private void ShowLaser() {
        // Laser shows which controller is controlling
        Vector3 theVector = controllerTrackedObj.transform.forward;
        hitPoint = controllerTrackedObj.transform.position;
        float distance_formula_on_vector = Mathf.Sqrt(theVector.x * theVector.x + theVector.y * theVector.y + theVector.z * theVector.z);
        // Using formula to find a point which lies at distance on a 3D line from vector and direction
        hitPoint.x = hitPoint.x + (0.05f / (distance_formula_on_vector)) * theVector.x;
        hitPoint.y = hitPoint.y + (0.05f / (distance_formula_on_vector)) * theVector.y;
        hitPoint.z = hitPoint.z + (0.05f / (distance_formula_on_vector)) * theVector.z;

        laser.SetActive(true);
        laserTransform.position = Vector3.Lerp(controllerTrackedObj.transform.position, hitPoint, .5f);
        laserTransform.LookAt(hitPoint);
        laserTransform.localScale = new Vector3(laserTransform.localScale.x, laserTransform.localScale.y,
           0.05f);
    }
}
