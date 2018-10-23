using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerCollider : MonoBehaviour {

    private ScaledWorldGrab scaledWorldGrab;

    private void OnTriggerStay(Collider col) {
        if (scaledWorldGrab.objSelected == true) {
            if (scaledWorldGrab.controller.GetPressDown(SteamVR_Controller.ButtonMask.Trigger)) {
                if (scaledWorldGrab.interacionType == ScaledWorldGrab.InteractionType.Manipulation_Full || scaledWorldGrab.interacionType == ScaledWorldGrab.InteractionType.Manipulation_Movement) {
                    col.gameObject.transform.SetParent(scaledWorldGrab.trackedObj.gameObject.transform);
                    scaledWorldGrab.objectGrabbed = true;
                } else if (scaledWorldGrab.interacionType == ScaledWorldGrab.InteractionType.Selection) {
                    scaledWorldGrab.tempObjectStored = col.gameObject;
                    print("Selected object in pure selection mode:" + col.gameObject.name);
                    return;
                }
            }
            if (scaledWorldGrab.controller.GetPressUp(SteamVR_Controller.ButtonMask.Trigger) && scaledWorldGrab.objectGrabbed == true) {
                col.gameObject.transform.SetParent(null);
                scaledWorldGrab.objectGrabbed = false;
                scaledWorldGrab.resetProperties();
            }
        }
    }

    // Use this for initialization
    void Start () {
        scaledWorldGrab = GameObject.Find("ScaledWorldGrab_Technique").GetComponent<ScaledWorldGrab>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
