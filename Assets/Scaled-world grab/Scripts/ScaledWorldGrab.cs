using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaledWorldGrab : MonoBehaviour {

    /* Scaled-world grab implementation by Kieran May
     * University of South Australia
     * 
     * The Scaled-world grab algorithm I wrote is based off: (pg 37) https://people.cs.vt.edu/~bowman/3dui.org/course_notes/siggraph2001/basic_techniques.pdf 
     *      - The initial selection technique used in this implementation is ray-casting
     * */

    SteamVR_TrackedObject trackedObj;
    SteamVR_Controller.Device controller;

    public GameObject mirroredCube;
    public GameObject laserPrefab;
    private GameObject laser;
    private Transform laserTransform;
    private Vector3 hitPoint;

    private void ShowLaser(RaycastHit hit) {
        mirroredCube.SetActive(false);
        laser.SetActive(true);
        laserTransform.position = Vector3.Lerp(trackedObj.transform.position, hitPoint, .5f);
        laserTransform.LookAt(hitPoint);
        laserTransform.localScale = new Vector3(laserTransform.localScale.x, laserTransform.localScale.y, hit.distance);
        InstantiateObject(hit.transform.gameObject);
    }


    bool objSelected = false;
    public GameObject cameraHead;
    public GameObject cameraRig;
    //private GameObject virtualHand;
    private GameObject selectedObject;
    private Transform oldParent;
    float Disteh;
    float Disteo;
    float scaleAmount;

    Vector3 oldHeadScale;
    Vector3 oldCameraRigScale;

    //Tham's scale method
    public void ScaleAround(Transform target, Transform pivot, Vector3 scale) {
        Transform pivotParent = pivot.parent;
        Vector3 pivotPos = pivot.position;
        pivot.parent = target;
        target.localScale = scale;
        target.position += pivotPos - pivot.position;
        pivot.parent = pivotParent;
    }

    private void InstantiateObject(GameObject obj) {
        if (controller.GetPressDown(SteamVR_Controller.ButtonMask.Trigger)) {
            if (objSelected == false) {
                selectedObject = obj;
                oldParent = selectedObject.transform.parent;
                objSelected = true;
                laser.SetActive(false);

                Disteh = Vector3.Distance(cameraHead.transform.position, trackedObj.transform.position);
                Disteo = Vector3.Distance(cameraHead.transform.position, obj.transform.position);
                print("cameraHead:"+ cameraHead.transform.position);
                print("hand:" + trackedObj.transform.position);
                print("object:" + obj.transform.localPosition);

                scaleAmount = Disteo / Disteh;
                print("scale amount:" + scaleAmount);
                oldHeadScale = cameraHead.transform.localScale;
                oldCameraRigScale = cameraRig.transform.localScale;
                //cameraHead.transform.localScale = new Vector3(2f, 2f, 2f);
                ScaleAround(cameraRig.transform, cameraHead.transform, new Vector3(scaleAmount, scaleAmount, scaleAmount));

                Vector3 eyeProportion = cameraHead.transform.localScale / scaleAmount;
                //Keep eye distance proportionate to original position
                cameraHead.transform.localScale = eyeProportion;
                //ScaleAround(cameraRig.transform, cameraHead.transform, new Vector3(4f, 4f, 4f));
                //selectedObject.transform.SetParent(trackedObj.transform);
            } else if (objSelected == true) {
                resetProperties();
            }
        }
    }

    bool objectGrabbed = false;

    private void OnTriggerStay(Collider col) {
        if (objSelected == true) {
            if (controller.GetTouchDown(SteamVR_Controller.ButtonMask.Trigger)) {
                col.gameObject.transform.SetParent(trackedObj.gameObject.transform);
                objectGrabbed = true;
            }
            if (controller.GetTouchUp(SteamVR_Controller.ButtonMask.Trigger) && objectGrabbed == true) {
                col.gameObject.transform.SetParent(null);
                objectGrabbed = false;
                resetProperties();
            }
        }
    }

    void resetProperties() {
        objSelected = false;
        selectedObject.transform.SetParent(oldParent);
        cameraHead.transform.localScale = new Vector3(1f, 1f, 1f);
        cameraRig.transform.localScale = new Vector3(1f, 1f, 1f);
        cameraRig.transform.localPosition = new Vector3(0f, 0f, 0f);
    }

    private void WorldGrab() {
        /*
        //print("updating frame..");
        virtualHand.transform.localEulerAngles = trackedObj.transform.localEulerAngles;
        //each frame
        float Disthcurr = Vector3.Distance(trackedObj.transform.position, cameraHead.transform.position); // Physical hand distance
        float Distvh = Disthcurr * (Disto / Disth); // Virtual hand distance
        Vector3 thcurr = (trackedObj.transform.position - cameraHead.transform.position);
        Vector3 VirtualHandPos = cameraHead.transform.position + Distvh * (thcurr);
        virtualHand.transform.position = VirtualHandPos;
        */
        if (controller.GetPressDown(SteamVR_Controller.ButtonMask.ApplicationMenu)) { // temp
            //Resetting everything back to normal
            objSelected = false;
            selectedObject.transform.SetParent(oldParent);
            cameraHead.transform.localScale = new Vector3(1f, 1f, 1f);
            cameraRig.transform.localScale = new Vector3(1f, 1f, 1f);
            cameraRig.transform.localPosition = new Vector3(0f, 0f, 0f);
        }
    }

    private void castRay() {
        Ray ray = Camera.main.ScreenPointToRay(trackedObj.transform.position);
        ShowLaser();
        RaycastHit hit;
        if (Physics.Raycast(trackedObj.transform.position, transform.forward, out hit, 100)) {
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

    // Use this for initialization
    void Start() {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
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
            WorldGrab();
        }
    }
}
