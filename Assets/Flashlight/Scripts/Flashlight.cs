using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

// Must be parented to a steam controller to can access controls to change size

public class Flashlight : MonoBehaviour {

    public GameObject objectAttachedTo;

    private SteamVR_TrackedObject trackedObj;
    private SteamVR_Controller.Device device;

    // Use this for initialization
    void Start () {
        objectAttachedTo = this.transform.parent.gameObject;
        trackedObj = this.GetComponentInParent<SteamVR_TrackedObject>();
        device = SteamVR_Controller.Input((int)trackedObj.index);
    }
	
	// Update is called once per frame
	void Update () {
        checkForInput();

        this.transform.position = objectAttachedTo.transform.position;
        Quaternion rot = objectAttachedTo.transform.rotation;
        //this.transform.rotation = controller.transform.rotation;
        this.transform.rotation = Quaternion.LookRotation(objectAttachedTo.transform.forward * -1); // Might be able to fix this with initial rotation of cone being changed
    }

    void checkForInput()
    {
        Vector2 touchpad = (device.GetAxis(EVRButtonId.k_EButton_Axis0)); // Getting reference to the touchpad
        if (touchpad.y > 0f) // top side of touchpad and pushing down
        {
            this.transform.localScale += new Vector3(0.01f, 0.01f, 0.01f);
        }
        else if (touchpad.y < 0f) // bottom side of touchpad and pushing down
        {
            if(this.transform.localScale.x > 0f)
            {
                this.transform.localScale -= new Vector3(0.01f, 0.01f, 0.01f);
            }
            
        }
    }
}
