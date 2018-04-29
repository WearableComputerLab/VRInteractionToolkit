using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SerialSelectionMode : MonoBehaviour {

    private SteamVR_TrackedObject trackedObj;
    private SteamVR_Controller.Device controller;
    public GameObject mirroredCube;

    public GameObject laserPrefab;
    private GameObject laser;
    private Transform laserTransform;
    private Vector3 hitPoint;
    private Vector3 hitPoint2D;
    private bool pickUpObjectsActive = false;
    public Material outlineMaterial;

    //Giving a weird get_FrameCount error in the console for some reason?
    /*int rightIndex = SteamVR_Controller.GetDeviceIndex(SteamVR_Controller.DeviceRelation.Rightmost);
    /int leftIndex = SteamVR_Controller.GetDeviceIndex(SteamVR_Controller.DeviceRelation.Leftmost);*/

    private void ShowLaser(RaycastHit hit) {
        mirroredCube.SetActive(false);
        laser.SetActive(true);
        laserTransform.position = Vector3.Lerp(trackedObj.transform.position, hitPoint, .5f);
        laserTransform.LookAt(hitPoint);
        laserTransform.localScale = new Vector3(laserTransform.localScale.x, laserTransform.localScale.y, hit.distance);
    }
    private void ShowLaser() {
        laser.SetActive(true);
        mirroredCube.SetActive(true);
    }

    public bool controllerRightPicked;
    public bool controllerLeftPicked;

    void Awake() {
        GameObject controllerRight = GameObject.Find("Controller (right)");
        GameObject controllerLeft = GameObject.Find("Controller (left)");
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
    
    private List<GameObject> selectedObjectsList = new List<GameObject>();
    private List<Material> rendererMaterialTrackerList = new List<Material>();

    void selectObject(GameObject obj) {
        Vector3 controllerPos = trackedObj.transform.forward;
        if (trackedObj != null && pickUpObjectsActive == false) {
            if (controller.GetTouchDown(SteamVR_Controller.ButtonMask.Trigger)) {
                if (obj != null && obj.name != "Mirrored Cube" && !selectedObjectsList.Contains(obj)) {
                    selectedObjectsList.Add(obj);
                    rendererMaterialTrackerList.Add(obj.transform.GetComponent<Renderer>().material);
                    obj.transform.GetComponent<Renderer>().material = outlineMaterial;
                    print("selected object:" + obj.name);
                    print("list size:" + selectedObjectsList.Count);
                } else {
                    for (int i=0; i<selectedObjectsList.Count; i++) {
                        selectedObjectsList[i].transform.GetComponent<Renderer>().material = rendererMaterialTrackerList[i];
                    }
                    selectedObjectsList.Clear();
                    print("Invalid selection, list cleared.");
                }
            }
        }
    }

    private bool objectsSelected = false;

    void activatePickupObjects() {
        if (controller.GetPressDown(SteamVR_Controller.ButtonMask.ApplicationMenu)) {
            pickUpObjectsActive = !pickUpObjectsActive;
            print("pick up objects set to:" + pickUpObjectsActive);
        }
        if (pickUpObjectsActive == true) {
            if (controller.GetTouchDown(SteamVR_Controller.ButtonMask.Trigger) && objectsSelected == false) {
                for (int i = 0; i < selectedObjectsList.Count; i++) {
                    if (selectedObjectsList[i].layer != LayerMask.NameToLayer("Ignore Raycast")) {
                        selectedObjectsList[i].transform.SetParent(trackedObj.transform);
                        objectsSelected = true;
                    }
                }
            }
            if (controller.GetTouchUp(SteamVR_Controller.ButtonMask.Trigger) && objectsSelected == true) {
                for (int i = 0; i < selectedObjectsList.Count; i++) {
                    if (selectedObjectsList[i].layer != LayerMask.NameToLayer("Ignore Raycast")) {
                        selectedObjectsList[i].transform.SetParent(null);
                        objectsSelected = false;
                    }
                }
            }
        }
    }

    void Update() {
        controller = SteamVR_Controller.Input((int)trackedObj.index);
        activatePickupObjects();
        mirroredObject();
        ShowLaser();
        Ray ray = Camera.main.ScreenPointToRay(trackedObj.transform.position);
        RaycastHit hit;
        if (Physics.Raycast(trackedObj.transform.position, trackedObj.transform.forward, out hit, 100)) {
            hitPoint = hit.point;
            //PickupObject(hit.transform.gameObject);
            ShowLaser(hit);
        }
        selectObject(hit.transform.gameObject);
    }
}
