using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TesterController : MonoBehaviour {

	private List<GameObject> testobjects;

	private float endTime;

	public TextMesh timerText;

	public TextMesh scoreText;

	public int score = 0;

	private GameObject goal;  // object must select

	public Material goalDefaultMaterial; // All goals must be colour;

	public Material goalHighlightMaterial; // material that highlights current goal

	private bool testRunning = false; // Tracking if test is running

	public int testTimer = 20;

	// Use this for initialization
	void Start () {
		testobjects = new List<GameObject>();
		foreach(Transform each in this.transform) {
			testobjects.Add(each.gameObject);
		}
		disableAllTestObjectComponentsNotMesh ();
	}
	
	// Update is called once per frame
	void Update () {

		// Test starts if not already running and spacebar on keyboard is pressed
		if (Input.GetKeyDown("space") && !testRunning)
        {
            startTest();
        }

		// Test stuff
		if(testRunning) {
			int timeLeft = (int)(endTime - Time.time);
			if(timeLeft < 0) timeLeft = 0;
			timerText.text = timeLeft.ToString();

			if( timeLeft == 0) {
				// If time hits 0 finished so do calculations or w/e and set running to false
				testRunning = false;
				if(goal != null) {
					goal.GetComponent<Renderer>().material = goalDefaultMaterial;
				}
				// Disables all components except for meshrenderer
				disableAllTestObjectComponentsNotMesh();

			} else {
				// do test stuff
				// must select new goal
				if(goal == null) {
					// get a index between 0 and length of objects so can choose randomly to highlight
					int number = Random.Range(0, testobjects.Count-1);
					goal = testobjects[number];

					goal.GetComponent<Renderer>().material = goalHighlightMaterial;

				}
			}
		}
	}

	public void disableAllTestObjectComponentsNotMesh() {
		foreach (Transform child in this.transform) {
			// Disables all test object components except for meshrenderer
			MonoBehaviour[] comps = child.GetComponents<MonoBehaviour> ();
			foreach (MonoBehaviour c in comps) {
				c.enabled = false;
			}
			child.GetComponent<Renderer> ().enabled = true;
		}
	}

	public void enableAllTestObjectComponents() {
		foreach (Transform child in this.transform) {
			// Disables all test object components except for meshrenderer
			MonoBehaviour[] comps = child.GetComponents<MonoBehaviour> ();
			foreach (MonoBehaviour c in comps) {
				c.enabled = true;
			}
		}
	}

	public void startTest() {
		if(!testRunning) {
			testRunning = true;

			// Enables all components in test objects
			enableAllTestObjectComponents();


			// reset score
			score = 0;
			scoreText.text = "Score: " + score.ToString();	
			// Start visual countdown timer
			endTime = Time.time + testTimer;
			timerText.text = testTimer.ToString();

			// Highlight first item
			// get a index between 0 and length of objects so can choose randomly to highlight
			int number = Random.Range(0, testobjects.Count-1);
			goal = testobjects[number];

			goal.GetComponent<Renderer>().material = goalHighlightMaterial;
		}		
	}

	public void objectSelected(GameObject theobject) {
		if(goal == theobject) {
			goal.GetComponent<Renderer>().material = goalDefaultMaterial;
			goal = null;
			// Increase score by 1
			score += 1;
			scoreText.text = "Score: " + score.ToString();
		}		
	}
}
