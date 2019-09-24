using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// keeping track of time passed resets every 500ms
	float timePassedTracker;
	float millisecondDelayTime = 500;

	// Update is called once per frame
	void Update () {

		if(timePassedTracker >= millisecondDelayTime) {
			// do stuff and reset
			print(timePassedTracker);
			timePassedTracker = 0;
		} 
		timePassedTracker = timePassedTracker += Time.deltaTime*1000f;
	}
}
