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
using Valve.VR;

[ExecuteInEditMode]
public class BendCastController : MonoBehaviour {
    // Method runs on drag and drop.
    void Awake()
    {
        print("running controller");
        // Controller only ever needs to be setup once
        BendCast cast;
        if((cast = this.GetComponent<BendCast>()) != null) {
            if(cast.leftController == null && cast.rightController == null) {
                // Locates the camera rig and its child controllers
                GameObject leftController = null, rightController = null;
#if SteamVR_Legacy
        // Locates the camera rig and its child controllers
        SteamVR_ControllerManager CameraRigObject = FindObjectOfType<SteamVR_ControllerManager>();
        leftController = CameraRigObject.left;
        rightController = CameraRigObject.right;

        cast.leftController = leftController;
        cast.rightController = rightController;
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
        cast.leftController = leftController;
        cast.rightController = rightController;
#endif
            }
        }
    }
}
