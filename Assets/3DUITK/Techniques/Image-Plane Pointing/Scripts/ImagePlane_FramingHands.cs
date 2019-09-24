using UnityEngine;
using UnityEngine.Events;
using Valve.VR;

public class ImagePlane_FramingHands : MonoBehaviour {
	
    /* ImagePlane_FramingHands implementation by Kieran May
     * University of South Australia
     * 
     *  Copyright(C) 2019 Kieran May
	 *
	 *  This program is free software: you can redistribute it and/or modify
	 *  it under the terms of the GNU General Public License as published by
	 *  the Free Software Foundation, either version 3 of the License, or
	 *  (at your option) any later version.
	 * 
	 *  This program is distributed in the hope that it will be useful,
	 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
	 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
	 *  GNU General Public License for more details.
	 *
	 *  You should have received a copy of the GNU General Public License
	 *  along with this program.If not, see<http://www.gnu.org/licenses/>.
	 */
	 
#if SteamVR_Legacy
    private SteamVR_TrackedObject trackedObjL;
    private SteamVR_TrackedObject trackedObjR;
    private SteamVR_Controller.Device controllerL;
    private SteamVR_Controller.Device controllerR;
#elif SteamVR_2
    private SteamVR_Behaviour_Pose trackedObjL;
	private SteamVR_Behaviour_Pose trackedObjR;
    public SteamVR_Action_Boolean m_controllerPress;
    public SteamVR_Action_Boolean m_applicationMenu;
#else
    public GameObject trackedObjL;
    public GameObject trackedObjR;
#endif

    internal bool objSelected = false;

    public GameObject controllerRight;
    public GameObject controllerLeft;
    public GameObject cameraHead; // Camera etye
    public GameObject cameraRig;


    public GameObject currentlyPointingAt;
    private Vector3 castingBezierFrom;
    public LayerMask interactionLayers;



    public enum InteractionType { Selection, Manipulation_Movement, Manipulation_UI };
    public InteractionType interactionType;

    public UnityEvent selectedObjectEvent; // Invoked when an object is selected
    public UnityEvent droppedObject;
    public UnityEvent hovered; // Invoked when an object is hovered by technique
    public UnityEvent unHovered; // Invoked when an object is no longer hovered by the technique
    public GameObject selectedObject;



    public GameObject laserPrefab;
    private GameObject laser;
    private Transform laserTransform;
    private Vector3 hitPoint;
    private GameObject mirroredCube;
    public GameObject pointOfInteraction;

    private Transform oldParent;
    private GameObject lastSelectedObject; // holds the selected object

    private Vector3 positionBeforeScale; // The position of camerarig when entering scaled mode



    private void ShowLaser(RaycastHit hit) {
        float controllerDist = Vector3.Distance(trackedObjL.transform.position, trackedObjR.transform.position);
        mirroredCube.SetActive(false);
        laser.SetActive(true);
        laserTransform.position = Vector3.Lerp(pointOfInteraction.transform.position, hitPoint, .5f);
        laserTransform.LookAt(hitPoint);
        laserTransform.localScale = new Vector3(controllerDist, controllerDist, hit.distance);
        //InstantiateObject(hit.transform.gameObject);
    }

    public static Vector3 leftController = new Vector3(0, 0, 0);
    public static Vector3 rightController = new Vector3(0, 0, 0);

    public static Vector3 leftLaser = new Vector3(0, 0, 0);
    public static Vector3 rightLaser = new Vector3(0, 0, 0);

    void createOffSet(GameObject obj) {
        Vector3 controllerPos = pointOfInteraction.transform.forward;
        Vector3 pos = pointOfInteraction.transform.position;
        float distance_formula_on_vector = Mathf.Sqrt(controllerPos.x * controllerPos.x + controllerPos.y * controllerPos.y + controllerPos.z * controllerPos.z);
        pos.x += (extendDistance / (distance_formula_on_vector)) * controllerPos.x;
        pos.y += (extendDistance / (distance_formula_on_vector)) * controllerPos.y;
        pos.z += (extendDistance / (distance_formula_on_vector)) * controllerPos.z;
        obj.transform.position = pos;
    }

