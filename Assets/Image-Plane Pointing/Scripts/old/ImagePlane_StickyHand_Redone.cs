using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class ImagePlane_StickyHand_Redone : MonoBehaviour {
    //http://www.cs.cmu.edu/~stage3/publications/97/conferences/3DSymposium/HeadCrusher/index.html

    /*
private SteamVR_TrackedObject trackedObj;
private SteamVR_Controller.Device controller;

public GameObject laserPrefab;
private GameObject laser;
private Transform laserTransform;
private Vector3 hitPoint;
private GameObject mirroredCube;
public Material outlineMaterial;

public enum InteractionType { Selection, Manipulation_Movement, Manipulation_Full };
public InteractionType interacionType;

public enum ControllerPicked { Left_Controller, Right_Controller };
public ControllerPicked controllerPicked;

private void ShowLaser(RaycastHit hit) {
mirroredCube.SetActive(false);
laser.SetActive(true);
laserTransform.position = Vector3.Lerp(trackedObj.transform.position, hitPoint, .5f);
laserTransform.LookAt(hitPoint);
laserTransform.localScale = new Vector3(laserTransform.localScale.x, laserTransform.localScale.y, hit.distance);
PickupObject(hit.transform.gameObject);
}

private bool pickedUpObject = false; //ensure only 1 object is picked up at a time
private GameObject tempObjectStored;
void PickupObject(GameObject obj) {
Vector3 controllerPos = trackedObj.transform.forward;
if (trackedObj != null) {
    if (controller.GetTouchDown(SteamVR_Controller.ButtonMask.Trigger) && pickedUpObject == false) {
        //obj.transform.SetParent(trackedObj.transform);
        float dist = Vector3.Distance(trackedObj.transform.position, obj.transform.position);
        //GameObject clonedObject = Instantiate(obj);
        obj.transform.position = Vector3.Lerp(trackedObj.transform.position, obj.transform.position, 0.05f);
        //clonedObject.transform.localScale = obj.transform.localScale / dist;
        obj.transform.localScale = new Vector3(obj.transform.localScale.x / dist, obj.transform.localScale.y / dist, obj.transform.localScale.z / dist);
        obj.transform.SetParent(trackedObj.transform);
        extendDistance = Vector3.Distance(controllerPos, obj.transform.position);
        //obj.transform.GetComponent<Renderer>().material = outlineMaterial;
        print("scale dist:" + dist);
        tempObjectStored = obj; // Storing the object as an instance variable instead of using the obj parameter fixes glitch of it not properly resetting on TriggerUp
        pickedUpObject = true;
    } else if (controller.GetTouchDown(SteamVR_Controller.ButtonMask.Trigger) && pickedUpObject == true) {
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
            print(extendDistance);
            reelObject(obj);
        }
    }

    void castRay() {
        mirroredObject();
        ShowLaser();
        Ray ray = Camera.main.ScreenPointToRay(trackedObj.transform.position);
        RaycastHit hit;
        if (Physics.Raycast(trackedObj.transform.position, cameraHead.transform.forward, out hit, 100)) {
            hitPoint = hit.point;
            ShowLaser(hit);
        }
    }

    private GameObject cameraHead;
    private GameObject cameraRig;

    void Awake() {
        GameObject controllerRight = GameObject.Find("Controller (right)");
        GameObject controllerLeft = GameObject.Find("Controller (left)");
        cameraHead = GameObject.Find("Camera (eye)");
        cameraRig = GameObject.Find("[CameraRig]");
        mirroredCube = this.transform.Find("Mirrored Cube").gameObject;
        if (controllerPicked == ControllerPicked.Right_Controller) {
            trackedObj = controllerRight.GetComponent<SteamVR_TrackedObject>();
        } else if (controllerPicked == ControllerPicked.Left_Controller) {
            trackedObj = controllerLeft.GetComponent<SteamVR_TrackedObject>();
        } else {
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
        castRay();
    }
    */
}