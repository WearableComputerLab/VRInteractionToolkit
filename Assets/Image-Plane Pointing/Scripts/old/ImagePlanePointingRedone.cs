
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class ImagePlanePointingRedone : MonoBehaviour {
    //http://www.cs.cmu.edu/~stage3/publications/97/conferences/3DSymposium/HeadCrusher/index.html
    /*
    private SteamVR_TrackedObject trackedObj;
    private SteamVR_Controller.Device controller;

    public GameObject laserPrefab;
    private GameObject laser;
    private Transform laserTransform;
    private Vector3 hitPoint;
    public GameObject mirroredCube;


    private void ShowLaser(RaycastHit hit) {
        mirroredCube.SetActive(false);
        laser.SetActive(true);
        laserTransform.position = Vector3.Lerp(trackedObj.transform.position, hitPoint, .5f);
        laserTransform.LookAt(hitPoint);
        laserTransform.localEulerAngles = new Vector3(laserTransform.localEulerAngles.x + 45f , laserTransform.localEulerAngles.y + 45f , laserTransform.localEulerAngles.z);
        laserTransform.localScale = new Vector3(laserTransform.localScale.x, laserTransform.localScale.y, hit.distance);
    }

    private bool pickedUpObject = false; //ensure only 1 object is picked up at a time
    private GameObject tempObjectStored;
    void PickupObject(GameObject obj) {
        Vector3 controllerPos = trackedObj.transform.forward;
        if (trackedObj != null) {
            if (controller.GetTouchDown(SteamVR_Controller.ButtonMask.Trigger) && pickedUpObject == false) {
                //obj.GetComponent<Collider>().attachedRigidbody.isKinematic = true;
                obj.transform.SetParent(trackedObj.transform);
                float dist = Vector3.Distance(trackedObj.transform.position, obj.transform.position);
                //obj.transform.localPosition = new Vector3(0f, 0f, 0f);
                //obj.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
                //obj.transform.localScale = new Vector3(trackedObj.transform.localScale.x/ dist, trackedObj.transform.localScale.y / dist, trackedObj.transform.localScale.z / dist);
                GameObject pickedObj2D = Instantiate(obj, new Vector3(0f, 0f, 0f), Quaternion.identity) as GameObject;
                pickedObj2D.transform.localScale = new Vector3(trackedObj.transform.localScale.x / dist, trackedObj.transform.localScale.y / dist, 0.1f);
                extendDistance = Vector3.Distance(controllerPos, obj.transform.position);
                print("scale dist:"+dist);
                tempObjectStored = obj; // Storing the object as an instance variable instead of using the obj parameter fixes glitch of it not properly resetting on TriggerUp
                pickedUpObject = true;
            }
            else if (controller.GetTouchDown(SteamVR_Controller.ButtonMask.Trigger) && pickedUpObject == true) {
                tempObjectStored.transform.SetParent(null);
                pickedUpObject = false;
            }
        }
    }

    private void ShowLaser() {
        laser.SetActive(true);
        mirroredCube.SetActive(true);
    }

    void mirroredObject() {
        Vector3 controllerPos = trackedObj.transform.forward;
        float distance_formula_on_vector = Mathf.Sqrt(controllerPos.x * controllerPos.x + controllerPos.y * controllerPos.y + controllerPos.z * controllerPos.z);
        Vector3 mirroredPos = trackedObj.transform.position;

        mirroredPos.x = mirroredPos.x + (100f / (distance_formula_on_vector)) * controllerPos.x;
        mirroredPos.y = mirroredPos.y + (100f / (distance_formula_on_vector)) * controllerPos.y;
        mirroredPos.z = mirroredPos.z + (100f / (distance_formula_on_vector)) * controllerPos.z;

        mirroredCube.transform.position = mirroredPos;
        mirroredCube.transform.rotation = trackedObj.transform.rotation;
    }

    void reelObject(GameObject obj) {
        Vector3 controllerPos = trackedObj.transform.forward;
        Vector3 pos = trackedObj.transform.position;
        float distance_formula_on_vector = Mathf.Sqrt(controllerPos.x * controllerPos.x + controllerPos.y * controllerPos.y + controllerPos.z * controllerPos.z);
        // Using formula to find a point which lies at distance on a 3D line from vector and direction
        pos.x += (extendDistance / (distance_formula_on_vector)) * controllerPos.x;
        pos.y += (extendDistance / (distance_formula_on_vector)) * controllerPos.y;
        pos.z += (extendDistance / (distance_formula_on_vector)) * controllerPos.z;

        obj.transform.position = pos;
        obj.transform.rotation = trackedObj.transform.rotation;
    }

    private float extendDistance = 0f;
    private float cursorSpeed = 20f; // Decrease to make faster, Increase to make slower

    private void PadScrolling(GameObject obj) {
        if (obj.transform.name == "Mirrored Cube") {
            return;
        }
        Vector3 controllerPos = trackedObj.transform.forward;
        if (controller.GetAxis().y != 0) {
            extendDistance += controller.GetAxis().y / cursorSpeed;
            reelObject(obj);
        }
    }

    public bool controllerRightPicked;
    public bool controllerLeftPicked;

    void Awake() {
        GameObject controllerRight = GameObject.Find("Controller (right)");
        GameObject controllerLeft = GameObject.Find("Controller (left)");
        print(controllerRight);
        if (controllerRightPicked == true) {
            trackedObj = controllerRight.GetComponent<SteamVR_TrackedObject>();
        } else if (controllerLeftPicked == true) {
            trackedObj = controllerLeft.GetComponent<SteamVR_TrackedObject>();
        } else { //TODO: Automatically attempt to detect controller
            print("Couldn't detect trackedObject, please specify the controller type in the settings.");
            Application.Quit();
        }
    }

    void Start() {
        laser = Instantiate(laserPrefab);
        laserTransform = laser.transform;
    }

    void Update() {
       // print(trackedObj.transform.position);
        controller = SteamVR_Controller.Input((int)trackedObj.index);
        mirroredObject();
        ShowLaser();
        Ray ray = Camera.main.ScreenPointToRay(trackedObj.transform.position);
        RaycastHit hit;
        if (Physics.Raycast(trackedObj.transform.position, trackedObj.transform.forward, out hit, 100)) {
            hitPoint = hit.point;
            PickupObject(hit.transform.gameObject);
            if (pickedUpObject == true) {
                PadScrolling(hit.transform.gameObject);
            }
            ShowLaser(hit);
        }
    }
    */
}