    private void interactionPosition() {
        Vector3 crossed = Vector3.Lerp(trackedObjL.transform.position, trackedObjR.transform.position, 0.5f);
        pointOfInteraction.transform.localPosition = crossed;
        pointOfInteraction.transform.localRotation = Quaternion.RotateTowards(trackedObjR.transform.rotation, trackedObjL.transform.rotation, 0);
        pointOfInteraction.transform.localRotation *= Quaternion.Euler(75, 0, 0);
    }

    internal void resetProperties() {
        objSelected = false;
        selectedObject.transform.SetParent(oldParent);
        cameraHead.transform.localScale = new Vector3(1f, 1f, 1f);
        cameraRig.transform.localScale = new Vector3(1f, 1f, 1f);
        cameraRig.transform.localPosition = positionBeforeScale;
    }

    //Tham's scale method
    public void ScaleAround(Transform target, Transform pivot, Vector3 scale) {
        Transform pivotParent = pivot.parent;
        Vector3 pivotPos = pivot.position;
        pivot.parent = target;
        target.localScale = scale;
        target.position += pivotPos - pivot.position;
        pivot.parent = pivotParent;
    }
    float Disteh;
    float Disteo;
    float scaleAmount;
    //Scale camera down attempt
    Vector3 oldHeadScale;
    Vector3 oldCameraRigScale;
    private void InstantiateObject(GameObject obj) {
        if (controllerEvents() == ControllerState.TRIGGER_DOWN) {
            if (objSelected == false && obj.transform.name != "Mirrored Cube") {
                if (interactionType == InteractionType.Selection) {
                    selectedObject = obj;
                    selectedObjectEvent.Invoke();
                    objSelected = true;
                } else if (interactionType == InteractionType.Manipulation_UI && this.GetComponent<SelectionManipulation>().inManipulationMode == false) {
                    selectedObject = obj;
                    objSelected = true;
                    this.GetComponent<SelectionManipulation>().selectedObject = obj;
                 } else if (interactionType == InteractionType.Manipulation_Movement) {
                    selectedObject = obj;
                    selectedObjectEvent.Invoke();
                    oldParent = selectedObject.transform.parent;
                    float dist = Vector3.Distance(pointOfInteraction.transform.position, selectedObject.transform.position);
                    selectedObject.transform.SetParent(pointOfInteraction.transform);
                    selectedObject.transform.localPosition = new Vector3(0f, 0f, 0f);

                    Vector3 controllerPos = pointOfInteraction.transform.forward;
                    Vector3 pos = pointOfInteraction.transform.position;
                    float distance_formula_on_vector = Mathf.Sqrt(controllerPos.x * controllerPos.x + controllerPos.y * controllerPos.y + controllerPos.z * controllerPos.z);
                    float distextended = 0.25f;
                    pos.x += (distextended / (distance_formula_on_vector)) * controllerPos.x;
                    pos.y += (distextended / (distance_formula_on_vector)) * controllerPos.y;
                    pos.z += (distextended / (distance_formula_on_vector)) * controllerPos.z;
                    obj.transform.position = pos;

                    selectedObject.transform.localScale = new Vector3(selectedObject.transform.localScale.x / dist, selectedObject.transform.localScale.y / dist, selectedObject.transform.localScale.z / dist);
                    print("Scaled to:" + selectedObject.transform.localScale.x);
                    //float dist = Vector3.Distance(pointOfInteraction.transform.position, selectedObject.transform.position);
                    //print(dist);

                    objSelected = true;
                    laser.SetActive(false);

                    positionBeforeScale = cameraRig.transform.localPosition;

                    Disteh = Vector3.Distance(cameraHead.transform.position, pointOfInteraction.transform.position);
                    Disteo = Vector3.Distance(cameraHead.transform.position, obj.transform.position);
                    print("cameraHead:" + cameraHead.transform.position);
                    print("hand:" + pointOfInteraction.transform.position);
                    print("object:" + obj.transform.localPosition);

                    scaleAmount = Disteo / Disteh;
                    print("scale amount:" + scaleAmount);
                    oldHeadScale = cameraHead.transform.localScale;
                    oldCameraRigScale = cameraRig.transform.localScale;
                    ScaleAround(cameraRig.transform, cameraHead.transform, new Vector3(scaleAmount, scaleAmount, scaleAmount));
                    //selectedObject.transform.localScale = new Vector3(scaleAmount, scaleAmount, scaleAmount);
                    Vector3 eyeProportion = cameraHead.transform.localScale / scaleAmount;
                    //Keep eye distance proportionate to original position
                    cameraHead.transform.localScale = eyeProportion;
                }

            } else if (objSelected == true) {
                if (interactionType == InteractionType.Manipulation_Movement) {
                    resetProperties();
                }
                if (interactionType == InteractionType.Selection) {
                    objSelected = false;
                }
                droppedObject.Invoke();
            }
        }
    }

