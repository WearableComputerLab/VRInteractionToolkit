using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaledWorldGrab : MonoBehaviour {

    /* HOMER implementation by Kieran May
     * University of South Australia
     * 
     * The HOMER algorithm I wrote is based off: (pg 34-35) https://people.cs.vt.edu/~bowman/3dui.org/course_notes/siggraph2001/basic_techniques.pdf 
     * 
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

    Vector3 oldScale;

    private void InstantiateObject(GameObject obj) {
        if (controller.GetPressDown(SteamVR_Controller.ButtonMask.Trigger)) {
            if (objSelected == false) {
                selectedObject = obj;
                oldParent = selectedObject.transform.parent;
                objSelected = true;
                //selectedObject.transform.SetParent(virtualHand.transform);
                laser.SetActive(false);

                Disteh = Vector3.Distance(cameraHead.transform.position, trackedObj.transform.position);
                Disteo = Vector3.Distance(cameraHead.transform.position, obj.transform.position);
                scaleAmount = Disteo / Disteh;
                /*foreach (Transform children in cameraRig.transform) {
                    if (children.gameObject != cameraHead) {
                        children.localScale = new Vector3(scaleAmount, scaleAmount, scaleAmount);
                    }
                }*/
                print("scale amount:" + scaleAmount);
                oldScale = cameraHead.transform.localScale;
                cameraHead.transform.localScale = new Vector3(scaleAmount, scaleAmount, scaleAmount);
                selectedObject.transform.SetParent(trackedObj.transform);
            }
        }
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
        if (controller.GetPressDown(SteamVR_Controller.ButtonMask.Trigger)) {
            objSelected = false;
            selectedObject.transform.SetParent(oldParent);
            cameraHead.transform.localScale = oldScale;
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
