using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class BubbleCursor : MonoBehaviour {
    
    /* 3D Bubble Cursor implementation by Kieran May
     * University of South Australia
     * 
     * TODO 
     * -Make compatible with the Oculus Rift
     * -Detect controllers & other gameobjects through script
     * -Make bubble cursor compatible with all type of gameobject shapes
     * -Refactor code
     * */

    private GameObject[] interactableObjects; // In-game objects
    private GameObject cursor;
    private float startRadius = 0f;
    private GameObject radiusBubble;
    private GameObject objectBubble;

    private SteamVR_TrackedObject trackedObj;
    private SteamVR_Controller.Device controller;

    public enum InteractionType { Selection, Manipulation_Movement, Manipulation_Full };
    public InteractionType interacionType;

    public enum ControllerPicked { Left_Controller, Right_Controller, Head };
    public ControllerPicked controllerPicked;


    private readonly float bubbleOffset = 0.6f;

    void Awake() {
        GameObject controllerRight = GameObject.Find(CONSTANTS.rightController);
        GameObject controllerLeft = GameObject.Find(CONSTANTS.leftController);
        GameObject cameraHead = GameObject.Find(CONSTANTS.cameraEyes);
        cursor = this.transform.Find("BubbleCursor").gameObject;
        radiusBubble = cursor.transform.Find("RadiusBubble").gameObject;
        objectBubble = this.transform.Find("ObjectBubble").gameObject;

        if (controllerPicked == ControllerPicked.Right_Controller) {
            trackedObj = controllerRight.GetComponent<SteamVR_TrackedObject>();
        } else if (controllerPicked == ControllerPicked.Left_Controller) {
            trackedObj = controllerLeft.GetComponent<SteamVR_TrackedObject>();
        } else if (controllerPicked == ControllerPicked.Head) {
            trackedObj = cameraHead.GetComponent<SteamVR_TrackedObject>();
        } else {
            print("Couldn't detect trackedObject, please specify the controller type in the settings.");
            Application.Quit();
        }
    }

        // Use this for initialization
    void Start () {
        interactableObjects = GameObject.FindGameObjectsWithTag("InteractableObjects");
        startRadius = cursor.GetComponent<SphereCollider>().radius;
        extendDistance = Vector3.Distance(trackedObj.transform.position, cursor.transform.position);
        SetCursorParent();
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
            Collider myCollider = interactableObjects[i].GetComponent<Collider>();
            if (myCollider.GetType() == typeof(SphereCollider)) {
                dist -= interactableObjects[i].GetComponent<SphereCollider>().radius * interactableObjects[i].transform.localScale.x;
            } else if (myCollider.GetType() == typeof(CapsuleCollider)) {
                dist -= interactableObjects[i].GetComponent<CapsuleCollider>().radius * interactableObjects[i].transform.localScale.x;
            } else if (myCollider.GetType() == typeof(BoxCollider)) {
                dist -= interactableObjects[i].GetComponent<BoxCollider>().size.x * interactableObjects[i].transform.localScale.x;
            }
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
        /*	float[] lowestDists = new float[4];
            lowestDists[0] = 0; // 1ST Lowest Distance
            lowestDists[1] = 0; // 2ND Lowest Distance
            lowestDists[2] = 0; // 1ST Lowest Index
            lowestDists [3] = 0; // 2ND Lowest Index
            float lowestDist = 0;
            float[][] allDists = new float[interactableObjects.Length][];
            for (int i=0; i< interactableObjects.Length; i++) {
                allDists[i] = new float[2];
            }

            int lowestValue = 0;
            for (int i = 0; i < interactableObjects.Length; i++) {
                float dist = Vector3.Distance(cursor.transform.position, interactableObjects[i].transform.position)/2f;
                if (i == 0) {
                    lowestDist = dist;
                    lowestValue = 0;
                } else {
                    if (dist < lowestDist) {
                        lowestDist = dist;
                        lowestValue = i;
                    }
                }
                allDists [i][0] = dist;
                allDists [i][1] = i;
            }
            float[][] arraytest = allDists.OrderBy(row => row[0]).ToArray();
            return arraytest;*/
    }

    /// <summary>
    /// Allows player to reposition the bubble cursor fowards & backwards.
    /// -Currently needs alot of work, only modifies the Z axis.
    /// </summary>
    private float extendDistance = 0f;
    public float cursorSpeed = 20f; // Decrease to make faster, Increase to make slower

    private void PadScrolling() {
        if (controller.GetAxis().y != 0) {
            //print(controller.GetAxis().y);
            //cursor.transform.position += new Vector3(0f, 0f, controller.GetAxis().y/20);
            extendDistance += controller.GetAxis().y / cursorSpeed;
            moveCursorPosition();
        }
    }

    private bool pickedUpObject = false; //ensure only 1 object is picked up at a time
    private GameObject tempObjectStored;
    void PickupObject(GameObject obj) {
        if (trackedObj != null) {
            if (controller.GetTouchDown(SteamVR_Controller.ButtonMask.Trigger) && pickedUpObject == false) {
                //obj.GetComponent<Collider>().attachedRigidbody.isKinematic = true;
                obj.transform.SetParent(cursor.transform);
                tempObjectStored = obj; // Storing the object as an instance variable instead of using the obj parameter fixes glitch of it not properly resetting on TriggerUp
                pickedUpObject = true;
            }
            if (controller.GetTouchUp(SteamVR_Controller.ButtonMask.Trigger) && pickedUpObject == true) {
                //obj.GetComponent<Collider>().attachedRigidbody.isKinematic = false;
                tempObjectStored.transform.SetParent(null);
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
    private GameObject selectedObject;
    // Update is called once per frame
    void Update () {
        if (trackedObj != null) {
            controller = SteamVR_Controller.Input((int)trackedObj.index);
            PadScrolling();
        }

        float[][] lowestDistances = FindClosestObjects();

        float ClosestCircleRadius = 0f;
        float SecondClosestCircleRadius = 0f;
        //Different colliders

        //SphereCollider
        if (interactableObjects[(int)lowestDistances[0][1]].GetComponent<Collider>().GetType() == typeof(SphereCollider)) {
            ClosestCircleRadius = lowestDistances[0][0] + (interactableObjects[(int)lowestDistances[0][1]].GetComponent<SphereCollider>().radius * interactableObjects[(int)lowestDistances[0][1]].transform.localScale.x) + (interactableObjects[(int)lowestDistances[0][1]].GetComponent<SphereCollider>().radius * interactableObjects[(int)lowestDistances[0][1]].transform.localScale.x);
        }
        if (interactableObjects[(int)lowestDistances[1][1]].GetComponent<Collider>().GetType() == typeof(SphereCollider)) {
            SecondClosestCircleRadius = lowestDistances[1][0] - (interactableObjects[(int)lowestDistances[1][1]].GetComponent<SphereCollider>().radius * interactableObjects[(int)lowestDistances[1][1]].transform.localScale.x) + (interactableObjects[(int)lowestDistances[1][1]].GetComponent<SphereCollider>().radius * interactableObjects[(int)lowestDistances[1][1]].transform.localScale.x);
        }
        //CapsuleCollider
        if (interactableObjects[(int)lowestDistances[0][1]].GetComponent<Collider>().GetType() == typeof(CapsuleCollider)) {
            ClosestCircleRadius = lowestDistances[0][0] + (interactableObjects[(int)lowestDistances[0][1]].GetComponent<CapsuleCollider>().radius * interactableObjects[(int)lowestDistances[0][1]].transform.localScale.x) + (interactableObjects[(int)lowestDistances[0][1]].GetComponent<CapsuleCollider>().radius * interactableObjects[(int)lowestDistances[0][1]].transform.localScale.x);
        }
        if (interactableObjects[(int)lowestDistances[1][1]].GetComponent<Collider>().GetType() == typeof(CapsuleCollider)) {
            SecondClosestCircleRadius = lowestDistances[1][0] - (interactableObjects[(int)lowestDistances[1][1]].GetComponent<CapsuleCollider>().radius * interactableObjects[(int)lowestDistances[1][1]].transform.localScale.x) + (interactableObjects[(int)lowestDistances[1][1]].GetComponent<CapsuleCollider>().radius * interactableObjects[(int)lowestDistances[1][1]].transform.localScale.x);
        }
        //BoxCollider
        if (interactableObjects[(int)lowestDistances[0][1]].GetComponent<Collider>().GetType() == typeof(BoxCollider)) {
            ClosestCircleRadius = lowestDistances[0][0] + (interactableObjects[(int)lowestDistances[0][1]].GetComponent<BoxCollider>().size.x * interactableObjects[(int)lowestDistances[0][1]].transform.localScale.x) + (interactableObjects[(int)lowestDistances[0][1]].GetComponent<BoxCollider>().size.x * interactableObjects[(int)lowestDistances[0][1]].transform.localScale.x);
        }
        if (interactableObjects[(int)lowestDistances[1][1]].GetComponent<Collider>().GetType() == typeof(BoxCollider)) {
            SecondClosestCircleRadius = lowestDistances[1][0] - (interactableObjects[(int)lowestDistances[1][1]].GetComponent<BoxCollider>().size.x * interactableObjects[(int)lowestDistances[1][1]].transform.localScale.x) + (interactableObjects[(int)lowestDistances[1][1]].GetComponent<BoxCollider>().size.x * interactableObjects[(int)lowestDistances[1][1]].transform.localScale.x);
        }

        float closestValue = Mathf.Min(ClosestCircleRadius, SecondClosestCircleRadius);

        if (ClosestCircleRadius < SecondClosestCircleRadius) {
            cursor.GetComponent<SphereCollider>().radius = (closestValue + ClosestCircleRadius)/2f;
            radiusBubble.transform.localScale = new Vector3((closestValue + ClosestCircleRadius), (closestValue + ClosestCircleRadius), (closestValue + ClosestCircleRadius));
            objectBubble.transform.localScale = new Vector3(0f, 0f, 0f);
            PickupObject(interactableObjects[(int)lowestDistances[0][1]]);
        } else {
            cursor.GetComponent<SphereCollider>().radius = (closestValue + SecondClosestCircleRadius)/2f;
            radiusBubble.transform.localScale = new Vector3((closestValue + SecondClosestCircleRadius), (closestValue + SecondClosestCircleRadius), (closestValue + SecondClosestCircleRadius));
            objectBubble.transform.position = interactableObjects[(int)lowestDistances[0][1]].transform.position;
            objectBubble.transform.localScale = new Vector3(interactableObjects[(int)lowestDistances[0][1]].transform.localScale.x + bubbleOffset, interactableObjects[(int)lowestDistances[0][1]].transform.localScale.y + bubbleOffset, interactableObjects[(int)lowestDistances[0][1]].transform.localScale.z + bubbleOffset);
            PickupObject(interactableObjects[(int)lowestDistances[0][1]]);

        }
    }
}
