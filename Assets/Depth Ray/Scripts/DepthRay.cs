using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

public class DepthRay : MonoBehaviour {

    /* Depth Ray implementation by Kieran May
     * University of South Australia
     * 
     * TODO
	 * -Add physics to gameObjects
     * -Refactor Code
     * */

    
    public GameObject controllerRight;
    public GameObject controllerLeft;

    //private GameObject[] interactableObject;
    public List<GameObject> interactableObject;
    private SteamVR_TrackedObject trackedObj;
    private SteamVR_Controller.Device controller;
    private GameObject mirroredCube;
    private RaycastHit[] raycastObjects;

    public GameObject laserPrefab;
    private GameObject laser;
    private Transform laserTransform;
    private GameObject cubeAssister;
    private Vector3 hitPoint;
    private Vector3 hitPoint2D;
    private RaycastHit[] oldHits;

    internal bool objectSelected = false;

    public enum InteractionType { Selection, Manipulation_Movement, Manipulation_Full };
    public enum SelectionAssister { Hide_Closest_Only, Hide_All_But_Closest };

    
    public InteractionType interacionType;
    public SelectionAssister selectionType;

    public enum ControllerPicked { Left_Controller, Right_Controller };
    public ControllerPicked controllerPicked;

    public UnityEvent selectedObject; // Invoked when an object is selected

    public UnityEvent droppedObject; // Invoked when an object is dropped

    public UnityEvent hovered; // Invoked when an object is hovered by technique
    public UnityEvent unHovered; // Invoked when an object is no longer hovered by the technique

    private void ShowLaser(RaycastHit hit) {
        mirroredCube.SetActive(false);
        laser.SetActive(true);
        laserTransform.position = Vector3.Lerp(trackedObj.transform.position, hitPoint, .5f);
        //cubeAssister.transform.position = Vector3.Lerp(trackedObj.transform.position, hitPoint, .5f);
        laserTransform.LookAt(hitPoint);
        //laserTransform.localScale = new Vector3(laserTransform.localScale.x, laserTransform.localScale.y, distance*10);
        laserTransform.localScale = new Vector3(laserTransform.localScale.x, laserTransform.localScale.y, hit.distance);
        //print(distance);
    }
    private void ShowLaser() {
        laser.SetActive(true);
        mirroredCube.SetActive(true);


        //laser.transform.position = trackedObj.transform.position*2;
        //laser.transform.position = new Vector3(trackedObj.transform.position.x, trackedObj.transform.position.y, trackedObj.transform.position.z*1.25f);
        //laser.transform.position = Vector3.Lerp(trackedObj.transform.position, forward, 0.6f);
        //laserTransform.LookAt(forward);
        //laserTransform.localScale = new Vector3(laserTransform.localScale.x, laserTransform.localScale.y, 1f);
        //laserTransform.position = Vector3.Lerp(trackedObj.transform.position, forward, .5f);
        /*laserTransform.position = Vector3.Lerp(trackedObj.transform.position, forward, .5f);
        laserTransform.LookAt(trackedObj.transform.position);
        laserTransform.localScale = new Vector3(laserTransform.localScale.x, laserTransform.localScale.y, 10f);*/
    }

    private float extendDistance = 0f;
    private float cursorSpeed = 20f; // Decrease to make faster, Increase to make slower

    private void PadScrolling() {
        if (controller.GetAxis().y != 0) {
            //print(controller.GetAxis().y);
            //cursor.transform.position += new Vector3(0f, 0f, controller.GetAxis().y/20);
            extendDistance += controller.GetAxis().y / cursorSpeed;
            moveCubeAssister();
        }
    }

