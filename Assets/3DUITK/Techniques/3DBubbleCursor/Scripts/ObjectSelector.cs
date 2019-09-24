using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

[RequireComponent(typeof(SteamVR_TrackedObject))]
public class ObjectSelector : MonoBehaviour {
    /*
    SteamVR_TrackedObject trackedObj;
    SteamVR_Controller.Device device;

    private void OnTriggerStay(Collider col) {
        Debug.Log("You have collided with " + col.name + " and activated OnTriggerStay");
        if (device.GetTouch(SteamVR_Controller.ButtonMask.Trigger)) {
            Debug.Log("You have collided with " + col.name + " while holding down Touch");
            col.attachedRigidbody.isKinematic = true;
            col.gameObject.transform.SetParent(this.gameObject.transform);
        }
        if (device.GetTouchUp(SteamVR_Controller.ButtonMask.Trigger)) {
            Debug.Log("You have released Touch while colliding with " + col.name);
            col.gameObject.transform.SetParent(null);
            col.attachedRigidbody.isKinematic = false;
        }
            
    }

    void Awake () {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
	}
	
	void FixedUpdate () {
        device = SteamVR_Controller.Input((int)trackedObj.index);
        /*if(device.GetTouch(SteamVR_Controller.ButtonMask.Trigger)) {
            Debug.Log("You have activated 'touch' on the Trigger");
        }*/
        /*if (device.GetTouchDown(SteamVR_Controller.ButtonMask.Trigger)) {
            Debug.Log("You have activated TouchDown on the Trigger");
            
        }
        if (device.GetTouchUp(SteamVR_Controller.ButtonMask.Trigger)) {
            Debug.Log("You have activated TouchUp on the Trigger");
        }*/

        //Press is before the click
        /*if (device.GetPress(SteamVR_Controller.ButtonMask.Trigger)) {
            Debug.Log("You have activated 'press' on the Trigger");
        }
        if (device.GetPressDown(SteamVR_Controller.ButtonMask.Trigger)) {
            Debug.Log("You have activated pressDown on the Trigger");
        }
        if (device.GetPressUp(SteamVR_Controller.ButtonMask.Trigger)) {
            Debug.Log("You have activated PressUp on the Trigger");
        }*/
    //}
}
