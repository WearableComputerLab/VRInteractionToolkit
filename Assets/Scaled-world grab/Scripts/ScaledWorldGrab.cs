using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ScaledWorldGrab : MonoBehaviour {

    /* Scaled-world grab implementation by Kieran May
     * University of South Australia
     * 
     * The Scaled-world grab algorithm I wrote is based off: (pg 37) https://people.cs.vt.edu/~bowman/3dui.org/course_notes/siggraph2001/basic_techniques.pdf 
     *      - The initial selection technique used in this implementation is ray-casting
     * */
    public GameObject controllerCollider;
    public LayerMask interactionLayers;

    public GameObject controllerRight;
    public GameObject controllerLeft;

    internal SteamVR_TrackedObject trackedObj;
    internal SteamVR_Controller.Device controller;

    private GameObject mirroredCube;
    public GameObject laserPrefab;
    private GameObject laser;
    private Transform laserTransform;
    private Vector3 hitPoint;

    public enum InteractionType { Selection, Manipulation_Movement, Manipulation_Full };
    public InteractionType interacionType;
    internal GameObject tempObjectStored;

    public enum ControllerPicked { Left_Controller, Right_Controller };
    public ControllerPicked controllerPicked;

    private void ShowLaser(RaycastHit hit) {
        if (isInteractionlayer(hit.transform.gameObject))
        {
            mirroredCube.SetActive(false);
            laser.SetActive(true);
            laserTransform.position = Vector3.Lerp(trackedObj.transform.position, hitPoint, .5f);
            laserTransform.LookAt(hitPoint);
            laserTransform.localScale = new Vector3(laserTransform.localScale.x, laserTransform.localScale.y, hit.distance);
            InstantiateObject(hit.transform.gameObject);
        }
    }


    internal bool objSelected = false;
    public GameObject cameraHead;
    public  GameObject cameraRig;
    //private GameObject virtualHand;
    public GameObject selectedObject;
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
        if (controller.GetHairTriggerDown()) {
            if (!objSelected && obj.transform.name != "Mirrored Cube") {             
                selectedObject = obj;
                oldParent = selectedObject.transform.parent;
                objSelected = true;
                laser.SetActive(false);
                cameraHeadLocalScaleOriginal = cameraHead.transform.localScale;
                cameraRigLocalScaleOriginal = cameraRig.transform.localScale;
                cameraRigLocalPositionOriginal = cameraRig.transform.localPosition;
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

    internal bool objectGrabbed = false;
    public static int grabbedAmount;

    private GameObject entered;
    private void OnTriggerEnter(Collider col) {
        if(isInteractionlayer(col.gameObject)) {
            print("Entered" + col.name);
            entered = col.gameObject;
        }        
    }

    private void OnTriggerExit(Collider col) {
        if(isInteractionlayer(col.gameObject)) {

        }
    }

    private bool isInteractionlayer(GameObject obj) {
        return interactionLayers == (interactionLayers | (1 << obj.layer));
    }

    private Vector3 cameraHeadLocalScaleOriginal;
    private Vector3 cameraRigLocalScaleOriginal;
    private Vector3 cameraRigLocalPositionOriginal;

    internal void resetProperties() {
        objSelected = false;
        selectedObject.transform.SetParent(oldParent);
        cameraHead.transform.localScale = cameraHeadLocalScaleOriginal;
        cameraRig.transform.localScale = cameraRigLocalScaleOriginal;
        cameraRig.transform.localPosition = cameraRigLocalPositionOriginal;
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
            resetProperties();
        }
    }

    private void castRay() {
        mirroredObject();
        ShowLaser();
        Ray ray = Camera.main.ScreenPointToRay(trackedObj.transform.position);
        RaycastHit hit;
        if (Physics.Raycast(trackedObj.transform.position, trackedObj.transform.forward, out hit, 100)) {
            hitPoint = hit.point;
            ShowLaser(hit);
        } 
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

    private void ShowLaser() {
        laser.SetActive(true);
        mirroredCube.SetActive(true);
        Vector3 theVector = trackedObj.transform.forward;
        hitPoint = trackedObj.transform.position;
        float distance_formula_on_vector = Mathf.Sqrt(theVector.x * theVector.x + theVector.y * theVector.y + theVector.z * theVector.z);
        // Using formula to find a point which lies at distance on a 3D line from vector and direction
        hitPoint.x = hitPoint.x + (100 / (distance_formula_on_vector)) * theVector.x;
        hitPoint.y = hitPoint.y + (100 / (distance_formula_on_vector)) * theVector.y;
        hitPoint.z = hitPoint.z + (100 / (distance_formula_on_vector)) * theVector.z;

        laser.SetActive(true);
        laserTransform.position = Vector3.Lerp(trackedObj.transform.position, hitPoint, .5f);
        laserTransform.LookAt(hitPoint);
        laserTransform.localScale = new Vector3(laserTransform.localScale.x, laserTransform.localScale.y,
           100);
    }
    
    void Awake() {      
        cameraHead = GameObject.Find(CONSTANTS.cameraEyes);
        cameraRig = GameObject.Find(CONSTANTS.cameraRig);
        mirroredCube = this.transform.Find("Mirrored Cube").gameObject;
        if (controllerPicked == ControllerPicked.Right_Controller) {
            trackedObj = controllerRight.GetComponent<SteamVR_TrackedObject>();
        } else if (controllerPicked == ControllerPicked.Left_Controller) {
            trackedObj = controllerLeft.GetComponent<SteamVR_TrackedObject>();
        } else {
            print("Couldn't detect trackedObject, please specify the controller type in the settings.");
            Application.Quit();
        }
        controllerCollider.transform.parent = trackedObj.transform;
    }

    // Use this for initialization
    void Start() {
        laser = Instantiate(laserPrefab);
        laserTransform = laser.transform;
    }

    // Update is called once per frame
    void Update() {
        controller = SteamVR_Controller.Input((int)trackedObj.index);
        if(controller.GetHairTriggerUp() && objectGrabbed && objSelected) {          
            selectedObject.gameObject.transform.SetParent(null);
            objectGrabbed = false;
            resetProperties();            
        }
        if (objSelected == false) {
            castRay();
        } else if (objSelected) {
            WorldGrab();
        }
    }
}
