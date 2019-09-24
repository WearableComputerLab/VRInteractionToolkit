using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectID : MonoBehaviour {

	public int ID;
    public bool movableObject;
    public GameObject clonedObject;

    public bool isMoving() {
        if(this.gameObject != null && this.transform.GetComponent<Rigidbody>() != null) {
            return !this.transform.GetComponent<Rigidbody>().IsSleeping();
        }
        return false;
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (isMoving() && clonedObject != null) {
            clonedObject.transform.localPosition = this.transform.localPosition;
            clonedObject.transform.localEulerAngles = this.transform.localEulerAngles;
        }
	}
}
