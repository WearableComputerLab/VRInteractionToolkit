/*
 *  Flashlight - Script to create a cone "flashlight" for the VR
 *  Flaslight technique within Unity for the HTC Vive.
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

// Must be parented to a steam controller to can access controls to change size

public class Flashlight : MonoBehaviour {

#if SteamVR_Legacy
    public SteamVR_TrackedObject trackedObj;
    private SteamVR_Controller.Device device;
#elif SteamVR_2
    public SteamVR_Behaviour_Pose trackedObj;
#else
    public GameObject trackedObj;
#endif

    public GameObject objectAttachedTo;

    public float transparency = 0.5f;
    public Color theColor = new Color(1f, 1f, 1f, 1f);

    public float resizeSpeed = 0.01f;

    // Use this for initialization
    void Start() {
        // Mesh render wasnt enabled outside of game so as not to be annoying. Must now enable
        this.GetComponent<Renderer>().enabled = true;

        // set this flashlight to be child of the object it is set to be attached to
        this.transform.parent = objectAttachedTo.transform;
        // making sure rotation and position is correct
        this.transform.eulerAngles = new Vector3(0, 180, 0);
        this.transform.localPosition = new Vector3(0, 0, 0);

        // Translates the cone so that whatever size it is as long as it is at position 0,0,0 if contoller it will jump to the origin point for flashlight
        translateConeDistanceAlongForward(this.GetComponent<Renderer>().bounds.size.z / 2f);
#if SteamVR_Legacy
        device = SteamVR_Controller.Input((int)trackedObj.index);
#endif
    }

    // Update is called once per frame
    void Update() {
        checkForInput();

        //this.transform.position = objectAttachedTo.transform.position;
        Quaternion rot = objectAttachedTo.transform.rotation;
        //this.transform.rotation = controller.transform.rotation;
        //this.transform.rotation = Quaternion.LookRotation(objectAttachedTo.transform.forward * -1); // Might be able to fix this with initial rotation of cone being changed
    }

    void translateConeDistanceAlongForward(float theDistance) {

        this.transform.position = this.transform.position + trackedObj.transform.forward * theDistance;
    }

    void checkForInput() {
        return;
        //Dont allow size to change if object is in hand - check by getting child object
        /*device = SteamVR_Controller.Input((int)trackedObj.index);
        FlashlightSelection childSelector = this.transform.GetComponent<FlashlightSelection>();
        if (!childSelector.holdingObject())
        {
            Vector2 touchpad = (device.GetAxis(EVRButtonId.k_EButton_Axis0)); // Getting reference to the touchpad


            if (touchpad.y > 0.7f) // Touchpad up
            {
                this.transform.localScale += new Vector3(0f, 0f, resizeSpeed);
                translateConeDistanceAlongForward(resizeSpeed);  // Move to match the resize of cone so that it stays locked to origin position
            }

            else if (touchpad.y < -0.7f) // Touchpad down
            {
                if (this.transform.localScale.z > 0f)
                {
                    this.transform.localScale -= new Vector3(0f, 0f, resizeSpeed);
                    translateConeDistanceAlongForward(resizeSpeed*-1);// Move to match the resize of cone so that it stays locked to origin position
                }
            }
            
            else if (touchpad.x > 0.7f) // Touchpad right 
            {
                    this.transform.localScale += new Vector3(resizeSpeed, resizeSpeed, 0f);
                    //this.transform.localPosition = anchorPoint;
            }
            else if (touchpad.x < -0.7f) // Touchpad left
            {
                if (this.transform.localScale.x > 0f && this.transform.localScale.y > 0f)
                {
                    this.transform.localScale -= new Vector3(resizeSpeed, resizeSpeed, 0f);
                    //this.transform.localPosition = anchorPoint;
                }     
            }
        }*/
    }
}
