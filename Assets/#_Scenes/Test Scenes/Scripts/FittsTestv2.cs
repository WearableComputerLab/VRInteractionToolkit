using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class FittsTestv2 : MonoBehaviour {
    private GameObject[] interactableObjects; // In-game objects
    public GameObject script;
    public Material outlineMaterial;
    private Material oldMaterial;
    private GameObject chosenObject;

    //Statistics
    private int selectedCount = -1;
    private float timer;
    private List<float> timeStorage = new List<float>();

    private void Awake() {
        //generateObjects();
        interactableObjects = GameObject.FindGameObjectsWithTag("InteractableObjects");
    }

    //ID = Log2 (2D/W) (D = Distance from hand to center of target, W = width of target)
    //Gets the difficulty of selection
    private double getIndexDifficulty(float D, float W) {
        return Math.Log(2f) * (2D/W);
    }

    //Gets the IP (Index Performance) to measure the human performance.
    //ID = Index Difficulty, MT = Avg time to complete movement
    private double getIndexPerformance(double ID, double MT) {
        return (ID / MT);
    }

    //MT = Average time to complete movement
    //a and b are constants (Not 100% sure on what to do with these)
    private readonly double a = 0;
    private readonly double b = 0;
    private double getAverageTime(double ID) {
        return a + b * ID;
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
        timer += Time.deltaTime * 1000;
        if (script.GetComponent<FishingReel>().lastSelectedObject != null && script.GetComponent<FishingReel>().lastSelectedObject.Equals(chosenObject)) {
                objectSelected();
        }
	}
}
