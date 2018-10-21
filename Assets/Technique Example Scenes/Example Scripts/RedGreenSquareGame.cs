using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RedGreenSquareGame : MonoBehaviour {

	// Must be attached to green square

	public GameObject GreenSquare;

	public List<GameObject> gameObjects;
	private Vector3[] gameObjectStartPositions;
	private Quaternion[] startRotations;
	private Vector3[] startSizes;

	private List<GameObject> objectsInRed;
	private List<GameObject> objectsInGreen;

	public Text timer;

	public Text finalScore;

	private float count = 0;
	private float lastTime; 

	private bool counting = false;

	// Use this for initialization
	void Start () {
		lastTime = (int)Time.time;

		gameObjectStartPositions = new Vector3[gameObjects.Count];
		startRotations = new Quaternion[gameObjects.Count];
		startSizes = new Vector3[gameObjects.Count];
		objectsInGreen = new List<GameObject>();
		objectsInRed = new List<GameObject>();

		// Get start postions for all objects so can reset
		for(int i = 0; i < gameObjects.Count; i++) {
			gameObjectStartPositions[i] = gameObjects[i].transform.position;
			startRotations[i] = gameObjects[i].transform.rotation;
			startSizes[i] = gameObjects[i].transform.localScale;
			objectsInRed.Add(gameObjects[i]);
		}
	}
	
	// Update is called once per frame
	void Update () {
		if(counting && Time.time-lastTime >= 1){
			// one scond has past so can add to timer
			lastTime = Time.time;
			count += 1;
			timer.text = count.ToString()  + " Sec";
		}
	}

	void OnTriggerEnter(Collider other) {
		// Check if is from red
		if(gameObjects.Contains(other.gameObject) && !objectsInGreen.Contains(other.gameObject)) {
			counting = true;
			// if so adds to green and removes from red
			objectsInGreen.Add(other.gameObject);
			objectsInRed.Remove(other.gameObject);

			// Check if reached end condition
			if(objectsInRed.Count == 0 && objectsInGreen.Count == gameObjects.Count) {
				// Success stop timer!
				finalScore.text = "Time taken:";
				timer.text = count.ToString() + " Sec";
				counting = false;
			}

		}
	}

	void OnTriggerExit(Collider other) {
		if(objectsInGreen.Contains(other.gameObject)) {
			objectsInGreen.Remove(other.gameObject);
		}
	}

	public void restartGame() {
		objectsInGreen = new List<GameObject>();
		objectsInRed = new List<GameObject>();
		counting = false;
		count = 0;
		timer.text = "0";
		finalScore.text = "";

		// Setting all objects to original positions
		for(int i = 0; i < gameObjects.Count; i++) {
			gameObjects[i].transform.position = gameObjectStartPositions[i];
			gameObjects[i].transform.rotation = startRotations[i];
			gameObjects[i].transform.localScale = startSizes[i];
			objectsInRed.Add(gameObjects[i]);
		}
	}
}
