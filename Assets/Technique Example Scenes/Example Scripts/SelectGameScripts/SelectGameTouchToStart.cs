using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectGameTouchToStart : MonoBehaviour {

	public TesterController controller;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter(Collider other) {
		if(other.GetComponent<CanResetSigns>() != null) {
			controller.startTest();
		}		
	}
}
