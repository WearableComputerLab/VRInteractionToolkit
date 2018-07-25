/*
 *  Bend Cast Controller - This controller class gets attatched to a prefab.
 *  When the prefab is dragged into the unity scene it will locate the Camera-Rig and then
 *  locate active vive controllers. Next, it will attatch a BendCast script component to itself 
 *  for each controller and automatically add reference to the controllers to them.
 *  Public properties for these BencCast components can then be adjusted if required.
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
public class BendCastController : MonoBehaviour {
    public enum InteractionType { Selection, Manipulation };
    public InteractionType interactionType;
    public GameObject laserPrefab;
    public Material MaterialToHighlightObjects;

    // Method runs on drag and drop.
    void Awake()
    {
        // Controller only ever needs to be setup once
        BendCast test = GetComponent<BendCast>();
        if(test != null) {
            return;
        }

        // Locates the camera rig and its child controllers
        SteamVR_ControllerManager CameraRigObject = FindObjectOfType<SteamVR_ControllerManager>();
        GameObject leftController = CameraRigObject.left;
        GameObject rightController = CameraRigObject.right;

        if(leftController != null)
        {
            // Creates left component
            BendCast componentLeft = gameObject.AddComponent<BendCast>() as BendCast;
            attatchComponents(componentLeft, leftController, "Right");
            
        } 
        else
        {
            Debug.Log("No left controller found. Did not attach Bend cast for left controller.");
        }

        if(rightController != null)
        {
            // Creates right component
            BendCast componentRight = gameObject.AddComponent<BendCast>() as BendCast;
            attatchComponents(componentRight, rightController, "Left");
        }
        else
        {
            Debug.Log("No right controller found. Did not attach Bend cast for right controller");
        }
    }

    // Adds reference the tracked object of the component, applies the relevant controller namem
    // Sets the initial layers of objects that the cast will bend to. (Can be adjusted as required by the user)
    // Sets the initial laser, and material prefab to the ones set to this controller.
    private void attatchComponents(BendCast component, GameObject controller, string name)
    {
        component.trackedObj = controller.GetComponent<SteamVR_TrackedObject>();
        component.controllerName = name + " Controller";
        component.layersOfObjectsToBendTo = new int[1];
        component.layersOfObjectsToBendTo[0] = 8;
        component.laserPrefab = laserPrefab;
        component.MaterialToHighlightObjects = MaterialToHighlightObjects;
        Debug.Log("Bend Cast attatched for" + name + "controller.");
    }
}
