using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HOMER : MonoBehaviour {

    /* HOMER implementation by Kieran May
     * University of South Australia
     * 
     * The HOMER algorithm I wrote is based off: (pg 34-35) https://people.cs.vt.edu/~bowman/3dui.org/course_notes/siggraph2001/basic_techniques.pdf 
     * 
     * */

    public GameObject controllerRight;
    public GameObject controllerLeft;
    SteamVR_TrackedObject trackedObj;
    SteamVR_Controller.Device controller;

    private GameObject mirroredCube;
    public GameObject laserPrefab;
    private GameObject laser;
    private Transform laserTransform;
    private Vector3 hitPoint;

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
        InstantiateObject(hit.transform.gameObject);
    }

    float Disth = 0f;
    float Disto = 0f;
    bool objSelected = false;
    private GameObject cameraHead; // t
    private GameObject virtualHand;
    private GameObject selectedObject;
    public GameObject handPrefab;
    private Transform oldParent;

    private void InstantiateObject(GameObject obj) {
        if (controller.GetPressDown(SteamVR_Controller.ButtonMask.Trigger)) {
            virtualHand = Instantiate(new GameObject("hand"));
            virtualHand.transform.position = obj.transform.position;
            virtualHand.SetActive(true);
            selectedObject = obj;
            oldParent = selectedObject.transform.parent;
            objSelected = true;
            selectedObject.transform.SetParent(virtualHand.transform);
            laser.SetActive(false);

            Disth = Vector3.Distance(trackedObj.transform.position, cameraHead.transform.position);
            Disto = Vector3.Distance(obj.transform.position, cameraHead.transform.position);
        }
    }

    private void HomerFormula() {
        //print("updating frame..");
        virtualHand.transform.localEulerAngles = trackedObj.transform.localEulerAngles;
        //each frame
        float Disthcurr = Vector3.Distance(trackedObj.transform.position, cameraHead.transform.position); // Physical hand distance
        float Distvh = Disthcurr * (Disto / Disth); // Virtual hand distance
        Vector3 thcurr = (trackedObj.transform.position - cameraHead.transform.position);
        Vector3 VirtualHandPos = cameraHead.transform.position + Distvh * (thcurr);
        virtualHand.transform.position = VirtualHandPos;
        virtualHand.transform.position = new Vector3(virtualHand.transform.position.x, virtualHand.transform.position.y, virtualHand.transform.position.z);

        if (controller.GetPressDown(SteamVR_Controller.ButtonMask.Trigger)) {
            objSelected = false;
            Destroy(virtualHand);
            selectedObject.transform.SetParent(oldParent);
        }
    }

    private void castRay() {
        Ray ray = Camera.main.ScreenPointToRay(trackedObj.transform.position);
        ShowLaser();
        RaycastHit hit;
        if (Physics.Raycast(trackedObj.transform.position, trackedObj.transform.forward, out hit, 100)) {
            hitPoint = hit.point;
            ShowLaser(hit);
        }
    }

    void moveMirroredCube() {
        //getControllerPosition();
        Vector3 mirroredPos = trackedObj.transform.position;
        Vector3 controllerPos = trackedObj.transform.forward;
        float distance_formula_on_vector = Mathf.Sqrt(controllerPos.x * controllerPos.x + controllerPos.y * controllerPos.y + controllerPos.z * controllerPos.z);

        mirroredPos.x = mirroredPos.x + (100f / (distance_formula_on_vector)) * controllerPos.x;
        mirroredPos.y = mirroredPos.y + (100f / (distance_formula_on_vector)) * controllerPos.y;
        mirroredPos.z = mirroredPos.z + (100f / (distance_formula_on_vector)) * controllerPos.z;
        mirroredCube.transform.position = mirroredPos;
        mirroredCube.transform.rotation = trackedObj.transform.rotation;
    }

    private void ShowLaser() {
        laser.SetActive(true);
        mirroredCube.SetActive(true);
    }

    void Awake() {
        cameraHead = GameObject.Find("Camera (eye)");
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

    // Use this for initialization
    void Start() {
        laser = Instantiate(laserPrefab);
        laserTransform = laser.transform;
    }

    // Update is called once per frame
    void Update() {
        controller = SteamVR_Controller.Input((int)trackedObj.index);
        if (objSelected == false) {
            moveMirroredCube();
            castRay();
        } else if (objSelected == true) {
            HomerFormula();
        }
    }
}