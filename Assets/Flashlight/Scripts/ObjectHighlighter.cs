using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectHighlighter : MonoBehaviour {
	private Material highlight;

	GameObject currentlyHighlighted;

	public ObjectHighlighter(Material highlight) {
		this.highlight = highlight;
	}

	public void deHighlighobject(GameObject theObject) {
		if(theObject.GetComponent<HighlightableObject>() != null) {
			theObject.GetComponent<HighlightableObject>().deHighlightObject();
		}
	}

	public void deHighlightCurrentObject() {
		if(currentlyHighlighted != null && currentlyHighlighted.GetComponent<HighlightableObject>() != null) {
			currentlyHighlighted.GetComponent<HighlightableObject>().deHighlightObject();
		}
	}

	public void highlightObject(GameObject theObject) {
		deHighlightCurrentObject();
		currentlyHighlighted = theObject;
		if(theObject.GetComponent<HighlightableObject>() == null) {
			theObject.AddComponent<HighlightableObject>();
			theObject.GetComponent<HighlightableObject>().setUpComponent(highlight);
		}
		theObject.GetComponent<HighlightableObject>().highlightObject();
	}

	private class HighlightableObject: MonoBehaviour {

		private Material original;
		private Material highlight;

		private GameObject theObject;

		public HighlightableObject() {			
			
		}

		public void setUpComponent(Material highlight) {
			this.highlight = highlight;
			original = this.GetComponent<Renderer>().material;
		}

		public void highlightObject() {
			
			this.GetComponent<Renderer>().material = highlight;
		}

		public void deHighlightObject() {
			this.GetComponent<Renderer>().material = original;
		}
	}	
}