    public enum ControllerState {
        TRIGGER_UP, TRIGGER_DOWN, APPLICATION_MENU, NONE
    }

    private ControllerState controllerEvents() {
#if SteamVR_Legacy
        if (controllerL.GetPressDown(SteamVR_Controller.ButtonMask.ApplicationMenu)) {
            return ControllerState.APPLICATION_MENU;
        }
        if (controllerL.GetPressDown(SteamVR_Controller.ButtonMask.Trigger)) {
            return ControllerState.TRIGGER_DOWN;
        }
#elif SteamVR_2
        if (m_controllerPress.GetStateDown(trackedObjL.inputSource)) {
            return ControllerState.TRIGGER_DOWN;
        } if (m_applicationMenu.GetStateDown(trackedObjL.inputSource)) {
            return ControllerState.APPLICATION_MENU;
        }
#endif
        return ControllerState.NONE;
    }

    private void WorldGrab() {
        if (controllerEvents() == ControllerState.APPLICATION_MENU) { // temp
                                                                      //Resetting everything back to normal
            objSelected = false;
            selectedObject.transform.SetParent(oldParent);
            cameraHead.transform.localScale = new Vector3(1f, 1f, 1f);
            cameraRig.transform.localScale = new Vector3(1f, 1f, 1f);
            cameraRig.transform.localPosition = new Vector3(0f, 0f, 0f);
        }
    }

    void checkSurroundingObjects() {

        Vector3 newForward = pointOfInteraction.transform.position - cameraHead.transform.position;

        Vector3 forwardVectorFromRemote = newForward;
        Vector3 positionOfRemote = cameraHead.transform.position;

        // This way is quite innefficient but is the way described for the bendcast.
        // Might make an example of a way that doesnt loop through everything
        var allObjects = FindObjectsOfType<GameObject>();

        float shortestDistance = float.MaxValue;

        GameObject objectWithShortestDistance = null;
        // Loop through objects and look for closest (if of a viable layer)
        for (int i = 0; i < allObjects.Length; i++) {
            // dont have to worry about executing twice as an object can only be on one layer
            if (interactionLayers == (interactionLayers | (1 << allObjects[i].layer))) {
                // Check if object is on plane projecting in front of VR remote. Otherwise ignore it. (we dont want our laser aiming backwards)
                Vector3 forwardParallelToDirectionPointing = Vector3.Cross(forwardVectorFromRemote, cameraHead.transform.up);
                Vector3 targObject = pointOfInteraction.transform.position - allObjects[i].transform.position;
                Vector3 perp = Vector3.Cross(forwardParallelToDirectionPointing, targObject);
                float side = Vector3.Dot(perp, cameraHead.transform.up);
                if (side < 0) {
                    // Object can only have one layer so can do calculation for object here
                    Vector3 objectPosition = allObjects[i].transform.position;

                    // Using vector algebra to get shortest distance between object and vector 
                    Vector3 forwardControllerToObject = pointOfInteraction.transform.position - objectPosition;
                    Vector3 controllerForward = forwardVectorFromRemote;
                    float distanceBetweenRayAndPoint = Vector3.Magnitude(Vector3.Cross(forwardControllerToObject, controllerForward)) / Vector3.Magnitude(controllerForward);



                    Vector3 newPoint = new Vector3(forwardVectorFromRemote.x * distanceBetweenRayAndPoint + positionOfRemote.x, forwardVectorFromRemote.y * distanceBetweenRayAndPoint + positionOfRemote.y
                            , forwardVectorFromRemote.z * distanceBetweenRayAndPoint + positionOfRemote.z);

                    if (distanceBetweenRayAndPoint < shortestDistance) {
                        shortestDistance = distanceBetweenRayAndPoint;
                        objectWithShortestDistance = allObjects[i];
                    }
                }

            }
        }
        if (objectWithShortestDistance != null) {

            // Invoke un-hover if object with shortest distance is now different to currently hovered
            if (currentlyPointingAt != objectWithShortestDistance) {
                unHovered.Invoke();
            }

            // setting the object that is being pointed at
            currentlyPointingAt = objectWithShortestDistance;

            hovered.Invoke(); // Broadcasting that object is hovered

            castingBezierFrom = pointOfInteraction.transform.position;

        } else {
            // Laser didnt reach any object so will disable
            currentlyPointingAt = null;
            lastSelectedObject = null;
        }
    }

