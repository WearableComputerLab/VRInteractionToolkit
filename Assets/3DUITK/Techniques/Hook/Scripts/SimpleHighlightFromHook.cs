using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleHighlightFromHook : MonoBehaviour {

	public Material highlightMaterial;
	private Material defaultMaterial;

	public Hook selectObject;

	// Use this for initialization
	void Start () {
		defaultMaterial = this.GetComponent<Renderer>().material;
		selectObject.hovered.AddListener(highlight);
		selectObject.unHovered.AddListener(unHighlight);	
		selectObject.selectedObject.AddListener(playSelectSound);	
	}

	void highlight() {
		if(selectObject.currentlyHovered == this.gameObject) {
			print("highlight");
			this.GetComponent<Renderer>().material = highlightMaterial;
		} 
	}

	void unHighlight() {
		this.GetComponent<Renderer>().material = defaultMaterial;
			
	}

	void playSelectSound() {
		if(selectObject.currentlyHovered == this.gameObject) {
			this.GetComponent<AudioSource>().Play();
		}	
	}
}
