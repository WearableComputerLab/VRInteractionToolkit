using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaledWorldSelectAndSendToController : MonoBehaviour {

	public ControllerColliderSWG selectObject;

	private TesterController controller;

	private GameObject originalParent;

	// Use this for initialization
	void Awake () {
		selectObject.selectedObject.AddListener(tellTesterOfSelection);
		controller = this.GetComponentInParent<TesterController>();
	}

	void Start() {
		originalParent = this.transform.parent.gameObject;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void tellTesterOfSelection() {
		print("trying to select");
		if(selectObject.scaleSelected == this.gameObject) {
			print("success");
			controller.objectSelected(this.gameObject);

			// Due to bubble trying to grab with parent can cancel out with this
			Rigidbody bod;
			if((bod = this.GetComponent<Rigidbody>()) != null) {
				bod.isKinematic = false;
				this.transform.parent = originalParent.transform;
			}
		}	
	}
}
