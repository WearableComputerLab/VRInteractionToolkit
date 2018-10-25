using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CountDistance : MonoBehaviour {

	public bool counting = false;

	public Vector3 lastLocation;

	public float totalDistance = 0f;
    public float countTime = 0f;

	// Use this for initialization
	void Start () {
		lastLocation = this.transform.position;
	}

	public void addDistance() {
		totalDistance += Vector3.Distance(this.transform.position, lastLocation);
		lastLocation = this.transform.position;
	}

    public void resetDistance() {
        totalDistance = 0;
    }
	
	// Update is called once per frame
	void Update () {
		if (counting) {
            countTime += Time.deltaTime;
            print(countTime);
            addDistance ();
		}
	}
}
