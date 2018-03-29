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
     * -Fix controller pad scrolling cursor in/out
     * -Make bubble cursor compatible with all type of gameobject shapes
     * -Refactor code
     * */

    private GameObject[] interactableObjects; // In-game objects
    public GameObject cursor;
    private float startRadius = 0f;
    public GameObject radiusBubble;
    public GameObject objectBubble;

    private SteamVR_TrackedObject trackedObj;
    private SteamVR_Controller.Device controller;


    public GameObject controllerRight;
    public GameObject controllerLeft;
    public GameObject cameraHead;
    public bool controllerRightPicked;
    public bool controllerLeftPicked;
    public bool cameraHeadPicked;

    private readonly float bubbleOffset = 0.6f;

    void Awake() {
        trackedObj = controllerRight.GetComponent<SteamVR_TrackedObject>();
    }

    // Use this for initialization
    void Start () {
        interactableObjects = GameObject.FindGameObjectsWithTag("InteractableObjects");
        startRadius = cursor.GetComponent<SphereCollider>().radius;

        /*cameraRig = GameObject.Find("[CameraRig]");
        controllerRight = cameraRig.transform.Find("Controller (right)").gameObject;
        controllerLeft = GameObject.Find("Controller (left)");
        cameraHead = GameObject.Find("Camera (head)");*/
        SetParent();
    }

    void SetParent() {
        if (controllerRightPicked == true) {
            cursor.transform.SetParent(controllerRight.transform);
        } else if (controllerLeftPicked == true) {
            cursor.transform.SetParent(controllerLeft.transform);
        } else if (cameraHeadPicked == true) {
            cursor.transform.SetParent(cameraHead.transform);
        }
    }

    /// <summary>
    /// Loops through interactable gameObjects within the scene & stores them in a 2D array.
    ///     -2D array is used to keep store gameObjects distance & also keep track of their index. eg [0][0] returns objects distance [0][1] returns objects index.
    /// Using Linq 2D array is sorted based on closest distances
    /// </summary>
    /// <returns>2D Array which contains order of objects with the closest distances & their allocated index</returns>
	private float[][] ClosestObject() {
		float[] lowestDists = new float[4];
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
		return arraytest;
	}

    /// <summary>
    /// Allows player to reposition the bubble cursor fowards & backwards.
    /// -Currently needs alot of work, only modifies the Z axis.
    /// </summary>

    private void PadScrolling() {
        if (controller.GetAxis().y != 0) {
            //print(controller.GetAxis().y);
            cursor.transform.position += new Vector3(0f, 0f, controller.GetAxis().y/20);
        }
    }

    // Update is called once per frame
    void Update () {
        controller = SteamVR_Controller.Input((int)trackedObj.index);
        PadScrolling();

        float[][] lowestDistances = ClosestObject();
        //float ClosestCircleRadius = lowestDistances[0][0] + interactableObjects[(int)lowestDistances[0][1]].GetComponent<SphereCollider>().radius;
        //float SecondClosestCircleRadius = lowestDistances[1][0] - interactableObjects[(int)lowestDistances[1][1]].GetComponent<SphereCollider>().radius;
        float ClosestCircleRadius = 0f;
        float SecondClosestCircleRadius = 0f;


        if (interactableObjects[(int)lowestDistances[0][1]].GetComponent<Collider>().GetType() == typeof(SphereCollider)) {
            ClosestCircleRadius = lowestDistances[0][0] + interactableObjects[(int)lowestDistances[0][1]].GetComponent<SphereCollider>().radius;
        }
        if (interactableObjects[(int)lowestDistances[1][1]].GetComponent<Collider>().GetType() == typeof(SphereCollider)) {
            SecondClosestCircleRadius = lowestDistances[1][0] - interactableObjects[(int)lowestDistances[1][1]].GetComponent<SphereCollider>().radius;
        }

        if (interactableObjects[(int)lowestDistances[0][1]].GetComponent<Collider>().GetType() == typeof(CapsuleCollider)) {
            ClosestCircleRadius = lowestDistances[0][0] + interactableObjects[(int)lowestDistances[0][1]].GetComponent<CapsuleCollider>().radius;
        }
        if (interactableObjects[(int)lowestDistances[1][1]].GetComponent<Collider>().GetType() == typeof(CapsuleCollider)) {
            SecondClosestCircleRadius = lowestDistances[1][0] - interactableObjects[(int)lowestDistances[1][1]].GetComponent<CapsuleCollider>().radius;
        }

        if (interactableObjects[(int)lowestDistances[0][1]].GetComponent<Collider>().GetType() == typeof(BoxCollider)) {
            ClosestCircleRadius = lowestDistances[0][0] + interactableObjects[(int)lowestDistances[0][1]].GetComponent<BoxCollider>().size.x;
        }
        if (interactableObjects[(int)lowestDistances[1][1]].GetComponent<Collider>().GetType() == typeof(BoxCollider)) {
            SecondClosestCircleRadius = lowestDistances[1][0] - interactableObjects[(int)lowestDistances[1][1]].GetComponent<BoxCollider>().size.x;
        }

        float closestValue = Mathf.Min(ClosestCircleRadius, SecondClosestCircleRadius);
        //print("updating");
        //print("FIRST closest radius:" + ClosestCircleRadius*2 + " | closest value:" + closestValue);
        //print("SECOND closest radius:" + SecondClosestCircleRadius* 2 + " | closest value:" + closestValue);
        if (ClosestCircleRadius * 2 < SecondClosestCircleRadius * 2) {
            cursor.GetComponent<SphereCollider>().radius = (closestValue + ClosestCircleRadius);
            radiusBubble.transform.localScale = new Vector3((closestValue + ClosestCircleRadius)*2, (closestValue + ClosestCircleRadius)*2, (closestValue + ClosestCircleRadius)*2);
            //print("TARGET:"+lowestDistances[0][1]);
            objectBubble.transform.localScale = new Vector3(0f, 0f, 0f);
           
        } else {
            cursor.GetComponent<SphereCollider>().radius = (closestValue + SecondClosestCircleRadius);
            radiusBubble.transform.localScale = new Vector3((closestValue + SecondClosestCircleRadius)*2, (closestValue + SecondClosestCircleRadius)*2, (closestValue + SecondClosestCircleRadius)*2);
            //print("TARGET:" + lowestDistances[1][1]);
            objectBubble.transform.position = interactableObjects[(int)lowestDistances[0][1]].transform.position;
            objectBubble.transform.localScale = new Vector3(interactableObjects[(int)lowestDistances[0][1]].transform.localScale.x + bubbleOffset, interactableObjects[(int)lowestDistances[0][1]].transform.localScale.y + bubbleOffset, interactableObjects[(int)lowestDistances[0][1]].transform.localScale.z + bubbleOffset);
        }
    }
}
