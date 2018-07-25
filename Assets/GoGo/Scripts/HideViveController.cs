/*
 *  Script to be attatched to a HTC Vive controller to hide it for a
 *  shadow controller. 
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

public class HideViveController : MonoBehaviour {

    private bool HideTrueController = false;

    // Use this for initialization
    void Start () {

        
    }

    // Update is called once per frame
    void Update () {
        
        //HideController();
    }

    // Hides the controller so we only show the "shadow controller"
    public void HideController()
    {
        //this.GetComponentInChildren<SteamVR_RenderModel>().gameObject.SetActive(false);
        Renderer[] renderers = this.transform.parent.GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            if (renderer.material.name == "Standard (Instance)")
            {
                renderer.enabled = HideTrueController;
            }
        }
    }
}
