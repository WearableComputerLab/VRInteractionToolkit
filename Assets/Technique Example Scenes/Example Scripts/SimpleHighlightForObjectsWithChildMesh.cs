using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleHighlightForObjectsWithChildMesh : MonoBehaviour {

		
	public Material highlightMaterial;
	private Material defaultMaterial;

	public GrabObject selectObject;

	// Use this for initialization
	void Start () {

		// Find a existing default material (this will only work if they all use the same texture)
		foreach(Transform obj in this.transform) {
			Renderer render;
			if((render = obj.GetComponent<Renderer>()) != null) {
				defaultMaterial = render.material;
				break;
			}
		}

		selectObject.hovered.AddListener(highlight);
		selectObject.unHovered.AddListener(unHighlight);	
		selectObject.selectedObject.AddListener(playSelectSound);	
	}

	void highlight() {
		if(selectObject.collidingObject == this.gameObject && selectObject.selection == null) {
			print("highlight");
			foreach(Transform obj in this.transform) {
				Renderer render;
				if((render = obj.GetComponent<Renderer>()) != null) {
					render.material = highlightMaterial;
				}
			}
		}		
	}

	void unHighlight() {
		if(selectObject.collidingObject == this.gameObject) {
			print("unhighlight");
			foreach(Transform obj in this.transform) {
				Renderer render;
				if((render = obj.GetComponent<Renderer>()) != null) {
					render.material = defaultMaterial;
				}
			}
		}		
	}

	void playSelectSound() {
		if(selectObject.collidingObject == this.gameObject) {
			this.GetComponent<AudioSource>().Play();
		}	
	}

}
