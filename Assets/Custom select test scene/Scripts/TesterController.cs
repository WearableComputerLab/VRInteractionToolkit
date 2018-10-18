using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TesterController : MonoBehaviour {

	public GameObject theSelectionGameObject;

	private List<GameObject> testobjects;

	private float endTime;

	public TextMesh timerText;

	public TextMesh scoreText;

	public int score = 0;

	private GameObject goal;  // object must select

	public Material goalDefaultMaterial; // All goals must be colour;

	public Material goalHighlightMaterial; // material that highlights current goal

	private bool testRunning = false; // Tracking if test is running

	// Use this for initialization
	void Start () {
		BendCast theSelectionComponent = theSelectionGameObject.GetComponent<BendCast>();
		theSelectionComponent.selectedObject.AddListener(objectSelected);

		testobjects = new List<GameObject>();
		foreach(Transform each in this.transform) {
			testobjects.Add(each.gameObject);
		}
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

	void startTest() {
		testRunning = true;

		// Start visual countdown timer
		endTime = Time.time + 60;
		timerText.text = "60";

		// Highlight first item
		// get a index between 0 and length of objects so can choose randomly to highlight
		int number = Random.Range(0, testobjects.Count-1);
		goal = testobjects[number];

		goal.GetComponent<Renderer>().material = goalHighlightMaterial;
	}

	void objectSelected() {
		if(theSelectionGameObject.GetComponent<BendCast>().selection.Equals(goal)) {
			goal.GetComponent<Renderer>().material = goalDefaultMaterial;
			//theSelectionGameObject.GetComponent<BendCast>().unhighlightedObject = goalDefaultMaterial;
			goal = null;
			// Increase score by 1
			score += 1;
			scoreText.text = "Score: " + score.ToString();
		}	
	}
}
