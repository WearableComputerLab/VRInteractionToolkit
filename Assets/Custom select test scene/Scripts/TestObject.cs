using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestObject : MonoBehaviour {

	public int assignedID;

	private int indexOfPositionMovingTowards = 0;

	public Vector3[] positionsToMoveTo = null;

	public float speed;

 	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
 		if(positionsToMoveTo != null) {
			 // start moveTowards
			if(positionsToMoveTo[indexOfPositionMovingTowards] == this.transform.position) {
				// Need to set new position
				indexOfPositionMovingTowards = (indexOfPositionMovingTowards == positionsToMoveTo.Length-1) ? 0 : indexOfPositionMovingTowards+1; 
			} 
			// keep moving towards
			transform.position = Vector3.MoveTowards(transform.position, positionsToMoveTo[indexOfPositionMovingTowards], speed*Time.deltaTime);		
		 }
	}
}
