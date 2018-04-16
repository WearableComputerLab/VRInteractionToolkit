using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class BubbleCursor3D : MonoBehaviour {
    
    /* 3D Bubble Cursor implementation by Kieran May
     * University of South Australia
     * 
     * TODO
     * -IMPORTANT: Found out scaling issue: fix is (scale/radius) ex (1.0 scale / 0.5f radius) = 2f (use this value to divide by the overall dist)
     * */

    private GameObject[] interactableObjects; // In-game objects
    public GameObject cursor;
    private float minRadius = 1f;
    public GameObject radiusBubble;
    public GameObject objectBubble;

    private SteamVR_TrackedObject trackedObj;
    private SteamVR_Controller.Device controller;
    public BubbleSelection bubbleSelection;

    public GameObject controllerRight;
    public GameObject controllerLeft;
    public GameObject cameraHead;
    public bool controllerRightPicked;
    public bool controllerLeftPicked;
    public bool cameraHeadPicked;

    private readonly float bubbleOffset = 0.6f;

    public SteamVR_TrackedObject getTrackedObject() {
        if (controllerRightPicked == true) {
            trackedObj = controllerRight.GetComponent<SteamVR_TrackedObject>();
        } else if (controllerLeftPicked == true) {
            trackedObj = controllerLeft.GetComponent<SteamVR_TrackedObject>();
        } else if (cameraHeadPicked == true) {
            trackedObj = cameraHead.GetComponent<SteamVR_TrackedObject>();
        }
        return trackedObj;
    }

    void Awake() {
        trackedObj = getTrackedObject();
    }

    // Use this for initialization
    void Start () {
        interactableObjects = GameObject.FindGameObjectsWithTag("InteractableObjects");
        //bubbleSelection = radiusBubble.GetComponent<BubbleSelection>();
        //minRadius = cursor.GetComponent<SphereCollider>().radius;
        getControllerPosition();
        extendDistance = Vector3.Distance(controllerPos, cursor.transform.position);
        /*cameraRig = GameObject.Find("[CameraRig]");
        controllerRight = cameraRig.transform.Find("Controller (right)").gameObject;
        controllerLeft = GameObject.Find("Controller (left)");
        cameraHead = GameObject.Find("Camera (head)");*/
        SetParent();
        bubbleSelection.trackedObj = getTrackedObject();
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
    private float extendDistance = 0f;
    private float cursorSpeed = 20f; // Decrease to make faster, Increase to make slower

    private void PadScrolling() {
        if (controller.GetAxis().y != 0) {
            //print(controller.GetAxis().y);
            //cursor.transform.position += new Vector3(0f, 0f, controller.GetAxis().y/20);
            extendDistance += controller.GetAxis().y / cursorSpeed;
            moveCursor();
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

    private Vector3 controllerPos = new Vector3(0, 0, 0);

    void getControllerPosition() {
        // Using the origin and the forward vector of the remote the extended positon of the remote can be calculated
        if (controllerRightPicked == true) {
            controllerPos = controllerRight.transform.forward;
        } else if (controllerLeftPicked == true) {
            controllerPos = controllerLeft.transform.forward;
        } else if (cameraHeadPicked == true) {
            controllerPos = cameraHead.transform.forward;
        }
    }

    void moveCursor() {
        getControllerPosition();
        Vector3 pose = new Vector3(0, 0, 0); ;
        if (controllerRightPicked == true) {
            pose = controllerRight.transform.position;
        } else if (controllerLeftPicked == true) {
            pose = controllerLeft.transform.position;
        } else if (cameraHeadPicked == true) {
            pose = cameraHead.transform.position;
        }

        float distance_formula_on_vector = Mathf.Sqrt(controllerPos.x * controllerPos.x + controllerPos.y * controllerPos.y + controllerPos.z * controllerPos.z);
        // Using formula to find a point which lies at distance on a 3D line from vector and direction
        pose.x = pose.x + (extendDistance / (distance_formula_on_vector)) * controllerPos.x;
        pose.y = pose.y + (extendDistance / (distance_formula_on_vector)) * controllerPos.y;
        pose.z = pose.z + (extendDistance / (distance_formula_on_vector)) * controllerPos.z;

        cursor.transform.position = pose;
        if (controllerRightPicked == true) {
            cursor.transform.rotation = controllerRight.transform.rotation;
        } else if (controllerLeftPicked == true) {
            cursor.transform.rotation = controllerLeft.transform.rotation;
        } else if (cameraHeadPicked == true) {
            cursor.transform.rotation = cameraHead.transform.rotation;
        }
    }

    // Update is called once per frame
    void Update() {
        /*print(bubbleSelection.inBubbleSelection);
        if (bubbleSelection.inBubbleSelection == true) {
            cursor.SetActive(false);
        }
        else if (bubbleSelection.inBubbleSelection == false) {*/
            if (trackedObj != null) {
                controller = SteamVR_Controller.Input((int)trackedObj.index);
                PadScrolling();
            }

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
                if (cursor.GetComponent<SphereCollider>().radius < minRadius) {
                    cursor.GetComponent<SphereCollider>().radius = minRadius;
                }
                radiusBubble.transform.localScale = new Vector3((closestValue + ClosestCircleRadius) * 2, (closestValue + ClosestCircleRadius) * 2, (closestValue + ClosestCircleRadius) * 2);
                if (radiusBubble.transform.localScale.x < minRadius * 2) {
                    radiusBubble.transform.localScale = new Vector3(minRadius * 2, minRadius * 2, minRadius * 2);
                }
                //print("TARGET:"+lowestDistances[0][1]);
                objectBubble.transform.localScale = new Vector3(0f, 0f, 0f);
                //PickupObject(interactableObjects[(int)lowestDistances[0][1]]);
                //bubbleSelection.PickupObject(controller, trackedObj, bubbleSelection.getSelectableObjects());
                bubbleSelection.enableMenu(controller, trackedObj, bubbleSelection.getSelectableObjects());
                bubbleSelection.clearList();
            } else {
                cursor.GetComponent<SphereCollider>().radius = (closestValue + SecondClosestCircleRadius);
                if (cursor.GetComponent<SphereCollider>().radius < minRadius) {
                    cursor.GetComponent<SphereCollider>().radius = minRadius;
                }
                radiusBubble.transform.localScale = new Vector3((closestValue + SecondClosestCircleRadius) * 2, (closestValue + SecondClosestCircleRadius) * 2, (closestValue + SecondClosestCircleRadius) * 2);
                if (radiusBubble.transform.localScale.x < minRadius * 2) {
                    radiusBubble.transform.localScale = new Vector3(minRadius * 2, minRadius * 2, minRadius * 2);
                }
                //print("TARGET:" + lowestDistances[1][1]);
                objectBubble.transform.position = interactableObjects[(int)lowestDistances[0][1]].transform.position;
                objectBubble.transform.localScale = new Vector3(interactableObjects[(int)lowestDistances[0][1]].transform.localScale.x + bubbleOffset, interactableObjects[(int)lowestDistances[0][1]].transform.localScale.y + bubbleOffset, interactableObjects[(int)lowestDistances[0][1]].transform.localScale.z + bubbleOffset);
                //PickupObject(interactableObjects[(int)lowestDistances[0][1]]);
                //bubbleSelection.PickupObject(controller, trackedObj, bubbleSelection.getSelectableObjects());
                bubbleSelection.enableMenu(controller, trackedObj, bubbleSelection.getSelectableObjects());
                bubbleSelection.clearList();
            }
        }
}
