using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ARMSelectSendToTesterCotroller : MonoBehaviour {

	
	public ARMLaser selectObject;

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
		if(selectObject.lastSelectedObject == this.gameObject) {
			print("success");
			controller.objectSelected(this.gameObject);
		}	
	}
}
