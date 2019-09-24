using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Valve.VR;

public class SimpleVirtualHand : MonoBehaviour {

    /* Simple Virtual Hand implementation by Kieran May
     * University of South Australia
     * 
     *  Copyright(C) 2019 Kieran May
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
	 
#if SteamVR_Legacy
    internal SteamVR_TrackedObject trackedObj;
    internal SteamVR_Controller.Device controller;
#elif SteamVR_2
    internal SteamVR_Behaviour_Pose trackedObj;
    public SteamVR_Action_Boolean m_controllerPress;
#else
    public GameObject trackedObj;
#endif


    public GameObject controllerCollider;
    public LayerMask interactionLayers;

    public GameObject controllerRight;
    public GameObject controllerLeft;

    public enum InteractionType { Selection, Manipulation_Movement};
    public InteractionType interacionType;

    public enum ControllerPicked { Left_Controller, Right_Controller };
    public ControllerPicked controllerPicked;

    internal bool objSelected = false;
    public GameObject selectedObject;

    Vector3 oldHeadScale;
    Vector3 oldCameraRigScale;

    internal bool objectGrabbed = false;
    public static int grabbedAmount;

    private GameObject entered;
    private void OnTriggerEnter(Collider col) {
        if(isInteractionlayer(col.gameObject)) {
            print("Entered" + col.name);
            entered = col.gameObject;
        }
    }


    private bool isInteractionlayer(GameObject obj) {
        return interactionLayers == (interactionLayers | (1 << obj.layer));
    }

    private void initializeControllers() {
        if (controllerPicked == ControllerPicked.Right_Controller) {
#if SteamVR_Legacy
            trackedObj = controllerRight.GetComponent<SteamVR_TrackedObject>();
#elif SteamVR_2
            trackedObj = controllerRight.GetComponent<SteamVR_Behaviour_Pose>();
#endif
        } else if (controllerPicked == ControllerPicked.Left_Controller) {
#if SteamVR_Legacy
            trackedObj = controllerLeft.GetComponent<SteamVR_TrackedObject>();
#elif SteamVR_2
            trackedObj = controllerLeft.GetComponent<SteamVR_Behaviour_Pose>();
#endif
        } else {
            print("Couldn't detect trackedObject, please specify the controller type in the settings.");
            Application.Quit();
        }

    }

    void Awake() {
        initializeControllers();
        controllerCollider.transform.parent = trackedObj.transform;
    }

    // Update is called once per frame
    void Update() {
        #if SteamVR_Legacy
        controller = SteamVR_Controller.Input((int)trackedObj.index);
#endif
    }
}
