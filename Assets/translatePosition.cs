using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class translatePosition : MonoBehaviour {

	private WorldInMiniature worldInMin;
	private GameObject clonedObject;
	public static int globalID;
	public string ID;

	void Awake() {
		worldInMin = GameObject.Find("WorldInMiniature_Technique").GetComponent<WorldInMiniature>();
	}

	// Use this for initialization
	void Start () {
		globalID++;
		ID = "WIM_ID:"+globalID;
	}

	private List<GameObject> listOfChildren = new List<GameObject>();
	private void findClonedObject(GameObject obj){
		if (null == obj)
			return;
    foreach (Transform child in obj.transform){
        if (null == child)
            continue;
		print(child.gameObject.name);
        listOfChildren.Add(child.gameObject);
        findClonedObject(child.gameObject);
		if (this.transform.name == child.gameObject.name) { //Comparison..
			clonedObject = child.gameObject;
			Destroy(clonedObject.GetComponent<translatePosition>());
			clonedObject.GetComponent<Rigidbody>().isKinematic = true;
			print("Found cloned object:"+child.gameObject.name);
			//print("Comparing IDs:"+obj.GetHashCode()+ ","+child.gameObject.
			listOfChildren.Clear();
		}
    }
}
	public void translateObject() {
		if (clonedObject == null) {
			findClonedObject(worldInMin.worldInMinParent);
		}
		if (isMoving() && worldInMin.WiMactive == true && clonedObject != null && worldInMin.currentObjectCollided != clonedObject) {
			//clonedObject.transform.position = new Vector3(this.transform.position.x/worldInMin.scaleAmount, this.transform.position.y/worldInMin.scaleAmount, this.transform.position.z/worldInMin.scaleAmount);
			clonedObject.transform.localPosition = this.transform.localPosition;
			//print("is moving.. new pos:"+clonedObject.transform.position);
		} 
	}

	private bool isMoving() {
		return !this.transform.GetComponent<Rigidbody>().IsSleeping();
	}
	
	private float timer;
	private void moveObject() {
		timer += Time.deltaTime;
	}

	// Update is called once per frame
	void Update () {
		//print(this.transform.GetComponent<Rigidbody>().IsSleeping());
		if (worldInMin.WiMactive == true) {
			translateObject();
		}
	}
}
