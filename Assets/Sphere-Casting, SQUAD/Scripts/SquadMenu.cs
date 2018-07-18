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

    public List<GameObject> selectableObjects = new List<GameObject>();

    private void destroyChildGameObjects() {
        foreach (Transform child in panel.transform) {
            if (child.gameObject.name != "CreateTrianglesSprite" && child.gameObject.name != "TriangleQuadObject") {
                GameObject.Destroy(child.gameObject);
            } else if (child.gameObject.name == "TriangleQuadObject") {
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

    private bool pickedUpObject = false; //ensure only 1 object is picked up at a time
    private GameObject pickedObject;
    private GameObject lastPickedObject;
    private Material oldPickedObjectMaterial;

    public void selectObject(SteamVR_Controller.Device controller, GameObject obj) {
        //print("picked object:" + pickedObject);
        if (controller.GetTouchDown(SteamVR_Controller.ButtonMask.Trigger) && quadrantIsPicked() == true && pickedObject == null && obj.transform.parent == panel.transform && obj.name != "TriangleQuadObject") {
            //disableSQUAD();
            //pickedObject = obj;
            string objName = obj.name.Substring(0, obj.name.Length-7);
            //print("obj picked:" + objName);
            pickedObject = GameObject.Find(objName);
            lastPickedObject = pickedObject;
            print("Final picked object:" + objName);
            oldPickedObjectMaterial = pickedObject.transform.GetComponent<Renderer>().material;
            pickedObject.transform.GetComponent<Renderer>().material = outlineMaterial;
            disableSQUAD();
        }
    }

    public void enableSQUAD(SteamVR_Controller.Device controller, SteamVR_TrackedObject trackedObj, List<GameObject> obj) {
        if (trackedObj != null) {
            if (controller.GetTouchDown(SteamVR_Controller.ButtonMask.Trigger) && pickedUpObject == false) {
                print("EnableSquad() called");
                SphereCasting.inMenu = true;
                panel.SetActive(true);
                //hideAllGameObjects();
                generate2DObjects(obj);
                if (lastPickedObject != null) {
                    lastPickedObject.transform.GetComponent<Renderer>().material = oldPickedObjectMaterial;
                }
            }
        }
    }


    public void selectQuad(SteamVR_Controller.Device controller, GameObject obj) {
        if (controller.GetTouchDown(SteamVR_Controller.ButtonMask.Trigger)) {
            if (obj.name == "TriangleQuadObject" && isActive() == true && quadrantPicked == false) {
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
            pickedObj2D.transform.SetParent(panel.transform, false);
            pickedObj2D.gameObject.AddComponent<Rigidbody>();
            pickedObj2D.GetComponent<Collider>().attachedRigidbody.isKinematic = true;
            pickedObj2D.transform.localScale = new Vector3(0.0625f, 0.0625f, 0f);
            pickedObj2D.transform.localRotation = Quaternion.identity;
            prefabText.GetComponent<TextMesh>().text = pickedObj.gameObject.name;
            pickedObjText = Instantiate(prefabText, new Vector3(0f, 0f, 0f), Quaternion.identity) as GameObject;
            pickedObjText.transform.SetParent(panel.transform, false);
            pickedObjText.gameObject.AddComponent<Rigidbody>();
            pickedObjText.GetComponent<Collider>().attachedRigidbody.isKinematic = true;
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
                pickedObjText.transform.localPosition = new Vector3(posX-0.01f, posY - 0.04f, -0.00001f);
            } else if (stage == 2) {
                imageSlots[1]++;
                pos = imageSlots[1]-1;
                posX = right[pos, 0];
                posY = right[pos, 1];
                pickedObj2D.transform.localPosition = new Vector3(posX, posY, -0.00001f);
                pickedObjText.transform.localPosition = new Vector3(posX - 0.01f, posY - 0.04f, -0.00001f);
            } else if (stage == 3) {
                imageSlots[2]++;
                pos = imageSlots[2] - 1;
                posX = up[pos, 0];
                posY = up[pos, 1];
                pickedObj2D.transform.localPosition = new Vector3(posX, posY, -0.00001f);
                pickedObjText.transform.localPosition = new Vector3(posX - 0.01f, posY - 0.04f, -0.00001f);
            } else if (stage == 4) {
                imageSlots[3]++;
                pos = imageSlots[3] - 1;
                posX = down[pos, 0];
                posY = down[pos, 1];
                pickedObj2D.transform.localPosition = new Vector3(posX, posY, -0.00001f);
                pickedObjText.transform.localPosition = new Vector3(posX - 0.01f, posY - 0.04f, -0.00001f);
            }
            //pickedObj2D.transform.localPosition = Vector3.zero;
        }
    }


    public void enableSQUAD() {
        panel.SetActive(true);
        SphereCasting.inMenu = true;
    }

    private void Start() {
        disableSQUAD();
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
