/*
 *  ARM Controller - This controller class gets attatched to a prefab.
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
using Valve.VR;

[ExecuteInEditMode]
public class ARMController : MonoBehaviour {

    void Awake()
    {

        // Controller only ever needs to be setup once
        ARMLaser test = GetComponent<ARMLaser>();
        if(test != null) {
            return;
        }

        // Need:
        // Controller
        // Controller model to shadow
        // Material to highlight objects
        // Reference to shadow objects (children of this object)

        // Locates the camera rig and its child controllers
        GameObject leftController = null, rightController = null;
#if SteamVR_Legacy
        // Locates the camera rig and its child controllers
        SteamVR_ControllerManager CameraRigObject = FindObjectOfType<SteamVR_ControllerManager>();
        leftController = CameraRigObject.left;
        rightController = CameraRigObject.right;
#elif SteamVR_2
        SteamVR_Behaviour_Pose[] controllers = FindObjectsOfType<SteamVR_Behaviour_Pose>();
        if (controllers.Length > 1) {
            leftController = controllers[0].inputSource.ToString() == "LeftHand" ? controllers[0].gameObject : controllers[1].inputSource.ToString() == "LeftHand" ? controllers[1].gameObject : null;
            rightController = controllers[0].inputSource.ToString() == "RightHand" ? controllers[0].gameObject : controllers[1].inputSource.ToString() == "RightHand" ? controllers[1].gameObject : null;
        } else if (controllers.Length == 1) {
            leftController = controllers[0].inputSource.ToString() == "LeftHand" ? controllers[0].gameObject : null;
            rightController = controllers[0].inputSource.ToString() == "RightHand" ? controllers[0].gameObject : null;
        } else {
            return;
        }

#endif
        // Get child shadow controllers and set their component info (if corresponding controllers exist)
        foreach (Transform child in transform)
        {
            if(child.name == "LeftHand" && leftController != null)
            {
                setARMinfo(leftController, child.gameObject);
            }
            else if (child.name == "RightHand" && rightController != null)
            {
                setARMinfo(rightController, child.gameObject);
            }
        }
    }

    private void setARMinfo(GameObject controller, GameObject shadowObject)
    {
        ARMLaser component = shadowObject.GetComponent<ARMLaser>();
        component.theController = controller;
        component.theModel = controller.GetComponentInChildren<SteamVR_RenderModel>().gameObject;
    } 
}
