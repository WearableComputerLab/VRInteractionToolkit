using UnityEngine;

public class FishingReel : MonoBehaviour {

    /* Fishing Reel implementation by Kieran May
     * University of South Australia
     * 
     * */

    private SteamVR_TrackedObject trackedObj;
    private SteamVR_Controller.Device controller;

    public GameObject laserPrefab;
    private GameObject laser;
    private Transform laserTransform;
    private Vector3 hitPoint;
    private GameObject mirroredCube;

    public enum InteractionType {Selection, Manipulation_Movement, Manipulation_Full};
    public InteractionType interacionType;

    public enum ControllerPicked { Left_Controller, Right_Controller };
    public ControllerPicked controllerPicked;

    internal bool objectSelected = false;

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

    private Valve.VR.EVRButtonId trigger = Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger;

    private bool pickedUpObject = false; //ensure only 1 object is picked up at a time
    internal GameObject tempObjectStored;
    public void PickupObject(GameObject obj) {
        Vector3 controllerPos = trackedObj.transform.forward;
        if (trackedObj != null) {
            if (controller.GetPressDown(trigger) && pickedUpObject == false) {
                if (interacionType == InteractionType.Manipulation_Movement && this.GetComponent<SelectionManipulation>().manipulationMovementEnabled == true) {
                    obj.transform.SetParent(trackedObj.transform);
                    extendDistance = Vector3.Distance(controllerPos, obj.transform.position);
                    tempObjectStored = obj; // Storing the object as an instance variable instead of using the obj parameter fixes glitch of it not properly resetting on TriggerUp
                    pickedUpObject = true;
                } else if (interacionType == InteractionType.Manipulation_Full && this.GetComponent<SelectionManipulation>().inManipulationMode == false) {
                    tempObjectStored = obj;
                    objectSelected = true;
                    this.GetComponent<SelectionManipulation>().selectedObject = obj;
                } else if (interacionType == InteractionType.Selection) {
                    tempObjectStored = obj;
                    objectSelected = true;
                }
            }
            if (controller.GetPressUp(trigger) && pickedUpObject == true) {
                if (interacionType == InteractionType.Manipulation_Movement) {
                    tempObjectStored.transform.SetParent(null);
                    pickedUpObject = false;
                }
                objectSelected = false;
            }
        }
    }

    private float extendDistance = 0f;
    private float cursorSpeed = 100f; // Decrease to make faster, Increase to make slower

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

    private GameObject manipulationIcons;

    void Awake() {
        GameObject controllerRight = GameObject.Find(CONSTANTS.rightController);
        GameObject controllerLeft = GameObject.Find(CONSTANTS.leftController);
        mirroredCube = this.transform.Find("Mirrored Cube").gameObject;
        if (controllerPicked == ControllerPicked.Right_Controller) {
            print(controllerRight);
            trackedObj = controllerRight.GetComponent<SteamVR_TrackedObject>();
        } else if (controllerPicked == ControllerPicked.Left_Controller) {
            trackedObj = controllerLeft.GetComponent<SteamVR_TrackedObject>();
        } else {
            print("Couldn't detect trackedObject, please specify the controller type in the settings.");
            Application.Quit();
        }

        if (interacionType == InteractionType.Manipulation_Full) {

            //this.gameObject.AddComponent<ColorPicker>();
            //this.GetComponent<ColorPicker>().trackedObj = trackedObj;
            this.gameObject.AddComponent<SelectionManipulation>();
            this.GetComponent<SelectionManipulation>().trackedObj = trackedObj;
            manipulationIcons = GameObject.Find("Manipulation_Icons");
            this.GetComponent<SelectionManipulation>().manipulationIcons = manipulationIcons;
        }

    }

    void Start() {
        //print("joystick names:" + Valve.VR.iN);
        laser = Instantiate(laserPrefab);
        laserTransform = laser.transform;
    }

    void Update() {
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

}
