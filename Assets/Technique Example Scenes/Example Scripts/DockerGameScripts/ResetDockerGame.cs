using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetDockerGame : MonoBehaviour {

	public Dock[] theDocks;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter(Collider other) {
		if(other.GetComponent<CanResetSigns>() != null) {
			restart();
		}
	}

	void restart() {
		foreach (Dock dock in theDocks) {
			dock.resetGame();
			print("hello");
		}
	}
}
