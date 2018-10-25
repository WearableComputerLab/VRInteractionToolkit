using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleHighlightFromBubble : MonoBehaviour {

	public Material highlightMaterial;
	private Material defaultMaterial;

	public BubbleCursor selectObject;

	// Use this for initialization
	void Start () {
		defaultMaterial = this.GetComponent<Renderer>().material;
		selectObject.hovered.AddListener(highlight);
		selectObject.unHovered.AddListener(unHighlight);	
	}

	void highlight() {
		if(selectObject.currentlyHovering == this.gameObject) {
			print("highlight");
			this.GetComponent<Renderer>().material = highlightMaterial;
		} 
	}

	void unHighlight() {
		if(selectObject.currentlyHovering == this.gameObject) {
			this.GetComponent<Renderer>().material = defaultMaterial;
		}
	}
}
