using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ImagePlane_StickyHand : MonoBehaviour {
    internal bool objSelected = false;
    public GameObject cameraHead;
    public GameObject cameraRig;

    /*public GameObject controllerRight = GameObject.Find("Controller (right)");
    public GameObject controllerLeft = GameObject.Find("Controller (left)");
    */

    public LayerMask interactionLayers;
    public GameObject lastSelectedObject; // holds the selected object

    public GameObject currentlyPointingAt;
    private Vector3 castingBezierFrom;
    public GameObject controllerRight = null;
    public GameObject controllerLeft = null;

    private SteamVR_TrackedObject trackedObj;
    private SteamVR_Controller.Device controller;
    public GameObject laserPrefab;
    private GameObject laser;
    private Transform laserTransform;
    private Vector3 hitPoint;
    private GameObject mirroredCube;
    private GameObject pointOfInteraction;
    internal GameObject selectedObject;
    private Transform oldParent;
    public Material outlineMaterial;

	public UnityEvent selectedObjectEvent; // Invoked when an object is selected

	public UnityEvent hovered; // Invoked when an object is hovered by technique
	public UnityEvent unHovered; // Invoked when an object is no longer hovered by the technique

    public enum InteractionType { Selection, Manipulation_Movement, Manipulation_Full };
    public InteractionType interacionType;

    public enum ControllerPicked { Left_Controller, Right_Controller };
    public ControllerPicked controllerPicked;


void checkSurroundingObjects()
    {

        Vector3 newForward = pointOfInteraction.transform.position - cameraHead.transform.position;

        Vector3 forwardVectorFromRemote = newForward;
        Vector3 positionOfRemote = cameraHead.transform.position;

        // This way is quite innefficient but is the way described for the bendcast.
        // Might make an example of a way that doesnt loop through everything
        var allObjects = FindObjectsOfType<GameObject>();

        float shortestDistance = float.MaxValue;

        GameObject objectWithShortestDistance = null;
        // Loop through objects and look for closest (if of a viable layer)
        for (int i = 0; i < allObjects.Length; i++)
        {
            // dont have to worry about executing twice as an object can only be on one layer
			if (interactionLayers == (interactionLayers | (1 << allObjects[i].layer)))
            {
                // Check if object is on plane projecting in front of VR remote. Otherwise ignore it. (we dont want our laser aiming backwards)
                Vector3 forwardParallelToDirectionPointing = Vector3.Cross(forwardVectorFromRemote, cameraHead.transform.up);
                Vector3 targObject = pointOfInteraction.transform.position-allObjects[i].transform.position;
                Vector3 perp = Vector3.Cross(forwardParallelToDirectionPointing, targObject);
                float side = Vector3.Dot(perp, cameraHead.transform.up);
                if(side < 0) {
                        // Object can only have one layer so can do calculation for object here
                    Vector3 objectPosition = allObjects[i].transform.position;

                    // Using vector algebra to get shortest distance between object and vector 
                    Vector3 forwardControllerToObject = pointOfInteraction.transform.position - objectPosition;
                    Vector3 controllerForward = forwardVectorFromRemote;
                    float distanceBetweenRayAndPoint = Vector3.Magnitude(Vector3.Cross(forwardControllerToObject,controllerForward))/Vector3.Magnitude(controllerForward);
                    
        

                    Vector3 newPoint = new Vector3(forwardVectorFromRemote.x * distanceBetweenRayAndPoint + positionOfRemote.x, forwardVectorFromRemote.y * distanceBetweenRayAndPoint + positionOfRemote.y
                            , forwardVectorFromRemote.z * distanceBetweenRayAndPoint + positionOfRemote.z);

                    if (distanceBetweenRayAndPoint < shortestDistance)
                    {
                        shortestDistance = distanceBetweenRayAndPoint;
                        objectWithShortestDistance = allObjects[i];
                    }
                }
                
            }         
        }
        if (objectWithShortestDistance != null)
        {
            
            // Invoke un-hover if object with shortest distance is now different to currently hovered
            if(currentlyPointingAt != objectWithShortestDistance) {
                unHovered.Invoke();
            }

            // setting the object that is being pointed at
            currentlyPointingAt = objectWithShortestDistance;
            
            hovered.Invoke(); // Broadcasting that object is hovered

            castingBezierFrom = trackedObj.transform.position;

        } else {
            // Laser didnt reach any object so will disable
            currentlyPointingAt = null;
            lastSelectedObject = null;
        }
    }
    private void ShowLaser(RaycastHit hit) {

        mirroredCube.SetActive(false);
        laser.SetActive(true);
        laserTransform.position = Vector3.Lerp(pointOfInteraction.transform.position, hitPoint, .5f);
        laserTransform.LookAt(hitPoint);
        laserTransform.localScale = new Vector3(laserTransform.localScale.x, laserTransform.localScale.y, hit.distance);
        //print(hit.transform.name);
        //PickupObject(hit.transform.gameObject);
        //PickupObject(currentlyPointingAt.gameObject);
    }

    private void interactionPosition() {
        pointOfInteraction.transform.position = trackedObj.transform.position;
        pointOfInteraction.transform.rotation = trackedObj.transform.rotation;
        pointOfInteraction.transform.localRotation *= Quaternion.Euler(75, 0, 0);
    }

    internal void resetProperties() {
        objSelected = false;
        selectedObject.transform.SetParent(oldParent);
        cameraHead.transform.localScale = new Vector3(1f, 1f, 1f);
        cameraRig.transform.localScale = new Vector3(1f, 1f, 1f);
        cameraRig.transform.localPosition = new Vector3(0f, 0f, 0f);
    }

    private Material oldMaterial;

    public void PickupObject(GameObject obj) {
        if (trackedObj != null) {
            if(controller.GetPressDown(SteamVR_Controller.ButtonMask.Trigger) && objSelected == false) {
                if(interacionType == InteractionType.Manipulation_Movement) {
                    selectedObject = obj;
                    oldParent = obj.transform.parent;
                    float dist = Vector3.Distance(trackedObj.transform.position, obj.transform.position);
                    obj.transform.position = Vector3.Lerp(trackedObj.transform.position, obj.transform.position, (obj.transform.localScale.x / dist) / dist);
                    obj.transform.localScale = new Vector3(obj.transform.localScale.x / dist, obj.transform.localScale.y / dist, obj.transform.localScale.z / dist);
                    obj.transform.SetParent(trackedObj.transform);
                    objSelected = true;
                } else if(interacionType == InteractionType.Selection) {
                    if(selectedObject != null && oldMaterial != null) {
                        selectedObject.transform.GetComponent<Renderer>().material = oldMaterial;
                    }
                    selectedObject = obj;
                    //oldMaterial = obj.transform.GetComponent<Renderer>().material;
                    //obj.transform.GetComponent<Renderer>().material = outlineMaterial;

                }
            } else if(controller.GetPressDown(SteamVR_Controller.ButtonMask.Trigger) && objSelected == true) {
                if(interacionType == InteractionType.Manipulation_Movement) {
                    //print("reset.."+oldParent+" | obj:"+ selectedObject);
                    selectedObject.transform.SetParent(oldParent);
                    objSelected = false;
                }
            }
        }
    }

    private void ShowLaser() {
        laser.SetActive(true);
        mirroredCube.SetActive(true);
        
        //print("not hitting..");
        //laser.transform.localScale = new Vector3(laser.transform.localScale.x + 0.1f, laser.transform.localScale.y + 0.1f, laser.transform.localScale.z + 0.1f);
    }

    void extendDistance(float distance, GameObject obj) {
        Vector3 controllerPos = trackedObj.transform.forward;
        Vector3 pos = trackedObj.transform.position;
        float distance_formula_on_vector = Mathf.Sqrt(controllerPos.x * controllerPos.x + controllerPos.y * controllerPos.y + controllerPos.z * controllerPos.z);
        // Using formula to find a point which lies at distance on a 3D line from vector and direction
        pos.x -= (distance / (distance_formula_on_vector)) * controllerPos.x;
        pos.y -= (distance / (distance_formula_on_vector)) * controllerPos.y;
        pos.z -= (distance / (distance_formula_on_vector)) * controllerPos.z;

        obj.transform.position = pos;
        obj.transform.rotation = trackedObj.transform.rotation;
    }


    private float cursorSpeed = 20f; // Decrease to make faster, Increase to make slower

    /*void mirroredObject() {
        Vector3 controllerPos = cameraHead.transform.forward;
        float distance_formula_on_vector = Mathf.Sqrt(controllerPos.x * controllerPos.x + controllerPos.y * controllerPos.y + controllerPos.z * controllerPos.z);
        Vector3 mirroredPos = pointOfInteraction.transform.position;

        mirroredPos.x = mirroredPos.x + (100f / (distance_formula_on_vector)) * controllerPos.x;
        mirroredPos.y = mirroredPos.y + (100f / (distance_formula_on_vector)) * controllerPos.y;
        mirroredPos.z = mirroredPos.z + (100f / (distance_formula_on_vector)) * controllerPos.z;

        mirroredCube.transform.position = mirroredPos;
        mirroredCube.transform.rotation = pointOfInteraction.transform.rotation;
    }*/


    
       void mirroredObject() {
        Vector3 controllerPos = pointOfInteraction.transform.forward;
        float distance_formula_on_vector = Mathf.Sqrt(controllerPos.x * controllerPos.x + controllerPos.y * controllerPos.y + controllerPos.z * controllerPos.z);
        Vector3 mirroredPos = pointOfInteraction.transform.position;

        mirroredPos.x = mirroredPos.x + (100f / (distance_formula_on_vector)) * controllerPos.x;
        mirroredPos.y = mirroredPos.y + (100f / (distance_formula_on_vector)) * controllerPos.y;
        mirroredPos.z = mirroredPos.z + (100f / (distance_formula_on_vector)) * controllerPos.z;

        mirroredCube.transform.position = mirroredPos;
        mirroredCube.transform.rotation = pointOfInteraction.transform.rotation;
    }
     

    void Awake() {

        //cameraHead = GameObject.Find("Camera (eye)");
        //cameraRig = GameObject.Find("[CameraRig]");
        pointOfInteraction = this.transform.Find("InteractionPoint").gameObject;
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

    void Start() {
        laser = Instantiate(laserPrefab);
        laserTransform = laser.transform;
    }

    void castRay() {
        checkSurroundingObjects();
        print(currentlyPointingAt);
        selectedObject = currentlyPointingAt;
        if (currentlyPointingAt != null) {
            PickupObject(currentlyPointingAt.gameObject);
        }
        interactionPosition();
        mirroredObject();
        
        Ray ray = Camera.main.ScreenPointToRay(cameraHead.transform.position);

        Vector3 newForward = pointOfInteraction.transform.position - cameraHead.transform.position;
        RaycastHit hit;
        if (Physics.Raycast(cameraHead.transform.position, newForward, out hit, 100)) {
            hitPoint = hit.point;
            ShowLaser(hit);
        } else {
            ShowLaser();
        }
    }

    void Update() {
        
        controller = SteamVR_Controller.Input((int)trackedObj.index);
        //if (objSelected == false) {
        castRay();
    }

}
