using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dock : MonoBehaviour {

	public GameObject dockObject;

	public Material successMaterial;

	private Material defaultMaterial;

	private Vector3 dockObjectOriginalPos;
	private Vector3 dockObjectOriginalRot;

	public float posSpread = 0.1f;
	public float rotSpread = 6f;

	private bool checking = false;

	// Use this for initialization
	void Start () {
		dockObjectOriginalPos = dockObject.transform.position;
		dockObjectOriginalRot = dockObject.transform.eulerAngles;
		defaultMaterial = dockObject.GetComponent<Renderer>().material;
	}
	
	// Update is called once per frame
	void Update () {
		if(checking) {
			if(checkWithinSpread()) {
				// IT FITS DO SOMETHING
				dockObject.GetComponent<Rigidbody>().isKinematic = true;
				dockObject.GetComponent<Renderer>().material = successMaterial;
			}
		}
	}

	public void resetGame() {
		dockObject.transform.position = dockObjectOriginalPos;
		dockObject.transform.eulerAngles = dockObjectOriginalRot;
		dockObject.GetComponent<Renderer>().material = defaultMaterial;
		dockObject.GetComponent<Rigidbody>().velocity = new Vector3(0,0,0);
		dockObject.GetComponent<Rigidbody>().angularVelocity = new Vector3(0,0,0);
		dockObject.GetComponent<Rigidbody>().isKinematic = false;
	}

	void OnTriggerEnter(Collider other) {
		if(other.gameObject == dockObject) {
			checking = true;
		}
	}

	void OnTriggerExit(Collider other) {
		if(other.gameObject == dockObject) {
			checking = false;
		}
	}

	bool checkWithinSpread() {
		//print("checking");
		// check if matches
		Vector3 otherPos = dockObject.gameObject.transform.position;
		Vector3 otherRot = dockObject.gameObject.transform.eulerAngles;
		Vector3 thisPos = this.transform.position;
		Vector3 thisRot = this.transform.eulerAngles;

		
		// if within 0.1 for positon
		// if within 6 of rotation
		float xPosMin = thisPos.x - posSpread;
		float xPosMax = thisPos.x + posSpread;
		float yPosMin = thisPos.y - posSpread;
		float yPosMax = thisPos.y + posSpread;
		float zPosMin = thisPos.z - posSpread;
		float zPosMax = thisPos.z + posSpread;

		float xRotMin = thisRot.x - rotSpread;
		float xRotMax = thisRot.x + rotSpread;
		float yRotMin = thisRot.y - rotSpread;
		float yRotMax = thisRot.y + rotSpread;
		float zRotMin = thisRot.z - rotSpread;
		float zRotMax = thisRot.z + rotSpread;

		//print("xposMin " + xPosMin);
		//print("xPosMax " + xPosMax);
		//print("xrotMin " + xRotMin);
		//print("XrtMax" + xRotMax);
		// Checking within all spreads
		if(otherPos.x > xPosMin && otherPos.x < xPosMax && otherPos.y > yPosMin 
			&& otherPos.y < yPosMax && otherPos.z > zPosMin && otherPos.z < zPosMax
				&& otherRot.z > xRotMin && otherRot.x < xRotMax && otherRot.y > yRotMin && otherRot.y < yRotMax
					&& otherRot.z > zRotMin && otherRot.z < zRotMax) {
						return true;
		} 
		return false;
	}
 }