    private void ShowLaser() {
        laser.SetActive(true);
        leftLaser = laser.transform.position;
        mirroredCube.SetActive(true);
    }


    private float extendDistance = 0f;
    private float cursorSpeed = 20f; // Decrease to make faster, Increase to make slower

    void mirroredObject() {
        Vector3 controllerPos = cameraHead.transform.forward;
        float distance_formula_on_vector = Mathf.Sqrt(controllerPos.x * controllerPos.x + controllerPos.y * controllerPos.y + controllerPos.z * controllerPos.z);
        Vector3 mirroredPos = pointOfInteraction.transform.position;

        mirroredPos.x = mirroredPos.x + (100f / (distance_formula_on_vector)) * controllerPos.x;
        mirroredPos.y = mirroredPos.y + (100f / (distance_formula_on_vector)) * controllerPos.y;
        mirroredPos.z = mirroredPos.z + (100f / (distance_formula_on_vector)) * controllerPos.z;

        mirroredCube.transform.position = mirroredPos;
        mirroredCube.transform.rotation = pointOfInteraction.transform.rotation;
    }

    void Awake() {
#if SteamVR_Legacy
        trackedObjL = controllerRight.GetComponent<SteamVR_TrackedObject>();
        trackedObjR = controllerLeft.GetComponent<SteamVR_TrackedObject>();
#elif SteamVR_2
        trackedObjL = controllerRight.GetComponent<SteamVR_Behaviour_Pose>();
		trackedObjR = controllerLeft.GetComponent<SteamVR_Behaviour_Pose>();
#endif
        mirroredCube = this.transform.Find("Mirrored Cube").gameObject;
        if (interactionType == InteractionType.Manipulation_UI) {
            this.gameObject.AddComponent<SelectionManipulation>();
            this.GetComponent<SelectionManipulation>().trackedObj = trackedObjL;
#if SteamVR_2
            this.GetComponent<SelectionManipulation>().m_controllerPress = m_controllerPress;
#endif
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
            InstantiateObject(currentlyPointingAt.gameObject);
        }
        interactionPosition();
        mirroredObject();
        ShowLaser();
        RaycastHit hit;
        if (Physics.Raycast(pointOfInteraction.transform.position, cameraHead.transform.forward, out hit, 100)) {
            hitPoint = hit.point;
            ShowLaser(hit);
        }
    }

    void Update() {
#if SteamVR_Legacy
        controllerL = SteamVR_Controller.Input((int)trackedObjL.index);
        controllerR = SteamVR_Controller.Input((int)trackedObjR.index);
#endif
        //if (objSelected == false) {
        castRay();
        if (objSelected == true) {
            WorldGrab(); //Using the ScaledWorldGrab to scale down the world
        }
    }

}