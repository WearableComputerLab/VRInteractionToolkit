/*
 *  SimpleHighlightFromBendCast - Script that can be attached to an object displaying how to utilize the bend cast controllers events to react externally when an object
 *                                is highlighted or selected.
 *  
 *  Copyright(C) 2018  Ian Hanan
 *
 *  This program is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 * 
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with this program.If not, see<http://www.gnu.org/licenses/>.

 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleHighlightFromBendcast : MonoBehaviour {

	public Material highlightMaterial;
	private Material defaultMaterial;

	public BendCast selectObject;

	// Use this for initialization
	void Start () {
		defaultMaterial = this.GetComponent<Renderer>().material;
		selectObject.hovered.AddListener(highlight);
		selectObject.unHovered.AddListener(unHighlight);	
		selectObject.selectedObject.AddListener(playSelectSound);	
	}

	void highlight() {
		if(selectObject.currentlyPointingAt == this.gameObject) {
			print("highlight");
			this.GetComponent<Renderer>().material = highlightMaterial;
		} 
	}

	void unHighlight() {
		print("unhighlight");
		this.GetComponent<Renderer>().material = defaultMaterial;
			
	}

	void playSelectSound() {
		if(selectObject.currentlyPointingAt == this.gameObject) {
			this.GetComponent<AudioSource>().Play();
		}	
	}
}
