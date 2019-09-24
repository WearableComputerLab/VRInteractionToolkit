using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Valve.VR;

public class ImagePlane_StickyHand : MonoBehaviour {
	
	  /* ImagePlane_StickyHands implementation by Kieran May
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
    private SteamVR_TrackedObject trackedObj;
    private SteamVR_Controller.Device controller;
#elif SteamVR_2
    private SteamVR_Behaviour_Pose trackedObj;
    public SteamVR_Action_Boolean m_controllerPress;
#else
    public GameObject trackedObj;
#endif

    internal bool objSelected = false;
    public GameObject cameraHead;
    public GameObject cameraRig;

    public LayerMask interactionLayers;
    private GameObject lastSelectedObject; // holds the selected object

    public GameObject currentlyPointingAt;
    private Vector3 castingBezierFrom;
    public GameObject controllerRight = null;
    public GameObject controllerLeft = null;

    public GameObject laserPrefab;
    private GameObject laser;
    private Transform laserTransform;
    private Vector3 hitPoint;
    private GameObject mirroredCube;
    private GameObject pointOfInteraction;   
    private Transform oldParent;
    public Material outlineMaterial;

	public UnityEvent selectedObjectEvent; // Invoked when an object is selected
    public UnityEvent droppedObject;
	public UnityEvent hovered; // Invoked when an object is hovered by technique
	public UnityEvent unHovered; // Invoked when an object is no longer hovered by the technique
	public GameObject selectedObject;

    public enum InteractionType { Selection, Manipulation_Movement, Manipulation_UI };
    public InteractionType interactionType;

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

    public enum ControllerState {
        TRIGGER_UP, TRIGGER_DOWN, NONE
    }

    private ControllerState controllerEvents() {
#if SteamVR_Legacy
        if (controller.GetPressDown(Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger)) {
            return ControllerState.TRIGGER_DOWN;
        }
        if (controller.GetPressUp(Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger)) {
            return ControllerState.TRIGGER_UP;
        }
#elif SteamVR_2
        if (m_controllerPress.GetStateDown(trackedObj.inputSource)) {
            return ControllerState.TRIGGER_DOWN;
        }
        if (m_controllerPress.GetStateUp(trackedObj.inputSource)) {
            return ControllerState.TRIGGER_UP;
        }
#endif
        return ControllerState.NONE;
    }

    private Material oldMaterial;

    public void PickupObject(GameObject obj) {
        if (trackedObj != null) {
            if(controllerEvents() == ControllerState.TRIGGER_DOWN && objSelected == false) {
                if (interactionType == InteractionType.Manipulation_Movement) {
                    selectedObject = obj;
                    oldParent = obj.transform.parent;
                    float dist = Vector3.Distance(trackedObj.transform.position, obj.transform.position);
                    obj.transform.position = Vector3.Lerp(trackedObj.transform.position, obj.transform.position, (obj.transform.localScale.x / dist) / dist);
                    obj.transform.localScale = new Vector3(obj.transform.localScale.x / dist, obj.transform.localScale.y / dist, obj.transform.localScale.z / dist);
                    obj.transform.SetParent(trackedObj.transform);
                    objSelected = true;
                    selectedObjectEvent.Invoke();
                } else if (interactionType == InteractionType.Selection) {
                    if (selectedObject != null && oldMaterial != null) {
                        selectedObject.transform.GetComponent<Renderer>().material = oldMaterial;
                    }
                    selectedObject = obj;
                    selectedObjectEvent.Invoke();
                    //oldMaterial = obj.transform.GetComponent<Renderer>().material;
                    //obj.transform.GetComponent<Renderer>().material = outlineMaterial;

                } else if (interactionType == InteractionType.Manipulation_UI && this.GetComponent<SelectionManipulation>().inManipulationMode == false) {
                    selectedObject = obj;
                    objSelected = true;
                    this.GetComponent<SelectionManipulation>().selectedObject = obj;
                }
            } else if(controllerEvents() == ControllerState.TRIGGER_DOWN && objSelected == true) {
                if(interactionType == InteractionType.Manipulation_Movement) {
                    //print("reset.."+oldParent+" | obj:"+ selectedObject);
                    selectedObject.transform.SetParent(oldParent);
                    objSelected = false;
                    droppedObject.Invoke();
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

    private void initializeControllers() {
        if (controllerPicked == ControllerPicked.Right_Controller) {
#if SteamVR_Legacy
            trackedObj = controllerRight.GetComponent<SteamVR_TrackedObject>();
#elif SteamVR_2
            trackedObj = controllerRight.GetComponent<SteamVR_Behaviour_Pose>();
#endif
        } else if (controllerPicked == ControllerPicked.Left_Controller) {
#if SteamVR_Legacy
            trackedObj = controllerLeft.GetComponent<SteamVR_TrackedObject>();
#elif SteamVR_2
            trackedObj = controllerLeft.GetComponent<SteamVR_Behaviour_Pose>();
#endif
        } else {
            print("Couldn't detect trackedObject, please specify the controller type in the settings.");
            Application.Quit();
        }

    }

    void Awake() {

        //cameraHead = GameObject.Find("Camera (eye)");
        //cameraRig = GameObject.Find("[CameraRig]");
        pointOfInteraction = this.transform.Find("InteractionPoint").gameObject;
        mirroredCube = this.transform.Find("Mirrored Cube").gameObject;
        initializeControllers();
        if (interactionType == InteractionType.Manipulation_UI) {
            this.gameObject.AddComponent<SelectionManipulation>();
            this.GetComponent<SelectionManipulation>().trackedObj = trackedObj;
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
            PickupObject(currentlyPointingAt.gameObject);
        }
        interactionPosition();
        mirroredObject();

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
#if SteamVR_Legacy
        controller = SteamVR_Controller.Input((int)trackedObj.index);
#endif
        //if (objSelected == false) {
        castRay();
    }

}