    private bool pickedUpObject = false; //ensure only 1 object is picked up at a time
    internal GameObject tempObjectStored;
    void PickupObject(GameObject obj) {
        if (trackedObj != null) {
            if (controller.GetTouchDown(SteamVR_Controller.ButtonMask.Trigger) && pickedUpObject == false) {
                if (interacionType == InteractionType.Manipulation_Movement || interacionType == InteractionType.Manipulation_Full) {
                    obj.transform.SetParent(trackedObj.transform);
                    tempObjectStored = obj; // Storing the object as an instance variable instead of using the obj parameter fixes glitch of it not properly resetting on TriggerUp
                    pickedUpObject = true;
                } else if (interacionType == InteractionType.Selection) {
                    tempObjectStored = obj;
                    objectSelected = true;
                    print("Selected object in pure selection mode:" + tempObjectStored.name);
                }
                selectedObject.Invoke();
            }
            if (controller.GetTouchUp(SteamVR_Controller.ButtonMask.Trigger) && pickedUpObject == true) {
                if (interacionType == InteractionType.Manipulation_Movement || interacionType == InteractionType.Manipulation_Full) {
                    tempObjectStored.transform.SetParent(null);
                    pickedUpObject = false;
                } else if (interacionType == InteractionType.Selection) {
                    objectSelected = false;
                }
                }
                droppedObject.Invoke();
        }
    }


    void moveCubeAssister() {
        //getControllerPosition();
        Vector3 mirroredPos = trackedObj.transform.position;
        Vector3 pos = trackedObj.transform.position;
        Vector3 controllerPos = trackedObj.transform.forward;
        float distance_formula_on_vector = Mathf.Sqrt(controllerPos.x * controllerPos.x + controllerPos.y * controllerPos.y + controllerPos.z * controllerPos.z);
        // Using formula to find a point which lies at distance on a 3D line from vector and direction
        if (extendDistance < 0) {
            extendDistance = 0;
        }
        pos.x = pos.x + (extendDistance / (distance_formula_on_vector)) * controllerPos.x;
        pos.y = pos.y + (extendDistance / (distance_formula_on_vector)) * controllerPos.y;
        pos.z = pos.z + (extendDistance / (distance_formula_on_vector)) * controllerPos.z;

        mirroredPos.x = mirroredPos.x + (100f / (distance_formula_on_vector)) * controllerPos.x;
        mirroredPos.y = mirroredPos.y + (100f / (distance_formula_on_vector)) * controllerPos.y;
        mirroredPos.z = mirroredPos.z + (100f / (distance_formula_on_vector)) * controllerPos.z;

        cubeAssister.transform.position = pos;
        cubeAssister.transform.rotation = trackedObj.transform.rotation;

        mirroredCube.transform.position = mirroredPos;
        mirroredCube.transform.rotation = trackedObj.transform.rotation;
    }

    public float thickness = 0.002f;
    float dist = 100f;

    private int ClosestObject() {
        int lowestValue = 0;
        float lowestDist = 0;
        for (int i = 0; i < raycastObjects.Length; i++) {
            float dist = Vector3.Distance(cubeAssister.transform.position, raycastObjects[i].transform.position) / 2f;
            if (i == 0) {
                lowestDist = dist;
                lowestValue = 0;
            } else {
                if (dist < lowestDist) {
                    lowestDist = dist;
                    lowestValue = i;
                }
            }
        }

            return lowestValue;
    }

    private void ResetAllMaterials() {
        if (oldHits != null) {
            foreach (RaycastHit hit in oldHits) {
                hit.transform.gameObject.GetComponent<Renderer>().material = defaultMat;
            }
        }
    }

    private bool Contains(GameObject obj, RaycastHit[] hits) {
        if (hits.Length >= 1) {
            foreach (RaycastHit hit in hits) {
                if (hit.transform.gameObject == obj) {
                    return true;
                }
            }
        }
        //print(interactableObject.Count);
        //obj.GetComponent<Renderer>().material = interactableObject.Find(d => d == obj).transform.GetComponent<Renderer>().material;
        obj.GetComponent<Renderer>().material = defaultMat;
        return false;
    }

