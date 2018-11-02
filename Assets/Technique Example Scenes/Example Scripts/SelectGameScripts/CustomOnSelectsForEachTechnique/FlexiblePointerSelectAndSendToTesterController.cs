using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlexiblePointerSelectAndSendToTesterController : MonoBehaviour {

	public FlexiblePointer selectObject;

	private TesterController controller;

	// Use this for initialization
	void Start () {
		selectObject.selectedObject.AddListener(tellTesterOfSelection);
		controller = this.GetComponentInParent<TesterController>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void tellTesterOfSelection() {
		print("trying to select");
		if(selectObject.selection == this.gameObject) {
			print("success");
			controller.objectSelected(this.gameObject);
		}	
	}
}
