/*
 *  Flexible pointer - VR interaction tool allowing the user to bend a ray cast
 *  along a bezier curve utilizng the HTC Vive controllers.
 *  
 *  Copyright(C) 2018  Ian Hanan
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using UnityEngine.Events;

public class FlexiblePointer : MonoBehaviour {

#if SteamVR_Legacy
    public SteamVR_TrackedObject trackedObjL;
    public SteamVR_TrackedObject trackedObjR;

    private SteamVR_Controller.Device deviceL;
    private SteamVR_Controller.Device deviceR;
#elif SteamVR_2
    public SteamVR_Action_Boolean m_controllerPress;
    public SteamVR_Action_Vector2 m_touchpadAxis;

    public SteamVR_Behaviour_Pose trackedObjL;
    public SteamVR_Behaviour_Pose trackedObjR;
#else
    public GameObject trackedObjL;
    public GameObject trackedObjR;
#endif

    public LayerMask interactionLayers;

    public GameObject controlPoint;

    public bool controlPointVisible = true;

    public GameObject laserContainer;

    public float scaleFactor = 2f;

    private Vector3 point0; // bezier back
    private Vector3 point1; // bezier control
    private Vector3 point2; /// bezier front

    // Laser vars
    private int numOfLasers = 20;
    public GameObject laserPrefab;
    private GameObject[] lasers;
    private Transform[] laserTransform;
    private Vector3 hitPoint;

    public GameObject currentlyPointingAt; // Is the gameobject that the ray is currently touching

    public GameObject selection;

    public UnityEvent selectedObject; // Invoked when an object is selected

    public UnityEvent hovered; // Invoked when an object is hovered by technique
    public UnityEvent unHovered; // Invoked when an object is no longer hovered by the technique



    // Allows to choose if the script purley selects or has full manipulation
    public enum InteractionType { Selection, Manipulation, Manipulation_UI};
    public InteractionType interactionType;

    // Use this for initialization
    void Start() {
        // Initalizing all the lasers
        lasers = new GameObject[numOfLasers];
        laserTransform = new Transform[numOfLasers];
        for (int i = 0; i < numOfLasers; i++) {
            GameObject laserPart = Instantiate(laserPrefab, new Vector3((float)i, 1, 0), Quaternion.identity) as GameObject;
            laserTransform[i] = laserPart.transform;
            lasers[i] = laserPart;
            laserPart.transform.parent = laserContainer.transform;
        }
        setPoint0and1();
        if (interactionType == InteractionType.Manipulation_UI) {
            this.gameObject.AddComponent<SelectionManipulation>();
            this.GetComponent<SelectionManipulation>().trackedObj = trackedObjR;
        }
    }

    // Returns 1 for controller 1 and 2 for controller 2
    int calculatePointingController() {
        Vector3 playerPos = this.transform.position;
        float distTo1 = Vector3.Distance(playerPos, trackedObjL.transform.position);
        float distTo2 = Vector3.Distance(playerPos, trackedObjR.transform.position);
        return 1;
    }

    void setPoint0and1() {
        // Setting test points
        Vector3 controller1Pos = trackedObjL.transform.position;

        Vector3 controller2Pos = trackedObjR.transform.position;

        Vector3 forwardVectorBetweenRemotes;

        // Will extend further based on the scale factor
        // by multiplying the distance between controllers by it
        // and calculating new end control point
        float distanceBetweenControllers = Vector3.Distance(controller1Pos, controller2Pos) * scaleFactor;

        if (calculatePointingController() == 1) {
            forwardVectorBetweenRemotes = new Vector3(controller1Pos.x - controller2Pos.x, controller1Pos.y - controller2Pos.y, controller1Pos.z - controller2Pos.z);

            // Start control point
            point0 = controller2Pos;

            float distance_formula_on_vector = Mathf.Sqrt(forwardVectorBetweenRemotes.x * forwardVectorBetweenRemotes.x + forwardVectorBetweenRemotes.y * forwardVectorBetweenRemotes.y + forwardVectorBetweenRemotes.z * forwardVectorBetweenRemotes.z);

            // End control point
            point2.x = controller1Pos.x + (distanceBetweenControllers / (distance_formula_on_vector)) * forwardVectorBetweenRemotes.x;
            ;
            point2.y = controller1Pos.y + (distanceBetweenControllers / (distance_formula_on_vector)) * forwardVectorBetweenRemotes.y;
            ;
            point2.z = controller1Pos.z + (distanceBetweenControllers / (distance_formula_on_vector)) * forwardVectorBetweenRemotes.z;
            ;
        } else {
            forwardVectorBetweenRemotes = new Vector3(controller2Pos.x - controller1Pos.x, controller2Pos.y - controller1Pos.y, controller2Pos.z - controller1Pos.z);

            // Start control point
            point0 = controller1Pos;

            float distance_formula_on_vector = Mathf.Sqrt(forwardVectorBetweenRemotes.x * forwardVectorBetweenRemotes.x + forwardVectorBetweenRemotes.y * forwardVectorBetweenRemotes.y + forwardVectorBetweenRemotes.z * forwardVectorBetweenRemotes.z);

            // End control point
            point2.x = controller2Pos.x + (distanceBetweenControllers / (distance_formula_on_vector)) * forwardVectorBetweenRemotes.x;
            point2.y = controller2Pos.y + (distanceBetweenControllers / (distance_formula_on_vector)) * forwardVectorBetweenRemotes.y;
            point2.z = controller2Pos.z + (distanceBetweenControllers / (distance_formula_on_vector)) * forwardVectorBetweenRemotes.z;
        }

        setCurveControlPoint();
    }

    // Initalizes point1 's curve
    void setCurveControlPoint() {

#if SteamVR_Legacy
        // Will use touchpads to calculate touchpoint
        deviceL = SteamVR_Controller.Input((int)trackedObjL.index);
        deviceR = SteamVR_Controller.Input((int)trackedObjR.index);

        Vector2 touchpadL = (deviceL.GetAxis(EVRButtonId.k_EButton_Axis0)); // Getting reference to the touchpad
        Vector2 touchpadR = (deviceR.GetAxis(EVRButtonId.k_EButton_Axis0)); // Getting reference to the touchpad
#elif SteamVR_2
        Vector2 touchpadL = (m_touchpadAxis.GetAxis(trackedObjL.inputSource)); // Getting reference to the touchpad
        Vector2 touchpadR = (m_touchpadAxis.GetAxis(trackedObjL.inputSource)); // Getting reference to the touchpad
#else
        //not supported without SteamVR
        Vector2 touchpadL = Vector2.zero;
        Vector2 touchpadR = Vector2.zero;
#endif


        // Set the controllable distance to be the distance between the end of laser to back remote
        float distanceToMoveControlPoint = Vector3.Distance(point2, trackedObjR.transform.position);

        if (float.IsNaN(distanceToMoveControlPoint)) {
            // error with calculation will return
            return;
        }

        // Checking touchpad L
        float xvalL = touchpadL.x;
        float yvalL = touchpadL.y;

        // Checking touchpad R
        float xvalR = touchpadR.x;
        float yvalR = touchpadR.y;

        // getting between the front of the flexible pointer to the back of the remotes
        Vector3 forwardBetweenRemotes = point2 - trackedObjR.transform.position;
        Vector3 middleOfRemotes = (point2 + trackedObjR.transform.position) / 2f;

        // moving along y axis acording to R y
        controlPoint.transform.position = vectorDistanceAlongFoward(distanceToMoveControlPoint * (yvalR), middleOfRemotes, forwardBetweenRemotes);
        // now need to move left and right by getting the side vector forward 
        Vector3 sideForward = Vector3.Cross(forwardBetweenRemotes, Vector3.up);
        controlPoint.transform.position = vectorDistanceAlongFoward(distanceToMoveControlPoint * (xvalR * -1), controlPoint.transform.position, sideForward);
        // now need to control depth using the other controller
        controlPoint.transform.position = vectorDistanceAlongFoward(distanceToMoveControlPoint * (yvalL), controlPoint.transform.position, Vector3.up);

        // setting the actual bezier curve point to follow control point
        point1 = controlPoint.transform.position;

    }

    private Vector3 vectorDistanceAlongFoward(float theDistance, Vector3 startPos, Vector3 forward) {
        return startPos + forward.normalized * theDistance;
    }

    public enum ControllerState {
        TRIGGER_UP, TRIGGER_DOWN, NONE
    }

    private ControllerState controllerEvents() {
#if SteamVR_Legacy
        if (deviceR.GetHairTriggerDown()) {
            return ControllerState.TRIGGER_DOWN;
        }
#elif SteamVR_2
        if (m_controllerPress.GetStateDown(trackedObjR.inputSource)) {
            return ControllerState.TRIGGER_DOWN;
        }
#endif
        return ControllerState.NONE;
    }

    // Update is called once per frame
    void Update() {
        checkControlPointVisibility();
        setPoint0and1();
        castBezierRay();

        // checking for selection
        if (controllerEvents() == ControllerState.TRIGGER_DOWN && currentlyPointingAt != null) {

            if (interactionType == InteractionType.Selection) {
                // Pure Selection
                selection = currentlyPointingAt;
                selectedObject.Invoke();


            } else if (interactionType == InteractionType.Manipulation) {
                // Currently no manipulation
                selection = currentlyPointingAt;
            } else if (interactionType == InteractionType.Manipulation_UI) {
                selection = currentlyPointingAt;
                this.GetComponent<SelectionManipulation>().selectedObject = selection;
            }
            selectedObject.Invoke();
        }
    }

    void castBezierRay() {
        float valueToSearchBezierBy = 0f;

        Vector3 positionOfLastLaserPart;
        if (calculatePointingController() == 1) {
            positionOfLastLaserPart = trackedObjR.transform.position;
        } else {
            positionOfLastLaserPart = trackedObjL.transform.position;
        }

        // Used to see if ANY of the lasers collided with an object
        bool foundObject = false;

        for (int i = 0; i < numOfLasers; i++) {
            lasers[i].SetActive(true);
            Vector3 nextPart = getBezierPoint(valueToSearchBezierBy);
            float distBetweenParts = Vector3.Distance(nextPart, positionOfLastLaserPart);
            if (float.IsNaN(distBetweenParts)) {
                // error with calculation will return
                return;
            }
            laserTransform[i].position = Vector3.Lerp(positionOfLastLaserPart, nextPart, 0.5f);
            laserTransform[i].LookAt(nextPart);
            laserTransform[i].localScale = new Vector3(laserTransform[i].localScale.x, laserTransform[i].localScale.y,
        distBetweenParts);

            positionOfLastLaserPart = nextPart;
            valueToSearchBezierBy += (1f / numOfLasers);

            if (i > 0) {
                if (!foundObject) {
                    // Do a ray cast check on each part to check for collision (extended from laser part) 
                    // First object collided with is the only one that will select
                    Vector3 dir = laserTransform[i - 1].forward;
                    RaycastHit hit;
                    if (Physics.Raycast(positionOfLastLaserPart, dir, out hit, distBetweenParts)) {
                        // no object previouslly was highlighted so just highlight this one
                        if (interactionLayers == (interactionLayers | (1 << hit.transform.gameObject.layer))) {
                            if (currentlyPointingAt != hit.transform.gameObject) {
                                unHovered.Invoke(); // unhover old object
                            }

                            currentlyPointingAt = hit.transform.gameObject;
                            hovered.Invoke();
                            foundObject = true;
                        }
                    }
                }
            }
        }
        if (!foundObject) {
            // no object was hit so unhover and deselect
            unHovered.Invoke();
            currentlyPointingAt = null;
        }
    }


    // t being betweek 0 and 1 to get a spot on the curve
    Vector3 getBezierPoint(float t) {
        return (Mathf.Pow(1 - t, 2) * point0 + 2 * (1 - t) * t * point1 + Mathf.Pow(t, 2) * point2);
    }

    // sets whether the user can see the control point. Will be called if user changes the bool variable setting
    private void checkControlPointVisibility() {
        if (controlPointVisible) {
            controlPoint.GetComponent<MeshRenderer>().enabled = true;
        } else {
            controlPoint.GetComponent<MeshRenderer>().enabled = false;
        }
    }
}
