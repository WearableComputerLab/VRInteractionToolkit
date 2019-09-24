using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SizeController : MonoBehaviour {

	public GameObject testObject;

	[Range(0, 1)]
	public float scale = 0.4f;

	[Range(0, 5)]
	public float circleRadius = 1f;

	[Range(4, 50)]
	public int numberOfItems = 4;

	public int currentNumberOfItems = 4;

	public bool forcedZ = true;

	// Use this for initialization
	void Start () {
	}
		
	
	// Update is called once per frame
	void Update () {
		if(Application.isPlaying) {
			return;
		}
		// update if number of items has changed
		if(numberOfItems != currentNumberOfItems) {		
			while(transform.childCount != 0) {
            	DestroyImmediate(transform.GetChild(0).gameObject);
         	}

			for(int itemCount = 0; itemCount < numberOfItems; itemCount++) {
				GameObject item = Instantiate(testObject);
				item.transform.parent = this.transform;
			}
			currentNumberOfItems = numberOfItems;
		}

		int count = 0;
		foreach(Transform each in this.transform) {
			each.localScale = new Vector3(scale, scale, scale);
			each.gameObject.GetComponent<TestObject>().assignedID = count;
			count++;
		}

		// setting items positions
		float angle = 360f/numberOfItems;
		float currentAngle = 0;
		foreach(Transform each in this.transform) {
			float x = this.transform.localPosition.x + (Mathf.Cos(Mathf.Deg2Rad * currentAngle) * circleRadius);
			float y = this.transform.localPosition.y + (Mathf.Sin(Mathf.Deg2Rad * currentAngle) * circleRadius);
			currentAngle += angle;
			each.localPosition = new Vector3(x, y, each.transform.localPosition.z);

			if(forcedZ) {
				each.transform.position = new Vector3(each.position.x, each.position.y, this.transform.position.z);
			} 			
		}
	}
}
