using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedGreenSquareRestarter : MonoBehaviour {

	public RedGreenSquareGame theGame;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter(Collider other) {
		if(other.GetComponent<CanResetSigns>() != null) {
			theGame.restartGame();
		}
	}
}
