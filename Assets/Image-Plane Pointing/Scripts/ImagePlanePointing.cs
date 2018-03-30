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

    /*private void move2DObject() {
        if (pickedObj2D != null) {
            if (controller.GetTouch(SteamVR_Controller.ButtonMask.Trigger)) {
                print("Touching 2D Object");
            }
        }
    }*/

    void Awake() {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
    }

    void Start() {
        laser = Instantiate(laserPrefab);
        laserTransform = laser.transform;
    }

    //Y -0.4 DOWN | +0.4 UP
    //X +0.4 RIGHT | -0.4 LEFT
    private void OnTriggerStay(Collider col) {
        //print("Colliding with " + col.name);
        if (pickedObj2D != null) {
            if (col.name == pickedObj2D.name) {
                if (controller.GetTouch(SteamVR_Controller.ButtonMask.Trigger)) {
                    //Debug.Log("You have collided with " + col.name + " while holding down Touch");
                    col.gameObject.transform.SetParent(this.gameObject.transform);
                    //col.gameObject.transform.position += new Vector3(0f, this.transform.position.y / 20f, 0f);
                    //print("x:"+this.transform.position.x + " | "+ "y:" + this.transform.position.y);
                }
                if (controller.GetTouchUp(SteamVR_Controller.ButtonMask.Trigger)) {
                    //Debug.Log("You have released Touch while colliding with " + col.name);
                    pickedObj2D.gameObject.transform.SetParent(panel.transform);
                    float newX = pickedObj2D.transform.localPosition.x * 10;
                    float newY = pickedObj2D.transform.localPosition.y * 10;
                    print("Y2:" + newY + " | X2:" + newX);
                    pickedObj.transform.position = new Vector3(pickedObj.transform.position.x + newX, pickedObj.transform.position.y + newY, pickedObj.transform.position.z);
                    pickedObj.transform.rotation = new Quaternion(pickedObj2D.transform.localRotation.x, pickedObj2D.transform.localRotation.y, pickedObj2D.transform.localRotation.z, pickedObj2D.transform.localRotation.w);
                    //pickedObj2D.transform.position = new Vector3(0f, 0f, 0f);
                }
            }
        }
    }
    //0.1 Y right
    //0.1 
    private void OnTriggerExit(Collider col) {
        if (pickedObj2D != null) {
            if (col.name == pickedObj2D.name) {
                //print("Y:" + pickedObj2D.transform.position.y + " | X:" + pickedObj2D.transform.position.x);
                //print("Y2:" + pickedObj2D.transform.localPosition.y + " | X2:" + pickedObj2D.transform.localPosition.x);
            }
        }
    }

    private GameObject pickedObj2D = null;
    private GameObject pickedObj = null;
    void generate2DObjects(GameObject pickedObject) {
        if (pickedObject.transform.tag == "PickableObject" && currentlyModifying == false) {
            panel.SetActive(true);
            currentlyModifying = true;
            pickedObj = pickedObject;
            pickedObj2D = Instantiate(pickedObject, new Vector3(0f, 0f, 0f), Quaternion.identity) as GameObject;
            pickedObj2D.transform.SetParent(panel.transform,false);
            pickedObj2D.gameObject.AddComponent<Rigidbody>();
            pickedObj2D.GetComponent<Collider>().attachedRigidbody.isKinematic = true;
            pickedObj2D.transform.localScale = new Vector3(0.25f, 0.25f, 0f);
            pickedObj2D.transform.localRotation = Quaternion.identity;
            pickedObj2D.transform.localPosition = Vector3.zero;
        }
    }

    void Update() {
        controller = SteamVR_Controller.Input((int)trackedObj.index);
        //move2DObject();
        Ray ray = Camera.main.ScreenPointToRay(trackedObj.transform.position);
        if (controller.GetPress(SteamVR_Controller.ButtonMask.Touchpad)) {
            currentlyModifying = false;
            panel.SetActive(false);
            if (pickedObj2D != null) {
                Destroy(pickedObj2D);
            }
            RaycastHit hit;
            if (Physics.Raycast(trackedObj.transform.position, transform.forward, out hit, 100)) {
                //print("hit:" + hit.transform.name);
                generate2DObjects(hit.transform.gameObject);
                hitPoint = hit.point;
                ShowLaser(hit);
            }
        } else {
            laser.SetActive(false);
        }
    }
}
