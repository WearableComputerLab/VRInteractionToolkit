using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereCasting : MonoBehaviour {

    /* Sphere-Casting implementation by Kieran May
    * University of South Australia
    * 
    * */

    private SteamVR_TrackedObject trackedObj;
    private SteamVR_Controller.Device controller;
    public PickupObjects pickupObjs;

    public GameObject laserPrefab;
    private GameObject laser;
    private Transform laserTransform;
    private Vector3 hitPoint;
    public GameObject mirroredCube;
    public GameObject sphereObject;

    private void ShowLaser(RaycastHit hit) {
        mirroredCube.SetActive(false);
        laser.SetActive(true);
        //sphereObject.transform.position = hit.transform.position;
        //sphereObject.transform.position = hitPoint;
        if (hit.transform.gameObject.name != "Mirrored Cube") {
            //sphereObject.transform.position = hitPoint;
            sphereObject.transform.position = hit.transform.position;
            sphereObject.SetActive(true);
            //pickupObjs.selectableObjects.Clear();
        } else {
            //pickupObjs.selectableObjects.Clear();
            //PickupObjects.selectableObjects.Clear();
            pickupObjs.clearList();
            sphereObject.SetActive(false);

        }
        laserTransform.position = Vector3.Lerp(trackedObj.transform.position, hitPoint, .5f);
        laserTransform.LookAt(hitPoint);
        laserTransform.localScale = new Vector3(laserTransform.localScale.x, laserTransform.localScale.y, hit.distance);
    }

    private void ShowLaser() {
        laser.SetActive(true);
        mirroredCube.SetActive(true);
    }

    private float extendRadius = 0f;
    private float cursorSpeed = 20f; // Decrease to make faster, Increase to make slower

    private void PadScrolling() {
        Vector3 controllerPos = trackedObj.transform.forward;
        if (controller.GetAxis().y != 0) {
            extendRadius += controller.GetAxis().y / cursorSpeed;
            sphereObject.transform.localScale = new Vector3((extendRadius) * 2, (extendRadius) * 2, (extendRadius) * 2);
        }
    }

    void Awake() {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
    }

    void Start() {
        laser = Instantiate(laserPrefab);
        laserTransform = laser.transform;
        pickupObjs = sphereObject.GetComponent<PickupObjects>();
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

    void Update() {
        controller = SteamVR_Controller.Input((int)trackedObj.index);
        mirroredObject();
        PadScrolling();
        ShowLaser();
        Ray ray = Camera.main.ScreenPointToRay(trackedObj.transform.position);
        RaycastHit hit;
        if (Physics.Raycast(trackedObj.transform.position, transform.forward, out hit, 100)) {
            //print("hit:" + hit.transform.name);
            hitPoint = hit.point;
            ShowLaser(hit);
            pickupObjs.PickupObject(controller, trackedObj, pickupObjs.getSelectableObjects());
            pickupObjs.clearList();
        }
    }

}
