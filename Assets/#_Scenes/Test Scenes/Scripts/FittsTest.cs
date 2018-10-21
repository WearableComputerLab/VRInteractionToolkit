using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class FittsTest : MonoBehaviour {
    private GameObject[] interactableObjects; // In-game objects
    public GameObject script;
    public Material outlineMaterial;
    private Material oldMaterial;
    private GameObject chosenObject;
    public float objectDistance;
    private float objectDistanceTemp;
    public float objectSize;
    private float objectSizeTemp;

    //Statistics
    private int selectedCount = -1;
    private float timer;
    private List<float> timeStorage = new List<float>();

    private void Awake() {
        //generateObjects();
        interactableObjects = GameObject.FindGameObjectsWithTag("InteractableObjects");
        objectDistanceTemp = objectDistance;
        objectSizeTemp = objectSize;
        initializeObjects();
    }

    private void initializeObjects() {
        foreach (GameObject obj in interactableObjects) {
            obj.transform.position = new Vector3(objectDistance, obj.transform.position.y, obj.transform.position.z);
            obj.transform.localScale = new Vector3(objectSize, objectSize, objectSize);
        }
    }

    private void initializeObjectPos() {
        foreach (GameObject obj in interactableObjects) {
            obj.transform.position = new Vector3(objectDistance, obj.transform.position.y, obj.transform.position.z);
        }
    }

    private void initializeObjectSize() {
        foreach (GameObject obj in interactableObjects) {
            obj.transform.localScale = new Vector3(objectSize, objectSize, objectSize);
        }
    }

    private void OnApplicationQuit() {
        print("Application ended after " + Time.time + " seconds");
        print("Amount of selections made:" + selectedCount);
        print("Average time:" + timeStorage.Average() + " milliseconds");
        print("Worst time:" + timeStorage.Max() + " milliseconds");
        print("Best time:" + timeStorage.Min() + " milliseconds");
    }

    /*private void generateObjects() {
        print("Spawned parent");
        GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        obj.transform.SetParent(ObjParents.transform);

    }*/

    void Start () {
        objectSelected();
    }

    void objectSelected() {
        
        script.GetComponent<FishingReel>();
        int randomObject = UnityEngine.Random.Range(0, interactableObjects.Length);
        if (chosenObject != null) {
            chosenObject.transform.GetComponent<Renderer>().material = oldMaterial;
        }
        chosenObject = interactableObjects[randomObject];
        //print("Chosen object:" + chosenObject.name);
        //print("Time taken:" + timer);
        if (timer != 0) { //Ignore the selection onload
            timeStorage.Add(timer);
        }
        timer = 0f;
        oldMaterial = chosenObject.transform.GetComponent<Renderer>().material;
        chosenObject.transform.GetComponent<Renderer>().material = outlineMaterial;
        selectedCount++;
    }

	void Update () {
        if (objectDistanceTemp != objectDistance) {
            //print("distance value changed");
            initializeObjectPos();
        }
        if (objectSizeTemp != objectSize) {
            //print("size value changed");
            initializeObjectSize();
        }
        objectDistanceTemp = objectDistance;
        objectSizeTemp = objectSize;
        timer += Time.deltaTime * 1000;
        if (script.GetComponent<FishingReel>().lastSelectedObject != null && script.GetComponent<FishingReel>().lastSelectedObject.Equals(chosenObject)) {
                objectSelected();
        }
	}
}
