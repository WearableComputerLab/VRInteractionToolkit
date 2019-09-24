using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class ImagePlanePointing : MonoBehaviour {

    /* Image Plane Pointing implementation by Kieran May
    * University of South Australia
    * 
    * -Already implemented the selection aspect of the Flexible Pointer
    * ie point a laser at an object and generate a 2D clone on a mini UI of the 3D selected object.
    * 
    * -Haven't yet started on the object manipulation aspect of the Flexible Pointer
    * ie move/rotate the 2D object clone & effect the 3D object in real-time
    * 
    * TODO
    * -Implement as prefab
    * -Implment manipulation/interaction aspect of Image Plane Pointing
    * */
    /*
            private SteamVR_TrackedObject trackedObj;
            private SteamVR_Controller.Device controller;

            private bool currentlyModifying = false;
            public GameObject laserPrefab;
            private GameObject laser;
            private Transform laserTransform;
            private Vector3 hitPoint;
            public GameObject panel;
            public GameObject cameraHead;
            public GameObject camera;

            private void ShowLaser(RaycastHit hit) {
                laser.SetActive(true);
                laserTransform.position = Vector3.Lerp(trackedObj.transform.position, hitPoint, .5f);
                laserTransform.LookAt(hitPoint);
                laserTransform.localScale = new Vector3(laserTransform.localScale.x, laserTransform.localScale.y, hit.distance);
            }
            private GameObject[] allSceneObjects;

            void isVisible() {
                foreach (GameObject obj in allSceneObjects) {
                    if (obj.GetComponent<Renderer>() != null && obj.GetComponent<Renderer>().isVisible) {
                        print("obj:" + obj.name + " is visible..");
                    } else {
                        print("obj:" + obj.name + " NOT VISIBLE");
                    }
                }
            }

            void Awake() {
                trackedObj = GetComponent<SteamVR_TrackedObject>();
            }

            private GameObject[] interactableObjects; // In-game objects

            void Start() {
                laser = Instantiate(laserPrefab);
                laserTransform = laser.transform;
                allSceneObjects = SceneManager.GetActiveScene().GetRootGameObjects();
                interactableObjects = GameObject.FindGameObjectsWithTag("InteractableObjects");
            }

            //Y -0.4 DOWN | +0.4 UP
            //X +0.4 RIGHT | -0.4 LEFT
            private void OnTriggerStay(Collider col) {
                //print("Colliding with " + col.name);
                if (pickedObj2D != null) {
                    if (col.name == pickedObj2D.name) {
                        if (controller.GetTouch(SteamVR_Controller.ButtonMask.Trigger)) {
                            //Debug.Log("You have collided with " + col.name + " while holding down Touch");
                            col.gameObject.transform.SetParent(this.gameObject.transform);
                            //col.gameObject.transform.position += new Vector3(0f, this.transform.position.y / 20f, 0f);
                            //print("x:"+this.transform.position.x + " | "+ "y:" + this.transform.position.y);
                        }
                        if (controller.GetTouchUp(SteamVR_Controller.ButtonMask.Trigger)) {
                            //Debug.Log("You have released Touch while colliding with " + col.name);
                            pickedObj2D.gameObject.transform.SetParent(panel.transform);
                            float newX = pickedObj2D.transform.localPosition.x * 10;
                            float newY = pickedObj2D.transform.localPosition.y * 10;
                            print("Y2:" + newY + " | X2:" + newX);
                            pickedObj.transform.position = new Vector3(pickedObj.transform.position.x + newX, pickedObj.transform.position.y + newY, pickedObj.transform.position.z);
                            pickedObj.transform.rotation = new Quaternion(pickedObj2D.transform.localRotation.x, pickedObj2D.transform.localRotation.y, pickedObj2D.transform.localRotation.z, pickedObj2D.transform.localRotation.w);
                            //pickedObj2D.transform.position = new Vector3(0f, 0f, 0f);
                        }
                    }
                }
            }
            //0.1 Y right
            //0.1 
            private void OnTriggerExit(Collider col) {
                if (pickedObj2D != null) {
                    if (col.name == pickedObj2D.name) {
                        //print("Y:" + pickedObj2D.transform.position.y + " | X:" + pickedObj2D.transform.position.x);
                        //print("Y2:" + pickedObj2D.transform.localPosition.y + " | X2:" + pickedObj2D.transform.localPosition.x);
                    }
                }
            }

            private float[][] ClosestObject() {
                float lowestDist = 0;
                int lowestValue = 0;
                float[][] allDists = new float[interactableObjects.Length][];
                for (int i = 0; i < interactableObjects.Length; i++) {
                    allDists[i] = new float[2];
                }
                for (int i = 0; i < interactableObjects.Length; i++) {
                    //float dist = Vector3.Distance(new Vector3(trackedObj.transform.position.x, trackedObj.transform.position.y, 0), new Vector3(interactableObjects[i].transform.position.x, interactableObjects[i].transform.position.y, interactableObjects[i].transform.position.z));
                    float dist = Vector2.Distance(trackedObj.transform.position, interactableObjects[i].transform.position);
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
                }
                float[][] arraytest = allDists.OrderBy(row => row[0]).ToArray();
                return arraytest;
            }

            private GameObject pickedObj2D = null;
            private GameObject pickedObj = null;
            void generate2DObjects(GameObject pickedObject) {
                if (pickedObject.transform.tag == "PickableObject" && currentlyModifying == false) {
                    panel.SetActive(true);
                    currentlyModifying = true;
                    pickedObj = pickedObject;
                    pickedObj2D = Instantiate(pickedObject, new Vector3(0f, 0f, 0f), Quaternion.identity) as GameObject;
                    pickedObj2D.transform.SetParent(panel.transform,false);
                    pickedObj2D.gameObject.AddComponent<Rigidbody>();
                    pickedObj2D.GetComponent<Collider>().attachedRigidbody.isKinematic = true;
                    pickedObj2D.transform.localScale = new Vector3(0.25f, 0.25f, 0f);
                    pickedObj2D.transform.localRotation = Quaternion.identity;
                    pickedObj2D.transform.localPosition = Vector3.zero;
                }
            }

            void printArray(float[][] arraytest) {
                for (int i = 0; i < arraytest.Length; i++) {
                    print(i+1+" | "+ arraytest[i][0] + " | name: " + interactableObjects[(int)arraytest[i][1]].name);
                }
            }

            void Update() {
                controller = SteamVR_Controller.Input((int)trackedObj.index);
                //move2DObject();
                //isVisible();
                //float[][] getClosestObject = ClosestObject();
                //printArray(getClosestObject);
                Ray ray = Camera.main.ScreenPointToRay(trackedObj.transform.position);
                    currentlyModifying = false;
                    panel.SetActive(false);
                    if (pickedObj2D != null) {
                        Destroy(pickedObj2D);
                    }
                    RaycastHit hit;
                    if (Physics.Raycast(trackedObj.transform.position, transform.forward, out hit, 100)) {
                        //print("hit:" + hit.transform.name);
                        generate2DObjects(hit.transform.gameObject);
                        hitPoint = hit.point;
                        ShowLaser(hit);
                    }
            }
            */
    }
       