    void Awake() {
        mirroredCube = this.transform.Find("Mirrored Cube").gameObject;
        cubeAssister = this.transform.Find("Cube Assister").gameObject;
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
        GameObject[] interactObjects = GameObject.FindGameObjectsWithTag("InteractableObjects");
        interactableObject = new List<GameObject>(interactObjects);
        laser = Instantiate(laserPrefab);
        laserTransform = laser.transform;
        cubeAssister.transform.position = trackedObj.transform.position;
        /*GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();
        int count = 0;
        for (int i=0; i<allObjects.Length; i++) {
            if (allObjects[i].layer != LayerMask.NameToLayer("Ignore Raycast")) {
                raycastObjects[count] = allObjects[i];
            }
            count++;
        }*/
    }
    float distance = 0f;
    Vector3 forward;
    private GameObject currentClosestObject;
    public Material outlineMaterial;
    public Material defaultMat;
    private Material currentClosestObjectMaterial;
    void Update() {
        controller = SteamVR_Controller.Input((int)trackedObj.index);
        moveCubeAssister();
        PadScrolling();
        forward = trackedObj.transform.TransformDirection(Vector3.forward) * 10;
        ShowLaser();
        RaycastHit[] hits = Physics.RaycastAll(trackedObj.transform.position, forward, 100.0F);
        if (hits.Length >= 1) {
            raycastObjects = hits;
            int closestVal = ClosestObject();
            if (raycastObjects[closestVal].transform.name != "Mirrored Cube") {
                if (selectionType == SelectionAssister.Hide_All_But_Closest) {
                    if (currentClosestObject != raycastObjects[closestVal].transform.gameObject) {
                        currentClosestObject = raycastObjects[closestVal].transform.gameObject;
                        PickupObject(raycastObjects[closestVal].transform.gameObject);
                    }
                } else if (selectionType == SelectionAssister.Hide_Closest_Only) {
                    if (currentClosestObject != raycastObjects[closestVal].transform.gameObject) {
                        //print("new closest object");
                        if (currentClosestObject != null) {
                            if (currentClosestObjectMaterial != null) {
                                currentClosestObject.transform.GetComponent<Renderer>().material = defaultMat;
                            }
                            currentClosestObjectMaterial = currentClosestObject.transform.GetComponent<Renderer>().material;
                        }
                        currentClosestObject = raycastObjects[closestVal].transform.gameObject;
                    } else {
                        currentClosestObject.transform.GetComponent<Renderer>().material = outlineMaterial;
                    }
                    PickupObject(raycastObjects[closestVal].transform.gameObject);
                }
            } else {
                if (currentClosestObject != null) {
                    currentClosestObject.transform.GetComponent<Renderer>().material = defaultMat;
                }
            }
        }

        //print("hit length:" + hits.Length);
        for (int i = 0; i < hits.Length; i++) {
            RaycastHit hit = hits[i];
            if (selectionType == SelectionAssister.Hide_All_But_Closest) {
                if (oldHits != null) {
                    if (hit.transform.gameObject != mirroredCube && hits.Length != 1) {
                        if (Contains(oldHits[i].transform.gameObject, hits) == true) {
                            if (hit.transform.gameObject != currentClosestObject) {
                                hit.transform.gameObject.transform.GetComponent<Renderer>().material = outlineMaterial;
                                print(hits[0].transform.gameObject.name);
                            }
                        }
                    } else if (hits.Length == 1) {
                        ResetAllMaterials();
                    }
                }
            } else if (selectionType == SelectionAssister.Hide_Closest_Only) {

            }

                //print("hit:" + hit.transform.name + " index:"+i);
            distance = hit.distance;
            hitPoint = hit.point;
            //hit.transform.gameObject.GetComponent<Renderer>().material.color = new Color(1, 1, 1, 0.5f);
            ShowLaser(hit);
        }
        if (hits.Length > 1) {
            oldHits = hits;
        }
    }
}
