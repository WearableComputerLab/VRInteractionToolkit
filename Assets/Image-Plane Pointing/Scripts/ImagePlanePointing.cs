using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImagePlanePointing : MonoBehaviour {

    /* Image Plane Pointing implementation by Kieran May
    * University of South Australia
    * 
    * -Already implemented the selection aspect of the Flexible Pointer
    * ie point a laser at an object and generate a 2D clone on a mini UI of the 3D selected object.
    * 
    * -Haven't yet started on the object manipulation aspect of the Flexible Pointer
    * ie move/rotate the 2D object clone & effect the 3D object in real-time
    * 
    * TODO
    * -Implement as prefab
    * -Implment manipulation/interaction aspect of Image Plane Pointing
    * */

    private SteamVR_TrackedObject trackedObj;
    private SteamVR_Controller.Device controller;

    private bool currentlyModifying = false;
    private GameObject selectedObject = null;
    public GameObject laserPrefab;
    private GameObject laser;
    private Transform laserTransform;
    private Vector3 hitPoint;
    public GameObject panel;

    private void ShowLaser(RaycastHit hit) {
        laser.SetActive(true);
        laserTransform.position = Vector3.Lerp(trackedObj.transform.position, hitPoint, .5f);
        laserTransform.LookAt(hitPoint);
        laserTransform.localScale = new Vector3(laserTransform.localScale.x, laserTransform.localScale.y, hit.distance);
    }

    void Awake() {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
    }

    void Start() {
        laser = Instantiate(laserPrefab);
        laserTransform = laser.transform;
    }

    void generate2DObjects(GameObject pickedObject) {
        if (pickedObject.transform.tag == "PickableObject" && currentlyModifying == false) {
            panel.SetActive(true);
            currentlyModifying = true;
            GameObject pickedObj2D = Instantiate(pickedObject, new Vector3(0f, 0f, 0f), Quaternion.identity) as GameObject;
            pickedObj2D.transform.SetParent(panel.transform,false);
            pickedObj2D.transform.localScale = new Vector3(0.5f, 0.5f, 0f);
            pickedObj2D.transform.localRotation = Quaternion.identity;
            pickedObj2D.transform.localPosition = Vector3.zero;
            selectedObject = pickedObj2D;
        }
    }

    void Update() {
        controller = SteamVR_Controller.Input((int)trackedObj.index);
        Ray ray = Camera.main.ScreenPointToRay(trackedObj.transform.position);
        if (controller.GetPress(SteamVR_Controller.ButtonMask.Touchpad)) {
            currentlyModifying = false;
            panel.SetActive(false);
            if (selectedObject != null) {
                Destroy(selectedObject);
            }
            RaycastHit hit;
            if (Physics.Raycast(trackedObj.transform.position, transform.forward, out hit, 100)) {
                print("hit:" + hit.transform.name);
                generate2DObjects(hit.transform.gameObject);
                hitPoint = hit.point;
                ShowLaser(hit);
            }
        } else {
            laser.SetActive(false);
        }
    }
}
