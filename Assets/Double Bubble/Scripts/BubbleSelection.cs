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
    public GameObject canvas;
    public GameObject cursor2D;
    public GameObject radiusBubble2D;
    public GameObject objectBubble2D;

    private bool pickedUpObject = false; //ensure only 1 object is picked up at a time
    private GameObject tempObjectStored;

    private GameObject pickedObj2D = null;
    private GameObject pickedObj = null;
    private int imageSlots = 0;
    private float[,] positions = new float[,] { { -0.3f, 0.2f }, { -0.1f, 0.2f }, { 0.1f, 0.2f }, { 0.3f, 0.2f },
                                                { -0.3f, 0.1f }, { -0.1f, 0.1f }, { 0.1f, 0.1f }, { 0.3f, 0.1f },
                                                { -0.3f, 0f }, { -0.1f, 0f }, { 0.1f, 0f }, { 0.3f, 0f },
                                                { -0.3f, -0.1f }, { -0.1f, -0.1f }, { 0.1f, -0.1f }, { 0.3f, -0.1f },
                                                { -0.3f, -0.2f  }, { -0.1f, -0.2f  }, { 0.1f, -0.2f }, { 0.3f, -0.2f },
                                                { -0.3f, -0.3f }, { -0.1f, -0.3f }, { 0.1f, -0.3f }, { 0.3f, -0.3f }};


    void generate2DObjects(List<GameObject> pickedObject) {
        pickedObjects = new GameObject[pickedObject.Count];
        pickedObject.CopyTo(pickedObjects);
        print("generate2DObjectsSIZE:" + pickedObjects.Length);
        panel.transform.SetParent(null);
        canvas.transform.SetParent(null);
        print("Amount of objects selected:" + pickedObject.Count);
        for (int i = 0; i < pickedObject.Count; i++) {
            print("object:" + pickedObject[i].name + " | count:" + (i+1));
            pickedObj = pickedObject[i];
            pickedObj2D = Instantiate(pickedObject[i], new Vector3(0f, 0f, 0f), Quaternion.identity) as GameObject;
            pickedObj2D.transform.SetParent(panel.transform, false);
            pickedObj2D.gameObject.AddComponent<Rigidbody>();
            pickedObj2D.GetComponent<Collider>().attachedRigidbody.isKinematic = true;
            pickedObj2D.transform.localScale = new Vector3(0.0625f, 0.0625f, 0f);
            pickedObj2D.transform.localRotation = Quaternion.identity;

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

    private float[][] ClosestObject() {
        float[] lowestDists = new float[4];
        lowestDists[0] = 0; // 1ST Lowest Distance
        lowestDists[1] = 0; // 2ND Lowest Distance
        lowestDists[2] = 0; // 1ST Lowest Index
        lowestDists[3] = 0; // 2ND Lowest Indexs
        float lowestDist = 0;
        float[][] allDists = new float[pickedObjects.Length][];
        for (int i = 0; i < pickedObjects.Length; i++) {
            allDists[i] = new float[2];
        }
        int lowestValue = 0;
        for (int i = 0; i < pickedObjects.Length; i++) {
            Vector3 objPos = panel.transform.Find(pickedObjects[i].name + "(Clone)").transform.localPosition;
            float dist = Vector3.Distance(cursor2D.transform.localPosition, objPos)/0.125f;
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
        print("lowest dist:" + lowestDist);
        float[][] arraytest = allDists.OrderBy(row => row[0]).ToArray();
        return arraytest;
    }

    /*public void PickupObject(SteamVR_Controller.Device controller, SteamVR_TrackedObject trackedObj, List<GameObject> obj) {
        if (trackedObj != null) {
            if (controller.GetTouchDown(SteamVR_Controller.ButtonMask.Trigger) && pickedUpObject == false) {
                for (int i = 0; i < obj.Count; i++) {
                    if (obj[i].tag == "InteractableObjects") {
                        obj[i].transform.SetParent(trackedObj.transform);
                        pickedUpObject = true;
                    }
                }
            }
            if (controller.GetTouchUp(SteamVR_Controller.ButtonMask.Trigger) && pickedUpObject == true) {
                for (int i = 0; i < obj.Count; i++) {
                    if (obj[i].tag == "InteractableObjects") {
                        obj[i].transform.SetParent(null);
                        pickedUpObject = false;
                    }
                }
            }
        }
    }*/
    public SteamVR_TrackedObject trackedObj;

    public void disableMenu() {
        canvas.SetActive(false);
        panel.SetActive(false);
    }

    public void enableMenu(SteamVR_Controller.Device controller, SteamVR_TrackedObject trackedObj, List<GameObject> obj) {
        if (trackedObj != null) {
            if (controller.GetTouchDown(SteamVR_Controller.ButtonMask.Trigger) && pickedUpObject == false) {
                //canvas.SetActive(true);
                panel.SetActive(true);
                generate2DObjects(obj);
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
        //canvas.SetActive(true);
        panel.SetActive(true);
    }

    private void Start() {
        disableMenu();
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

    private readonly float bubbleOffset = 0.6f;

    private void Update() {
        if (trackedObj != null) {
            //controller = SteamVR_Controller.Input((int)trackedObj.index);
            moveCursor();
        }
        if (pickedObjects != null) {
            float[][] lowestDistances = ClosestObject();
            print("TARGET:" + pickedObjects[(int)lowestDistances[0][1]].name);
            /*print("1st:" + lowestDistances[0][0]);
            print("2nd:" + lowestDistances[1][0]);
            print("3rd:" + lowestDistances[2][0]);
            print("4th:" + lowestDistances[3][0]);*/
            float ClosestCircleRadius = 0f;
            float SecondClosestCircleRadius = 0f;

            //print("SIZE:" + pickedObjects.Length);
            print("lowestDist:" + lowestDistances[0][1]);
            ClosestCircleRadius = lowestDistances[0][0] + pickedObjects[(int)lowestDistances[0][1]].GetComponent<SphereCollider>().radius;
            SecondClosestCircleRadius = lowestDistances[1][0] - pickedObjects[(int)lowestDistances[1][1]].GetComponent<SphereCollider>().radius;

            /*if (pickedObjects[(int)lowestDistances[0][1]].GetComponent<Collider>().GetType() == typeof(SphereCollider)) {
                ClosestCircleRadius = lowestDistances[0][0] + pickedObjects[(int)lowestDistances[0][1]].GetComponent<SphereCollider>().radius;
            }
            if (pickedObjects[(int)lowestDistances[1][1]].GetComponent<Collider>().GetType() == typeof(SphereCollider)) {
                SecondClosestCircleRadius = lowestDistances[1][0] - pickedObjects[(int)lowestDistances[1][1]].GetComponent<SphereCollider>().radius;
            }

            if (pickedObjects[(int)lowestDistances[0][1]].GetComponent<Collider>().GetType() == typeof(CapsuleCollider)) {
                ClosestCircleRadius = lowestDistances[0][0] + pickedObjects[(int)lowestDistances[0][1]].GetComponent<CapsuleCollider>().radius;
            }
            if (pickedObjects[(int)lowestDistances[1][1]].GetComponent<Collider>().GetType() == typeof(CapsuleCollider)) {
                SecondClosestCircleRadius = lowestDistances[1][0] - pickedObjects[(int)lowestDistances[1][1]].GetComponent<CapsuleCollider>().radius;
            }

            if (pickedObjects[(int)lowestDistances[0][1]].GetComponent<Collider>().GetType() == typeof(BoxCollider)) {
                ClosestCircleRadius = lowestDistances[0][0] + pickedObjects[(int)lowestDistances[0][1]].GetComponent<BoxCollider>().size.x;
            }
            if (pickedObjects[(int)lowestDistances[1][1]].GetComponent<Collider>().GetType() == typeof(BoxCollider)) {
                SecondClosestCircleRadius = lowestDistances[1][0] - pickedObjects[(int)lowestDistances[1][1]].GetComponent<BoxCollider>().size.x;
            }*/

            float closestValue = Mathf.Min(ClosestCircleRadius, SecondClosestCircleRadius);
            closestValue -= 0.5f;
            print("FIRST closest radius:" + ClosestCircleRadius + " | closest value:" + closestValue);
            print("SECOND closest radius:" + SecondClosestCircleRadius + " | closest value:" + closestValue);
            if (ClosestCircleRadius * 2 < SecondClosestCircleRadius * 2) {
                cursor2D.GetComponent<SphereCollider>().radius = (closestValue + ClosestCircleRadius);
                print("new radius:" + closestValue + ClosestCircleRadius);
                //radiusBubble2D.transform.localScale = new Vector3((closestValue + ClosestCircleRadius) * 2, (closestValue + ClosestCircleRadius) * 2, (closestValue + ClosestCircleRadius) * 2);
                //print("TARGET:" + pickedObjects[(int)lowestDistances[0][1]].name);
                //objectBubble2D.transform.localScale = new Vector3(0f, 0f, 0f);
            } else {
                cursor2D.GetComponent<SphereCollider>().radius = (closestValue + SecondClosestCircleRadius);
                print("new radius:" + closestValue + SecondClosestCircleRadius);
                //radiusBubble2D.transform.localScale = new Vector3((closestValue + SecondClosestCircleRadius) * 2, (closestValue + SecondClosestCircleRadius) * 2, (closestValue + SecondClosestCircleRadius) * 2);
                //print("TARGET:" + pickedObjects[(int)lowestDistances[1][1]].name);
                //objectBubble2D.transform.position = pickedObjects[(int)lowestDistances[0][1]].transform.position;
                //objectBubble2D.transform.localScale = new Vector3(pickedObjects[(int)lowestDistances[0][1]].transform.localScale.x + bubbleOffset, pickedObjects[(int)lowestDistances[0][1]].transform.localScale.y + bubbleOffset, pickedObjects[(int)lowestDistances[0][1]].transform.localScale.z + bubbleOffset);
            }
        }
    }

    private void OnTriggerStay(Collider collider) {
        if (collider.gameObject.tag == "InteractableObjects") {
            selectableObjects.Add(collider.gameObject);
        }
    }

}
