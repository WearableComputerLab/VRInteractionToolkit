using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Controls all test objects with test object script

public class MovementController : MonoBehaviour {

	private GameObject[] theObjects;
	private Vector3[] positions;

	// Objects will follow eachother via their set number
	public float movementSpeed = 0;

	// Use this for initialization
	void Start () {
		GameObject[] allObjects = new GameObject[this.transform.childCount];
		
		for(int i = 0; i < this.transform.childCount; i++) {
			allObjects[i] = this.transform.GetChild(i).gameObject;
		}
		GameObject[] objectsInOrder = new GameObject[allObjects.Length];
		int count = 0;
		// Finds all the objects for testing and adds them in order of their assigned index to the orderedList
		foreach(GameObject each in allObjects) {
			TestObject applicableObject;
			
			if((applicableObject = each.GetComponent<TestObject>()) != null) {
				applicableObject.speed = movementSpeed;
				objectsInOrder[applicableObject.assignedID] = applicableObject.gameObject;
				count++;
			}			
		}

		// Setting final array
		theObjects = new GameObject[count];
		positions = new Vector3[count];
		for(int index = 0; index < count; index++) {
			theObjects[index] = objectsInOrder[index];
			positions[index] = theObjects[index].transform.position;
		}

		// each object is given a list of positions to move to in order (index 0 is position of the object itself)
		for(int index = 0; index < count; index++) {

			Vector3[] customPositionsList = new Vector3[theObjects.Length];

			// Two sets of loops to get all objects in order from start index
			int countThroughCustomPositions = 0;

			// loop from index location to end of objects
			for(int customIndex = index; customIndex < count; customIndex++) {
				customPositionsList[countThroughCustomPositions] = positions[customIndex];
				countThroughCustomPositions++;
			}
			// loop from start of objects to index location
			for(int customIndex = 0; customIndex < index; customIndex++) {
				customPositionsList[countThroughCustomPositions] = positions[customIndex];
				countThroughCustomPositions++;
			}
			theObjects[index].GetComponent<TestObject>().positionsToMoveTo = customPositionsList;
		}
	}
	
	// Update is called once per frame (and when scene changes)
	void Update () {
	
	}
}
