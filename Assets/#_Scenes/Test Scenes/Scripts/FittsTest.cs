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
    public GameObject chosenObject;

    //Statistics
    private int selectedCount = -1;
    private float timer;
    private List<float> timeStorage = new List<float>();

    private void Awake() {
        interactableObjects = GameObject.FindGameObjectsWithTag("InteractableObjects");
    }

    private void OnApplicationQuit() {
        print("Application ended after " + Time.time + " seconds");
        print("Amount of selections made:" + selectedCount);
        print("Average time:" + timeStorage.Average() + " milliseconds");
        print("Worst time:" + timeStorage.Max() + " milliseconds");
        print("Best time:" + timeStorage.Min() + " milliseconds");
    }

    void Start () {
        objectSelected();
    }

    void objectSelected() {
        int randomObject = UnityEngine.Random.Range(0, interactableObjects.Length);
        if (chosenObject != null) {
            chosenObject.transform.GetComponent<Renderer>().material = oldMaterial;
        }
        chosenObject = interactableObjects[randomObject];
        print("Chosen object:" + chosenObject.name);
        print("Time taken:" + timer);
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
        if (script.GetComponent<FishingReel>().tempObjectStored != null && script.GetComponent<FishingReel>().tempObjectStored.Equals(chosenObject)) {
                objectSelected();
        }
	}
}
