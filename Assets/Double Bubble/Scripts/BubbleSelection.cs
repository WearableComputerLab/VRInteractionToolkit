using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BubbleSelection : MonoBehaviour {

    public List<GameObject> selectableObjects = new List<GameObject>();
    //private List<GameObject> pickedObjects = new List<GameObject>();

    private GameObject[] pickedObjects;
    public BubbleCursor3D bubbleCursor;
    public GameObject panel;
    public GameObject cursor2D;
    public GameObject radiusBubble2D;
    public GameObject objectBubble2D;
    public bool inBubbleSelection = false;

    private bool pickedUpObject = false; //ensure only 1 object is picked up at a time
    private GameObject tempObjectStored;
    private SteamVR_Controller.Device controller;
    private GameObject pickedObj2D = null;
    private GameObject pickedObj = null;
    private int imageSlots = 0;
    private float[,] positions = new float[,] { { -0.3f, 0.2f }, { -0.1f, 0.2f }, { 0.1f, 0.2f }, { 0.3f, 0.2f },
                                                { -0.3f, 0.1f }, { -0.1f, 0.1f }, { 0.1f, 0.1f }, { 0.3f, 0.1f },
                                                { -0.3f, 0f }, { -0.1f, 0f }, { 0.1f, 0f }, { 0.3f, 0f },
                                                { -0.3f, -0.1f }, { -0.1f, -0.1f }, { 0.1f, -0.1f }, { 0.3f, -0.1f },
                                                { -0.3f, -0.2f  }, { -0.1f, -0.2f  }, { 0.1f, -0.2f }, { 0.3f, -0.2f },
                                                { -0.3f, -0.3f }, { -0.1f, -0.3f }, { 0.1f, -0.3f }, { 0.3f, -0.3f }};

    private float scaleAmount = 10f;
    void generate2DObjects(List<GameObject> pickedObject) {
        pickedObjects = new GameObject[pickedObject.Count];
        pickedObject.CopyTo(pickedObjects);
        print("generate2DObjectsSIZE:" + pickedObjects.Length);
        panel.transform.SetParent(null);
        print("Amount of objects selected:" + pickedObject.Count);
        for (int i = 0; i < pickedObject.Count; i++) {
            print("generating object:" + pickedObject[i].name + " | at pos:" + (i+1));
            pickedObj = pickedObject[i];
            pickedObj2D = Instantiate(pickedObject[i], new Vector3(0f, 0f, 0f), Quaternion.identity) as GameObject;
            pickedObj2D.transform.SetParent(panel.transform, false);
            pickedObj2D.gameObject.AddComponent<Rigidbody>();
            pickedObj2D.GetComponent<Collider>().attachedRigidbody.isKinematic = true;
            //pickedObj2D.transform.localScale = new Vector3(0.0625f, 0.0625f, 0f);
            pickedObj2D.transform.localScale = new Vector3(pickedObject[i].transform.localScale.x / scaleAmount, pickedObject[i].transform.localScale.y / scaleAmount, pickedObject[i].transform.localScale.z / scaleAmount);
            pickedObj2D.transform.localRotation = Quaternion.identity;
            pickedObj2D.name = pickedObject[i].name + " (Clone)";
            int pos = 0;
            float posX = 0;
            float posY = 0;
            imageSlots++;
            pos = imageSlots - 1;
            posX = positions[pos, 0];
            posY = positions[pos, 1];
            pickedObj2D.transform.localPosition = new Vector3(posX, posY, 0f);
        }
    }

    private SteamVR_TrackedObject trackedObj;
    public GameObject controllerRight;
    public GameObject controllerLeft;
    public GameObject cameraHead;
    public bool controllerRightPicked;
    public bool controllerLeftPicked;
    public bool cameraHeadPicked;


    void Awake() {
        //getTrackedObject();
    }

    private float[][] ClosestObject() {
        float lowestDist = 0;
        float[][] allDists = new float[pickedObjects.Length][];
        for (int i = 0; i < pickedObjects.Length; i++) {
            allDists[i] = new float[2];
        }
        int lowestValue = 0;
        for (int i = 0; i < pickedObjects.Length; i++) {
            Transform objPos = panel.transform.Find(pickedObjects[i].name+" (Clone)").transform;
            //print("cursorPos:" + cursor2D.transform.localPosition);
            //print("objPos:" + objPos.localPosition);
            float dist = Vector3.Distance(cursor2D.transform.localPosition, objPos.localPosition);
			Collider myCollider = pickedObjects [i].GetComponent<Collider> ();
			if (myCollider.GetType() == typeof(SphereCollider)) {
				dist -= pickedObjects [i].GetComponent<SphereCollider> ().radius * objPos.localScale.x;
			} else if (myCollider.GetType() == typeof(CapsuleCollider)) {
				dist -= pickedObjects [i].GetComponent<CapsuleCollider> ().radius * objPos.localScale.x;
			} else if (myCollider.GetType() == typeof(BoxCollider)) {
				dist -= pickedObjects [i].GetComponent<BoxCollider> ().size.x * objPos.localScale.x;
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
        }
        float[][] arraytest = allDists.OrderBy(row => row[0]).ToArray();
        return arraytest;
    }



    public void disableMenuOnLoad() {
        panel.SetActive(false);
        inBubbleSelection = false;
    }

    private GameObject selectedObject;

    public void disableMenuOnTrigger() {
        if (controller.GetTouchDown(SteamVR_Controller.ButtonMask.Trigger) && inBubbleSelection == true) {
            print("Selected object:"+ selectedObject.name);
            clearList();
            destroyChildGameObjects();
            pickedObjects = null;
            panel.SetActive(false);
            inBubbleSelection = false;
            imageSlots = 0;
            if (bubbleCursor != null) {
                if (bubbleCursor.cursor != null) {
                    bubbleCursor.cursor.SetActive(true);
                }
            }
        }
    }

    public void enableMenu(SteamVR_Controller.Device myController, SteamVR_TrackedObject myTrackedObj, List<GameObject> obj) {
        if (myTrackedObj != null) {
            if (myController.GetTouchDown(SteamVR_Controller.ButtonMask.Trigger) && inBubbleSelection == false) {
                print("size:" + obj.Count);
                trackedObj = myTrackedObj;
                panel.SetActive(true);
                bubbleCursor.cursor.SetActive(false);
                inBubbleSelection = true;
                generate2DObjects(obj);
            }
        }
    }

    private void destroyChildGameObjects() {
        foreach (Transform child in panel.transform) {
            if (child.gameObject.name != "ObjectBubble2D" && child.gameObject.name != "Cursor2D" && child.gameObject.name != "RadiusBubble2D") {
                GameObject.Destroy(child.gameObject);
            }
        }
    }


    private Vector3 controllerPos = new Vector3(0, 0, 0);

    void getControllerPosition() {
         controllerPos = trackedObj.transform.forward;
    }

    void moveCursor() {
        getControllerPosition();
        Vector3 pose = new Vector3(0, 0, 0); ;
        pose = trackedObj.transform.position;
        float distance_formula_on_vector = Mathf.Sqrt(controllerPos.x * controllerPos.x + controllerPos.y * controllerPos.y + controllerPos.z * controllerPos.z);
        // Using formula to find a point which lies at distance on a 3D line from vector and direction
        pose.x = pose.x + ((distance_formula_on_vector)) * controllerPos.x;
        pose.y = pose.y + ((distance_formula_on_vector)) * controllerPos.y;
        //pose.z = pose.z + ((distance_formula_on_vector)) * controllerPos.z;

        cursor2D.transform.position = pose;
        cursor2D.transform.rotation = new Quaternion(0f, 0f, 0f, 0f);
    }

    public void enableMenu() {
        panel.SetActive(true);
    }

    private void Start() {
        bubbleCursor = GameObject.Find("3DBubbleCursor_Technique").GetComponent<BubbleCursor3D>();
        disableMenuOnLoad();
    }


    public void clearList() {
        selectableObjects.Clear();
        //pickedObjects = null;
    }

    public List<GameObject> getSelectableObjects() {
        return selectableObjects;
    }

    public int selectableObjectsCount() {
        return selectableObjects.Count;
    }

    private readonly float bubbleOffset = 0.025f;

    private void Update() {
        if (inBubbleSelection == true) {
            if (trackedObj != null) {
                controller = SteamVR_Controller.Input((int)trackedObj.index);
                moveCursor();
            }
            if (pickedObjects != null) {
                float[][] lowestDistances = ClosestObject();
                Transform objPos = panel.transform.Find(pickedObjects[(int)lowestDistances[0][1]].name + " (Clone)").transform;
                Transform objPos2 = panel.transform.Find(pickedObjects[(int)lowestDistances[1][1]].name + " (Clone)").transform;
                //print("TARGET:" + pickedObjects[(int)lowestDistances[0][1]].name);
                float ClosestCircleRadius = 0f;
                float SecondClosestCircleRadius = 0f;

                //ClosestCircleRadius = lowestDistances[0][0] + (objPos.GetComponent<SphereCollider>().radius * objPos.localScale.x) + (objPos.GetComponent<SphereCollider>().radius * objPos.localScale.x);
                //SecondClosestCircleRadius = lowestDistances[1][0] - (objPos2.GetComponent<SphereCollider>().radius * objPos2.localScale.x) + (objPos2.GetComponent<SphereCollider>().radius * objPos2.localScale.x);

                
                if (pickedObjects[(int)lowestDistances[0][1]].GetComponent<Collider>().GetType() == typeof(SphereCollider)) {
                    ClosestCircleRadius = lowestDistances[0][0] + (objPos.GetComponent<SphereCollider>().radius * objPos.localScale.x) + (objPos.GetComponent<SphereCollider>().radius * objPos.localScale.x);
                }
                if (pickedObjects[(int)lowestDistances[1][1]].GetComponent<Collider>().GetType() == typeof(SphereCollider)) {
                    SecondClosestCircleRadius = lowestDistances[1][0] - (objPos2.GetComponent<SphereCollider>().radius * objPos2.localScale.x) + (objPos2.GetComponent<SphereCollider>().radius * objPos2.localScale.x);
                }
                if (pickedObjects[(int)lowestDistances[0][1]].GetComponent<Collider>().GetType() == typeof(CapsuleCollider)) {
                    ClosestCircleRadius = lowestDistances[0][0] + (objPos.GetComponent<CapsuleCollider>().radius * objPos.localScale.x) + (objPos.GetComponent<CapsuleCollider>().radius * objPos.localScale.x);
                }
                if (pickedObjects[(int)lowestDistances[1][1]].GetComponent<Collider>().GetType() == typeof(CapsuleCollider)) {
                    SecondClosestCircleRadius = lowestDistances[1][0] - (objPos2.GetComponent<CapsuleCollider>().radius * objPos2.localScale.x) + (objPos2.GetComponent<CapsuleCollider>().radius * objPos2.localScale.x);
                }

                if (pickedObjects[(int)lowestDistances[0][1]].GetComponent<Collider>().GetType() == typeof(BoxCollider)) {
					ClosestCircleRadius = lowestDistances[0][0] + (objPos.GetComponent<BoxCollider>().size.x * objPos.localScale.x) + (objPos.GetComponent<BoxCollider>().size.x * objPos.localScale.x);
                }
                if (pickedObjects[(int)lowestDistances[1][1]].GetComponent<Collider>().GetType() == typeof(BoxCollider)) {
					SecondClosestCircleRadius = lowestDistances[1][0] - (objPos2.GetComponent<BoxCollider>().size.x * objPos2.localScale.x) + (objPos2.GetComponent<BoxCollider>().size.x * objPos2.localScale.x);
                }

                float closestValue = Mathf.Min(ClosestCircleRadius, SecondClosestCircleRadius);
                selectedObject = pickedObjects[(int)lowestDistances[0][1]];
               // print("FIRST closest radius:" + ClosestCircleRadius + " | closest value:" + closestValue);
               // print("SECOND closest radius:" + SecondClosestCircleRadius + " | closest value:" + closestValue);
                if (ClosestCircleRadius < SecondClosestCircleRadius) {
                    float finalVal = ((closestValue + ClosestCircleRadius) * 10) / 2;
                    cursor2D.GetComponent<SphereCollider>().radius = finalVal;
                    radiusBubble2D.transform.localScale = new Vector3(finalVal * 2, finalVal * 2, 1f);
                    objectBubble2D.transform.localScale = new Vector3(0f, 0f, 0f);
                    //Usually it works with the objectBubble disappearing when the object is fully encapsulated, but it's alot more user-friendly commenting out this part for the 2D selection
                    //bubbleCursor.objectBubble.transform.localScale = new Vector3(0f, 0f, 0f);
                } else {
                    float finalVal = ((closestValue + SecondClosestCircleRadius) * 10) / 2;
                    cursor2D.GetComponent<SphereCollider>().radius = finalVal;
                    radiusBubble2D.transform.localScale = new Vector3(finalVal * 2, finalVal * 2, 1f);
                    string objName = pickedObjects[(int)lowestDistances[0][1]].transform.name.Substring(0, pickedObjects[(int)lowestDistances[0][1]].transform.name.Length);
                   // print("objName" + objName);
                    Transform findOriginalObject = GameObject.Find(objName).transform;
                    Transform findObject = panel.transform.Find(pickedObjects[(int)lowestDistances[0][1]].transform.name + " (Clone)");
                    objectBubble2D.transform.localPosition = findObject.localPosition;
                    objectBubble2D.transform.localScale = new Vector3(findObject.transform.localScale.x + bubbleOffset, findObject.localScale.y + bubbleOffset, findObject.transform.localScale.z + bubbleOffset);
                    bubbleCursor.objectBubble.transform.position = findOriginalObject.position;
                    bubbleCursor.objectBubble.transform.localScale = new Vector3(findOriginalObject.localScale.x + bubbleCursor.bubbleOffset, findOriginalObject.localScale.y + bubbleCursor.bubbleOffset, findOriginalObject.transform.localScale.z + bubbleCursor.bubbleOffset);
                }
            }
            disableMenuOnTrigger();
        }
    }

}
