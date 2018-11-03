using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SquadMenu : MonoBehaviour {
	
	/* SQUAD implementation by Kieran May
    * University of South Australia
    * 
    * -SQUAD is an extension of Sphere-Casting which places selected objects into a QUAD menu for more precise selection
    * 
    * TODO
    * -Refactor code
    */

    public GameObject panel;
    public GameObject prefabText;
    public Material quadrantMaterial;
    public Material outlineMaterial;
    public Material triangleMaterial;
    public GameObject cameraHead;
    private bool quadrantPicked = false;
    private Transform[] TriangleQuadrant = new Transform[4];

    public List<GameObject> selectableObjects = new List<GameObject>();

    private void destroyChildGameObjects() {
        for(int i = 0; i < 4; i++) {
            foreach(Transform child in TriangleQuadrant[i].transform) {
                GameObject.Destroy(child.gameObject);
            }
        }
        foreach (Transform child in panel.transform) {
            if (child.gameObject.name != "CreateTrianglesSprite" && !child.gameObject.name.Contains("TriangleQuad")) {
                GameObject.Destroy(child.gameObject);
            } else if (child.gameObject.name.Contains("TriangleQuad") && !child.gameObject.name.Contains("Placeholder")) {
                child.gameObject.transform.GetComponent<Renderer>().material = triangleMaterial;
            }
        }

    }

    public bool isActive() {
        return panel.activeInHierarchy;
    }

    public bool quadrantIsPicked() {
        return quadrantPicked;
    }

    public void hideAllGameObjects() {
        GameObject[] allObjects = SceneManager.GetActiveScene().GetRootGameObjects();
        foreach (GameObject obj in allObjects) {
            if (obj.name != "CreateTriangles" && obj.name != "[CameraRig]" && obj.name != "[SteamVR]") {
                print("Hidden Object:" + obj.name);
                obj.SetActive(false);
            }
        }
    }

    public void showAllGameObjects() {
        GameObject[] allObjects = SceneManager.GetActiveScene().GetRootGameObjects();
        foreach (GameObject obj in allObjects) {
            if (obj.name != "CreateTriangles" && obj.name != "[CameraRig]" && obj.name != "[SteamVR]") {
                print("Hidden Object:" + obj.name);
                obj.SetActive(true);
            }
        }
    }

    public void disableSQUAD() {
        clearList();
        //showAllGameObjects();
        panel.transform.SetParent(cameraHead.transform);
        panel.SetActive(false);
        SphereCasting.inMenu = false;
        quadrantPicked = false;
        pickedObject = null;
        destroyChildGameObjects();
        print("Squad disabled..");
    }

	/*public void initializeTriangles() {
		panel.transform.SetParent (cameraHead.transform);
		panel.SetActive (false);
		panel.transform.localEulerAngles = new Vector3 (0f, 0f, 0f);
		panel.transform.localPosition = new Vector3 (0f, 0f, 1f);
	}*/

    private bool pickedUpObject = false; //ensure only 1 object is picked up at a time
    private GameObject pickedObject;
    private GameObject lastPickedObject;
    private Material oldPickedObjectMaterial;

    public void selectObject(SteamVR_Controller.Device controller, GameObject obj) {
        //print("picked object:" + pickedObject);
        quadrantPicked = true;
        if(controller.GetPressDown(SteamVR_Controller.ButtonMask.Trigger) && quadrantIsPicked() == true) {
        //if (controller.GetPressDown(SteamVR_Controller.ButtonMask.Trigger) && quadrantIsPicked() == true && pickedObject == null && obj.transform.parent == panel.transform && !obj.name.Contains("TriangleQuad")) {
            //disableSQUAD();
            //pickedObject = obj;
            //string objName = obj.name.Substring(0, obj.name.Length-7);
            string objName = obj.name;
            //print("obj picked:" + objName);
            pickedObject = GameObject.Find(objName);
            lastPickedObject = pickedObject;
            print("Final picked object:" + objName);
            oldPickedObjectMaterial = pickedObject.transform.GetComponent<Renderer>().material;
            pickedObject.transform.GetComponent<Renderer>().material = outlineMaterial;
            disableSQUAD();
        }
    }

    private void clearObjects() {

    }

    public void refineQuad(SteamVR_Controller.Device controller, GameObject obj) {
        int val = 0;
        int count = 0;
        if (obj.name == "TriangleQuad North") {
            val = 0;
        } else if(obj.name == "TriangleQuad South") {
            val = 1;
        } else if(obj.name == "TriangleQuad East") {
            val = 2;
        } else if(obj.name == "TriangleQuad West") {
            val = 3;
        }
        List<GameObject> quadObjs = new List<GameObject>();
        foreach (Transform child in TriangleQuadrant[val]) {
            print(child.name+" in: " + TriangleQuadrant[val]);
            quadObjs.Add(child.gameObject);
            count++;
        }
        destroyChildGameObjects();
        print("----- REFINING QUAD -----");
        print(count);
        if(count == 1) {
            selectObject(controller, quadObjs[0]);
        } else {
            generate2DObjects(quadObjs);
        }
    }

    private void setQuadrants() { //NSEW
        foreach (Transform child in panel.transform) {
            print("quad:"+child.transform.name);
            if (child.name == "TriangleQuad North Placeholder") {
                TriangleQuadrant[0] = child;
            } else if(child.name == "TriangleQuad South Placeholder") {
                TriangleQuadrant[1] = child;
            } else if(child.name == "TriangleQuad East Placeholder") {
                TriangleQuadrant[2] = child;
            } else if (child.name == "TriangleQuad West Placeholder") {
                TriangleQuadrant[3] = child;
            }
        }
    }


    public void enableSQUAD(SteamVR_Controller.Device controller, SteamVR_TrackedObject trackedObj, List<GameObject> obj) {
        if (trackedObj != null) {
            if (controller.GetPressDown(SteamVR_Controller.ButtonMask.Trigger) && pickedUpObject == false) {
                print("EnableSquad() called");
                SphereCasting.inMenu = true;
                panel.SetActive(true);
                setQuadrants();
                //hideAllGameObjects();
                generate2DObjects(obj);
                if (lastPickedObject != null) {
                    lastPickedObject.transform.GetComponent<Renderer>().material = oldPickedObjectMaterial;
                }
            }
        }
    }

    private GameObject lastQuad;
    private Material oldMaterial;
    public void hoverQuad(SteamVR_Controller.Device controller, GameObject obj) {
        //print("obj contians:"+obj.name.Contains("TriangleQuad"));
        if(obj.name.Contains("TriangleQuad") && isActive() == true) {
            if(lastQuad == null) {
                oldMaterial = obj.transform.GetComponent<Renderer>().material;
                lastQuad = obj;
                obj.transform.GetComponent<Renderer>().material = quadrantMaterial;
            } else {
                if (lastQuad != obj) {
                    lastQuad.transform.GetComponent<Renderer>().material = oldMaterial;
                    obj.transform.GetComponent<Renderer>().material = quadrantMaterial;
                    lastQuad = obj;
                }
                if(controller.GetPressDown(SteamVR_Controller.ButtonMask.Trigger)) {
                    print("Quad selected:" + obj.name);
                    refineQuad(controller, obj);
                }
            }
        }
    }


    public void selectQuad(SteamVR_Controller.Device controller, GameObject obj) {
        if (controller.GetPressDown(SteamVR_Controller.ButtonMask.Trigger)) {
            if (obj.name.Contains("TriangleQuad") && isActive() == true && quadrantPicked == false) {
                Renderer rend = obj.transform.GetComponent<Renderer>();
                //rend.material.color = Color.blue;
                rend.material = quadrantMaterial;
                quadrantPicked = true;
                //obj.transform.GetComponent<Renderer>().material.color = Color.clear;
            }
        }
    }

    //Stores 36 objects

    private GameObject pickedObjText = null;
    private GameObject pickedObj2D = null;
    private GameObject pickedObj = null;
    private int[] imageSlots;
    private int[,] locChanges = new int[,] { {1,2}, { 2, 3 } };
    private float[,] left = new float[,] { { -0.35f, 0.2f },
                                              { -0.4f, 0.1f }, { -0.3f, 0.1f },
                                              { -0.4f, 0f }, { -0.3f, 0f }, {-0.2f, 0f},
                                              { -0.4f, -0.1f }, {-0.3f, -0.1f},
                                              { -0.35f, -0.2f }};

    private float[,] right = new float[,] { { 0.35f, 0.2f },
                                              { 0.4f, 0.1f }, { 0.3f, 0.1f },
                                              { 0.4f, 0f }, { 0.3f, 0f }, {0.2f, 0f},
                                              { 0.4f, -0.1f }, {0.3f, -0.1f},
                                              { 0.35f, -0.2f }};


    private float[,] up = new float[,] { { -0.1f, 0.4f }, { 0f, 0.4f }, { 0.1f, 0.4f },
                                         { -0.1f, 0.3f }, { 0f, 0.3f }, { 0.1f, 0.3f },
                                         { -0.1f, 0.2f }, { 0f, 0.2f }, { 0.1f, 0.2f }};


    private float[,] down = new float[,] { { -0.1f, -0.4f }, { 0f, -0.4f }, { 0.1f, -0.4f },
                                         { -0.1f, -0.3f }, { 0f, -0.3f }, { 0.1f, -0.3f },
                                         { -0.1f, -0.2f }, { 0f, -0.2f }, { 0.1f, -0.2f }};

	private float scaleAmount = 10f;
    void generate2DObjects(List<GameObject> pickedObject) {
        imageSlots = new int[9];
        panel.transform.SetParent(null);
        //print("Amount of objects selected:" + pickedObject.Count);
        for (int i = 0; i < pickedObject.Count; i++) {
            //print("object:" + pickedObject[i].name + " | count:" + (i+1));
            pickedObj = pickedObject[i];
            int stage = 1;
            if ((i + 1) % 1 == 0) {
                stage = 1;
            } if ((i + 1) % 2 == 0) {
                stage = 2;
            } if ((i + 1) % 3 == 0) {
                stage = 3;
            } if ((i + 1) % 4 == 0) {
                stage = 4;
            }
            
            //print("object:" + pickedObject[i].name + " | count:" + (i + 1) + " | stage:"+stage);
            pickedObj2D = Instantiate(pickedObject[i], new Vector3(0f, 0f, 0f), Quaternion.identity) as GameObject;
            pickedObj2D.name = pickedObject[i].name;
			//pickedObj2D.layer = 99;
            pickedObj2D.transform.SetParent(panel.transform, false);
			//pickedObj2D.transform.localScale = new Vector3(pickedObject[i].transform.lossyScale.x / scaleAmount, pickedObject[i].transform.lossyScale.y / scaleAmount, pickedObject[i].transform.lossyScale.z / scaleAmount);
			print (pickedObject[i].transform.lossyScale.x + " | " + pickedObject[i].transform.name);
            //pickedObj2D.transform.localScale = new Vector3(0.0625f, 0.0625f, 0f);
            pickedObj2D.transform.localRotation = Quaternion.identity;
            print("New object generated:" + pickedObj2D.name);
            prefabText.GetComponent<TextMesh>().text = pickedObj.gameObject.name;
            pickedObjText = Instantiate(prefabText, new Vector3(0f, 0f, 0f), Quaternion.identity) as GameObject;
            pickedObjText.transform.SetParent(pickedObj2D.transform, false);
			if (pickedObj2D.GetComponent<Rigidbody> () == null) {
				pickedObj2D.gameObject.AddComponent<Rigidbody> ();
			}
			pickedObj2D.GetComponent<Rigidbody>().isKinematic = true;
			if (pickedObj2D.GetComponent<Collider> () != null) {
				Destroy (pickedObj2D.GetComponent<Collider> ());
			}
            pickedObjText.GetComponent<TextMesh>().fontSize = 250;
            //pickedObjText.transform.localScale = new Vector3(0f, -0.7f, 0f);
            //pickedObjText.gameObject.AddComponent<Rigidbody>();
            //pickedObjText.GetComponent<Collider>().attachedRigidbody.isKinematic = true;
            //pickedObj2D.transform.localScale = new Vector3(0.01f, 0.01f, 0f);
            pickedObjText.transform.localRotation = Quaternion.identity;

            int pos = 0;
            float posX = 0;
            float posY = 0;
            if (stage == 1) {
                imageSlots[0]++;
                pos = imageSlots[0]-1;
                posX = left[pos, 0];
                posY = left[pos, 1];
                pickedObj2D.transform.localPosition = new Vector3(posX, posY, -0.00001f);
                //pickedObjText.transform.localPosition = new Vector3(posX-0.01f, posY - 0.04f, -0.00001f);
                pickedObjText.transform.localPosition = new Vector3(0f, -0.6f, 0f);
                pickedObj2D.transform.SetParent(TriangleQuadrant[3], true);
            } else if (stage == 2) {
                imageSlots[1]++;
                pos = imageSlots[1]-1;
                posX = right[pos, 0];
                posY = right[pos, 1];
                pickedObj2D.transform.localPosition = new Vector3(posX, posY, -0.00001f);
                pickedObjText.transform.localPosition = new Vector3(0f, -0.6f, 0f);
                pickedObj2D.transform.SetParent(TriangleQuadrant[2], true);
            } else if (stage == 3) {
                imageSlots[2]++;
                pos = imageSlots[2] - 1;
                posX = up[pos, 0];
                posY = up[pos, 1];
                pickedObj2D.transform.localPosition = new Vector3(posX, posY, -0.00001f);
                pickedObjText.transform.localPosition = new Vector3(0f, -0.6f, 0f);
                pickedObj2D.transform.SetParent(TriangleQuadrant[0], true);
            } else if (stage == 4) {
                imageSlots[3]++;
                pos = imageSlots[3] - 1;
                posX = down[pos, 0];
                posY = down[pos, 1];
                pickedObj2D.transform.localPosition = new Vector3(posX, posY, -0.00001f);
                pickedObjText.transform.localPosition = new Vector3(0f, -0.6f, 0f);
                pickedObj2D.transform.SetParent(TriangleQuadrant[1], true);
            }
            //pickedObj2D.transform.localPosition = Vector3.zero;
        }
    }

    private void Start() {
        //disableSQUAD();
		//initializeTriangles();
    }

    public void clearList() {
        selectableObjects.Clear();
    }

    public List<GameObject> getSelectableObjects() {
        return selectableObjects;
    }

    public int selectableObjectsCount() {
        return selectableObjects.Count;
    }

    private void OnTriggerStay(Collider collider) {
        if (collider.gameObject.layer != LayerMask.NameToLayer("Ignore Raycast") && !selectableObjects.Contains(collider.gameObject)) {
            selectableObjects.Add(collider.gameObject);
        }
    }

}
