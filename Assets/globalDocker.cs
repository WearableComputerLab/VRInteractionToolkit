using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class globalDocker : MonoBehaviour {
	public GameObject[] outlineObjects = new GameObject[4];

    public static int sceneCounter = 0;
	// Use this for initialization
	void Start () {
		sceneCounter++;
		outlineObjects[sceneCounter-1].SetActive(true);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
