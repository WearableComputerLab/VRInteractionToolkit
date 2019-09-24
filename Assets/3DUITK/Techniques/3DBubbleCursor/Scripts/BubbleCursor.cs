using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEditor;
using Valve.VR;

public class BubbleCursor : MonoBehaviour {

    /* 3D Bubble Cursor implementation by Kieran May
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
    public SteamVR_Action_Boolean m_touchpad;
    public SteamVR_Action_Vector2 m_touchpadAxis;
#else
    public GameObject trackedObj;
#endif

    private GameObject[] interactableObjects; // In-game objects
    private GameObject cursor;
    private float startRadius = 0f;
    private GameObject radiusBubble;
    private GameObject objectBubble;
    public LayerMask interactableLayer;

    public enum InteractionType { Selection, Manipulation_Movement, Manipulation_UI };
    public InteractionType interactionType;

    public enum ControllerPicked { Left_Controller, Right_Controller, Head };
    public ControllerPicked controllerPicked;

    public GameObject currentlyHovering = null;

    public GameObject controllerRight;
    public GameObject controllerLeft;
    public GameObject cameraHead;
    public UnityEvent selectedObject; // Invoked when an object is selected
    public UnityEvent droppedObject; // Invoked when an object is dropped
    public UnityEvent hovered; // Invoked when an object is hovered by technique
    public UnityEvent unHovered; // Invoked when an object is no longer hovered by the technique

    private readonly float bubbleOffset = 0.6f;

    private GameObject[] getInteractableObjects() {
        GameObject[] AllSceneObjects = FindObjectsOfType<GameObject>();
        List<GameObject> interactableObjects = new List<GameObject>();
        foreach (GameObject obj in AllSceneObjects) {
            if (obj.layer == Mathf.Log(interactableLayer.value, 2)) {
                interactableObjects.Add(obj);
            }
        }
        return interactableObjects.ToArray();
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
        } else if (controllerPicked == ControllerPicked.Head) {
#if SteamVR_Legacy
            trackedObj = cameraHead.GetComponent<SteamVR_TrackedObject>();
#elif SteamVR_2
            trackedObj = cameraHead.GetComponent<SteamVR_Behaviour_Pose>();
#endif
        } else {
            print("Couldn't detect trackedObject, please specify the controller type in the settings.");
            Application.Quit();
        }

    }

    void Awake() {
        cursor = this.transform.Find("BubbleCursor").gameObject;
        radiusBubble = cursor.transform.Find("RadiusBubble").gameObject;
        objectBubble = this.transform.Find("ObjectBubble").gameObject;
        initializeControllers();
        if (interactionType == InteractionType.Manipulation_UI) {
            this.gameObject.AddComponent<SelectionManipulation>();
            this.GetComponent<SelectionManipulation>().trackedObj = trackedObj;
        }
    }

        // Use this for initialization
    void Start () {
        interactableObjects = getInteractableObjects();
        startRadius = cursor.GetComponent<SphereCollider>().radius;
        extendDistance = Vector3.Distance(trackedObj.transform.position, cursor.transform.position);
        SetCursorParent();
        moveCursorPosition();
    }

    void SetCursorParent() {
        cursor.transform.SetParent(trackedObj.transform);
    }


    /// <summary>
    /// Loops through interactable gameObjects within the scene & stores them in a 2D array.
    ///     -2D array is used to keep store gameObjects distance & also keep track of their index. eg [0][0] returns objects distance [0][1] returns objects index.
    /// Using Linq 2D array is sorted based on closest distances
    /// </summary>
    /// <returns>2D Array which contains order of objects with the closest distances & their allocated index</returns>
	private float[][] FindClosestObjects() {
        float[] lowestDists = new float[4];
        lowestDists[0] = 0; // 1ST Lowest Distance
        lowestDists[1] = 0; // 2ND Lowest Distance
        lowestDists[2] = 0; // 1ST Lowest Index
        lowestDists[3] = 0; // 2ND Lowest Index
        float lowestDist = 0;
        float[][] allDists = new float[interactableObjects.Length][];
        for (int i = 0; i < interactableObjects.Length; i++) {
            allDists[i] = new float[2];
        }
        int lowestValue = 0;
        for (int i = 0; i < interactableObjects.Length; i++) {
            float dist = Vector3.Distance(cursor.transform.position, interactableObjects[i].transform.position);
            dist -= (interactableObjects[i].transform.lossyScale.x / 2f);
            /*
            Collider myCollider = interactableObjects[i].GetComponent<Collider>();
            if (myCollider.GetType() == typeof(SphereCollider) || myCollider.GetType() == typeof(CapsuleCollider)) {
                dist -= (interactableObjects[i].transform.lossyScale.x /2f);
            } else if (myCollider.GetType() == typeof(BoxCollider) || myCollider.GetType() == typeof(MeshCollider)) {
                dist -= (interactableObjects[i].transform.lossyScale.x / 2f);
            }*/
            if (i == 0) {
                lowestDist = dist;
                lowestValue = 0;
            } else {
                if (dist < lowestDist) {
                    lowestDist = dist;
                    lowestValue = i;
                }
            }
            allDists[i][0] = dist;
            allDists[i][1] = i;
            //allDists[i][2] = circleObjects[i].GetComponent<CircleCollider2D>().radius;
        }
        float[][] arraytest = allDists.OrderBy(row => row[0]).ToArray();
        arraytest = allDists.OrderBy(row => row[0]).ToArray();
        return arraytest;
    }

    /// <summary>
    /// Allows player to reposition the bubble cursor fowards & backwards.
    /// -Currently needs alot of work, only modifies the Z axis.
    /// </summary>
    private float extendDistance = 0f;
    public float cursorSpeed = 20f; // Decrease to make faster, Increase to make slower

    private void PadScrolling() {
#if SteamVR_Legacy
        if (controller.GetAxis().y != 0) {
            extendDistance += controller.GetAxis().y / cursorSpeed;
            moveCursorPosition();
        }
#elif SteamVR_2
        if (m_touchpadAxis.GetAxis(trackedObj.inputSource).y != 0) {
            extendDistance += m_touchpadAxis.GetAxis(trackedObj.inputSource).y / cursorSpeed;
            moveCursorPosition();
        }
#endif
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

    private bool pickedUpObject = false; //ensure only 1 object is picked up at a time
    internal GameObject lastSelectedObject;

    void PickupObject(GameObject obj) {
        if (trackedObj != null) {
            if (controllerEvents() == ControllerState.TRIGGER_DOWN && pickedUpObject == false) {
                if (interactionType == InteractionType.Manipulation_Movement) {
                    //obj.GetComponent<Collider>().attachedRigidbody.isKinematic = true;
                    obj.transform.SetParent(cursor.transform);
                    lastSelectedObject = obj; // Storing the object as an instance variable instead of using the obj parameter fixes glitch of it not properly resetting on TriggerUp
                    pickedUpObject = true;
                } else if (interactionType == InteractionType.Manipulation_UI && this.GetComponent<SelectionManipulation>().inManipulationMode == false) {
                    lastSelectedObject = obj;
                    pickedUpObject = true;
                    this.GetComponent<SelectionManipulation>().selectedObject = obj;
                } else if (interactionType == InteractionType.Selection) {
                    lastSelectedObject = obj;
                    pickedUpObject = true;                   
                }
                selectedObject.Invoke();
            }
            if (controllerEvents() == ControllerState.TRIGGER_UP && pickedUpObject == true) {
                if(interactionType == InteractionType.Manipulation_Movement) {
                    //obj.GetComponent<Collider>().attachedRigidbody.isKinematic = false;
                    lastSelectedObject.transform.SetParent(null);
                    pickedUpObject = false;
                    droppedObject.Invoke();
                }
                pickedUpObject = false;
            }
        }
    }

    void moveCursorPosition() {
        Vector3 controllerPos = trackedObj.transform.forward;
        Vector3 pose = trackedObj.transform.position;
        float distance_formula_on_vector = Mathf.Sqrt(controllerPos.x * controllerPos.x + controllerPos.y * controllerPos.y + controllerPos.z * controllerPos.z);
        // Using formula to find a point which lies at distance on a 3D line from vector and direction
        pose.x = pose.x + (extendDistance / (distance_formula_on_vector)) * controllerPos.x;
        pose.y = pose.y + (extendDistance / (distance_formula_on_vector)) * controllerPos.y;
        pose.z = pose.z + (extendDistance / (distance_formula_on_vector)) * controllerPos.z;

        cursor.transform.position = pose;
        cursor.transform.rotation = trackedObj.transform.rotation;
    }
 
    // Update is called once per frame
    void Update () {
        if (trackedObj != null) {
#if SteamVR_Legacy
            controller = SteamVR_Controller.Input((int)trackedObj.index);
#endif
            PadScrolling();
        }

        float[][] lowestDistances = FindClosestObjects();

        float ClosestCircleRadius = 0f;
        float SecondClosestCircleRadius = 0f;
        //Different colliders
        if(interactableObjects.Length >= 2) {
            ClosestCircleRadius = lowestDistances[0][0] + (interactableObjects[(int)lowestDistances[0][1]].transform.lossyScale.x / 2f) + (interactableObjects[(int)lowestDistances[0][1]].transform.lossyScale.x / 2f);
            SecondClosestCircleRadius = lowestDistances[1][0] - (interactableObjects[(int)lowestDistances[1][1]].transform.lossyScale.x / 2f) + (interactableObjects[(int)lowestDistances[1][1]].transform.lossyScale.x / 2f);

            float closestValue = Mathf.Min(ClosestCircleRadius, SecondClosestCircleRadius);

            if(ClosestCircleRadius < SecondClosestCircleRadius) {
                cursor.GetComponent<SphereCollider>().radius = (closestValue + ClosestCircleRadius) / 2f;
                radiusBubble.transform.localScale = new Vector3((closestValue + ClosestCircleRadius), (closestValue + ClosestCircleRadius), (closestValue + ClosestCircleRadius));
                objectBubble.transform.localScale = new Vector3(0f, 0f, 0f);
                PickupObject(interactableObjects[(int)lowestDistances[0][1]]);
            } else {
                cursor.GetComponent<SphereCollider>().radius = (closestValue + SecondClosestCircleRadius) / 2f;
                radiusBubble.transform.localScale = new Vector3((closestValue + SecondClosestCircleRadius), (closestValue + SecondClosestCircleRadius), (closestValue + SecondClosestCircleRadius));
                objectBubble.transform.position = interactableObjects[(int)lowestDistances[0][1]].transform.position;
                objectBubble.transform.localScale = new Vector3(interactableObjects[(int)lowestDistances[0][1]].transform.lossyScale.x + bubbleOffset, interactableObjects[(int)lowestDistances[0][1]].transform.lossyScale.y + bubbleOffset, interactableObjects[(int)lowestDistances[0][1]].transform.lossyScale.z + bubbleOffset);
                PickupObject(interactableObjects[(int)lowestDistances[0][1]]);
            }
            if(currentlyHovering != interactableObjects[(int)lowestDistances[0][1]]) {
                unHovered.Invoke();
            }           
            currentlyHovering = interactableObjects[(int)lowestDistances[0][1]];
            hovered.Invoke();

        } else {
            print("ERROR: Unable to initialize bubble cursor - Current objects using interactable layer in scene is:" +interactableObjects.Length + " (Requires atleast 2 GameObjects in scene to use bubble cursor)");
            print("- Make sure to set the GameObjects layer type to the Interactable Layer type specified in the prefab settings...");
        }
    }
}
