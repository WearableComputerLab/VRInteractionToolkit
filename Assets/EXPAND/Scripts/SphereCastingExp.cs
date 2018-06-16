using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereCastingExp : MonoBehaviour {

    /* Sphere-Casting implementation for EXPAND by Kieran May
    * University of South Australia
    * 
    * */

    private SteamVR_TrackedObject trackedObj;
    private SteamVR_Controller.Device controller;
    public static bool inMenu = false;
    internal ExpandMenu menu;

    public GameObject laserPrefab;
    private GameObject laser;
    private Transform laserTransform;
    private Vector3 hitPoint;
    private GameObject mirroredCube;
    private GameObject sphereObject;

    public enum InteractionType { Selection, Manipulation_Movement, Manipulation_Full };
    public InteractionType interacionType;

    public enum ControllerPicked { Left_Controller, Right_Controller };
    public ControllerPicked controllerPicked;

    private void ShowLaser(RaycastHit hit) {
        //print("object hit:" + hit.transform.gameObject.name);
        //menu.selectQuad(controller, hit.transform.gameObject);
        //print(inMenu);
        mirroredCube.SetActive(false);
        laser.SetActive(true);
        //sphereObject.transform.position = hit.transform.position;
        //sphereObject.transform.position = hitPoint;
        if (hit.transform.gameObject.name != "Mirrored Cube" && inMenu == false) {
            //sphereObject.transform.position = hitPoint;
            sphereObject.transform.position = hit.transform.position;
            sphereObject.SetActive(true);
        } else {
            //sphereObject.SetActive(false);

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
        GameObject controllerRight = GameObject.Find("Controller (right)");
        GameObject controllerLeft = GameObject.Find("Controller (left)");
        mirroredCube = this.transform.Find("Mirrored Cube").gameObject;
        sphereObject = this.transform.Find("SphereTooltip").gameObject;
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
        menu = sphereObject.GetComponent<ExpandMenu>();
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
        if (inMenu == false) {
            mirroredObject();
            PadScrolling();
        }
        ShowLaser();
        Ray ray = Camera.main.ScreenPointToRay(trackedObj.transform.position);
        RaycastHit hit;
        if (Physics.Raycast(trackedObj.transform.position, trackedObj.transform.forward, out hit, 100)) {
            //print("hit:" + hit.transform.name);
            hitPoint = hit.point;
            ShowLaser(hit);
            if (menu.isActive() == false) {
                menu.enableEXPAND(controller, trackedObj, menu.getSelectableObjects());
                menu.clearList();
            } else if (menu.isActive() == true) {
                menu.selectObject(controller, hit.transform.gameObject);
            }
        }
    }

}
