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
            print("object:" + pickedObject[i].name + " | count:" + (i+1));
            pickedObj = pickedObject[i];
            pickedObj2D = Instantiate(pickedObject[i], new Vector3(0f, 0f, 0f), Quaternion.identity) as GameObject;
            pickedObj2D.transform.SetParent(panel.transform, false);
            pickedObj2D.gameObject.AddComponent<Rigidbody>();
            pickedObj2D.GetComponent<Collider>().attachedRigidbody.isKinematic = true;
            //pickedObj2D.transform.localScale = new Vector3(0.0625f, 0.0625f, 0f);
            pickedObj2D.transform.localScale = new Vector3(pickedObject[i].transform.localScale.x / scaleAmount, pickedObject[i].transform.localScale.y / scaleAmount, pickedObject[i].transform.localScale.z / scaleAmount);
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
            Transform objPos = panel.transform.Find(pickedObjects[i].name + "(Clone)").transform;
            print("cursorPos:" + cursor2D.transform.localPosition);
            print("objPos:" + objPos.localPosition);
            float dist = Vector3.Distance(cursor2D.transform.localPosition, objPos.localPosition);
            dist = dist * (0.5f / objPos.localScale.x);
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
        panel.SetActive(false);
        inBubbleSelection = false;
    }

    public void enableMenu(SteamVR_Controller.Device controller, SteamVR_TrackedObject trackedObj, List<GameObject> obj) {
        if (trackedObj != null) {
            if (controller.GetTouchDown(SteamVR_Controller.ButtonMask.Trigger) && pickedUpObject == false) {
                panel.SetActive(true);
                inBubbleSelection = true;
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

    private readonly float bubbleOffset = 0.05f;

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
            //float scaledRadiusClosest = pickedObjects[(int)lowestDistances[0][1]].GetComponent<SphereCollider>().radius * (pickedObjects[(int)lowestDistances[0][1]].transform.localScale.x/10f);
            //float scaledRadiusSecond = pickedObjects[(int)lowestDistances[1][1]].GetComponent<SphereCollider>().radius * (pickedObjects[(int)lowestDistances[1][1]].transform.localScale.x/10f);
            //print("x:" + pickedObjects[(int)lowestDistances[0][1]].transform.localScale.x);
            //print("scaled radius closest:" + scaledRadiusClosest + " | w/out radius:" + lowestDistances[0][0]);
            //print("scaled radius second:" + scaledRadiusSecond + " | w/out radius:" + lowestDistances[1][0]);
            //ClosestCircleRadius = lowestDistances[0][0] + pickedObjects[(int)lowestDistances[0][1]].GetComponent<SphereCollider>().radius;
            //SecondClosestCircleRadius = lowestDistances[1][0] - pickedObjects[(int)lowestDistances[1][1]].GetComponent<SphereCollider>().radius;

            //ClosestCircleRadius = lowestDistances[0][0] + pickedObjects[(int)lowestDistances[0][1]].GetComponent<SphereCollider>().radius/2f;
            //SecondClosestCircleRadius = lowestDistances[1][0] - pickedObjects[(int)lowestDistances[1][1]].GetComponent<SphereCollider>().radius/2f;


            //ClosestCircleRadius = lowestDistances[0][0];
            //SecondClosestCircleRadius = lowestDistances[1][0];

            Transform gameObjClosest = panel.transform.Find(pickedObjects[(int)lowestDistances[0][1]].transform.name + "(Clone)");
            Transform gameObjSecond = panel.transform.Find(pickedObjects[(int)lowestDistances[1][1]].transform.name + "(Clone)");
            if (pickedObjects[(int)lowestDistances[0][1]].GetComponent<Collider>().GetType() == typeof(SphereCollider)) {
                /*Transform findObject = panel.transform.Find(pickedObjects[(int)lowestDistances[0][1]].transform.name + "(Clone)");
                float objectScale = findObject.GetComponent<SphereCollider>().radius / (findObject.localScale.x * 10);
                //cursor2D.GetComponent<SphereCollider>().radius = (closestValue + ClosestCircleRadius)/2f;
                float finalVal = lowestDistances[0][0] + (pickedObjects[(int)lowestDistances[0][1]].GetComponent<SphereCollider>().radius * gameObjClosest.localScale.x * 10);
                finalVal = finalVal / objectScale;
                ClosestCircleRadius = finalVal;*/


                print("gameobjscale:" + gameObjClosest.localScale.x);
                print("old:"+ pickedObjects[(int)lowestDistances[0][1]].GetComponent<SphereCollider>().radius / 2f);
                ClosestCircleRadius = lowestDistances[0][0] + (pickedObjects[(int)lowestDistances[0][1]].GetComponent<SphereCollider>().radius * gameObjClosest.localScale.x*10);
                print("new:" + (pickedObjects[(int)lowestDistances[0][1]].GetComponent<SphereCollider>().radius * gameObjClosest.localScale.x * 10));
            }
            if (pickedObjects[(int)lowestDistances[1][1]].GetComponent<Collider>().GetType() == typeof(SphereCollider)) {
                SecondClosestCircleRadius = lowestDistances[1][0] - (pickedObjects[(int)lowestDistances[1][1]].GetComponent<SphereCollider>().radius * gameObjSecond.localScale.x * 10);
            }

            if (pickedObjects[(int)lowestDistances[0][1]].GetComponent<Collider>().GetType() == typeof(CapsuleCollider)) {
                ClosestCircleRadius = lowestDistances[0][0] + pickedObjects[(int)lowestDistances[0][1]].GetComponent<CapsuleCollider>().radius / 2f;
            }
            if (pickedObjects[(int)lowestDistances[1][1]].GetComponent<Collider>().GetType() == typeof(CapsuleCollider)) {
                SecondClosestCircleRadius = lowestDistances[1][0] - pickedObjects[(int)lowestDistances[1][1]].GetComponent<CapsuleCollider>().radius / 2f;
            }

            if (pickedObjects[(int)lowestDistances[0][1]].GetComponent<Collider>().GetType() == typeof(BoxCollider)) {
                ClosestCircleRadius = lowestDistances[0][0] + pickedObjects[(int)lowestDistances[0][1]].GetComponent<BoxCollider>().size.x / 2f;
            }
            if (pickedObjects[(int)lowestDistances[1][1]].GetComponent<Collider>().GetType() == typeof(BoxCollider)) {
                SecondClosestCircleRadius = lowestDistances[1][0] - pickedObjects[(int)lowestDistances[1][1]].GetComponent<BoxCollider>().size.x / 2f;
            }

            float closestValue = Mathf.Min(ClosestCircleRadius, SecondClosestCircleRadius);

            print("FIRST closest radius:" + ClosestCircleRadius + " | closest value:" + closestValue);
            print("SECOND closest radius:" + SecondClosestCircleRadius + " | closest value:" + closestValue);
            if (ClosestCircleRadius < SecondClosestCircleRadius) {
                Transform findObject = panel.transform.Find(pickedObjects[(int)lowestDistances[0][1]].transform.name + "(Clone)");
                float objectScale = findObject.GetComponent<SphereCollider>().radius / (findObject.localScale.x*10);
                //cursor2D.GetComponent<SphereCollider>().radius = (closestValue + ClosestCircleRadius)/2f;
                float finalVal = (closestValue + ClosestCircleRadius) / 2f;
                finalVal = finalVal/objectScale;
                cursor2D.GetComponent<SphereCollider>().radius = finalVal;
                radiusBubble2D.transform.localScale = new Vector3((closestValue + ClosestCircleRadius), (closestValue + ClosestCircleRadius), (closestValue + ClosestCircleRadius));
                //print("TARGET:" + pickedObjects[(int)lowestDistances[0][1]].name);
                objectBubble2D.transform.localScale = new Vector3(0f, 0f, 0f);
            } else {
                cursor2D.GetComponent<SphereCollider>().radius = (closestValue + SecondClosestCircleRadius)/2f;
                print("new radius:" + closestValue + SecondClosestCircleRadius);
                radiusBubble2D.transform.localScale = new Vector3((closestValue + SecondClosestCircleRadius), (closestValue + SecondClosestCircleRadius), (closestValue + SecondClosestCircleRadius));
                //print("TARGET:" + pickedObjects[(int)lowestDistances[1][1]].name);
                //print("OBJECT:" + pickedObjects[(int)lowestDistances[0][1]].transform.name);
                //print("DIST:" + panel.transform.Find(pickedObjects[(int)lowestDistances[0][1]].transform.name+"(Clone)").localPosition);
                Transform findObject = panel.transform.Find(pickedObjects[(int)lowestDistances[0][1]].transform.name + "(Clone)");
                objectBubble2D.transform.localPosition = findObject.localPosition;
                objectBubble2D.transform.localScale = new Vector3(findObject.transform.localScale.x + bubbleOffset, findObject.localScale.y + bubbleOffset, findObject.transform.localScale.z + bubbleOffset);
            }
        }
    }

    private void OnTriggerStay(Collider collider) {
        if (collider.gameObject.tag == "InteractableObjects") {
            selectableObjects.Add(collider.gameObject);
        }
    }

}
