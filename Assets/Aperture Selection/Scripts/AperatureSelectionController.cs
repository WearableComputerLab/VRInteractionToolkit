/*
 *  AperatureSelectionController Controller - This controller class gets attatched to a prefab.
 *  
 *  This method allows a user to drag the bend cast prefab into the scene and immediately use it.
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

[ExecuteInEditMode]
public class AperatureSelectionController : MonoBehaviour {

	public GameObject controller;
	public GameObject aperatureVolume;
	
	// Use this for initialization
	void Awake() {
		AperatureSelection selector;
		if((selector = this.GetComponent<AperatureSelection>()) != null) {
			if(selector.controllerTrackedObj == null || selector.headsetTrackedObj == null) {
				// Locates the camera rig and its child controllers
				SteamVR_ControllerManager CameraRigObject = FindObjectOfType<SteamVR_ControllerManager>();
				GameObject rightController = CameraRigObject.right;
				
				GameObject eye = FindObjectOfType<SteamVR_Camera>().gameObject;
				GameObject head = eye.transform.parent.gameObject;

				controller = rightController.gameObject;
				selector.controllerTrackedObj = rightController.GetComponent<SteamVR_TrackedObject>();
				selector.headsetTrackedObj = head.GetComponent<SteamVR_TrackedObject>();

				AperatureSelectionSelector objectSelection;
				if((objectSelection = aperatureVolume.GetComponent<AperatureSelectionSelector>()) != null) {
					objectSelection.theController = rightController.GetComponent<SteamVR_TrackedObject>();
				}
			}
		}
	}
}
