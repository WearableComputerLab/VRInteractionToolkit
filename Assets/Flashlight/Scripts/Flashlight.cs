﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

// Must be parented to a steam controller to can access controls to change size

public class Flashlight : MonoBehaviour
{

    public GameObject objectAttachedTo;

    private SteamVR_TrackedObject trackedObj;
    private SteamVR_Controller.Device device;

    public float transparency = 0.5f;
    public Color theColor = new Color(1f, 1f, 1f, 1f);

    public float resizeSpeed = 0.01f;

    // Use this for initialization
    void Start()
    {
        objectAttachedTo = this.transform.parent.gameObject;
        trackedObj = this.GetComponentInParent<SteamVR_TrackedObject>();
        device = SteamVR_Controller.Input((int)trackedObj.index);

        //GameObject flashLightModel = this.transform.GetChild(0).gameObject;
        //flashLightModel.GetComponent<MeshRenderer>().material.color = new Color(theColor.r, theColor.b, theColor.g, transparency);

        this.GetComponent<MeshRenderer>().material.color = new Color(theColor.r, theColor.b, theColor.g, transparency);
    }

    // Update is called once per frame
    void Update()
    {
        checkForInput();

        this.transform.position = objectAttachedTo.transform.position;
        Quaternion rot = objectAttachedTo.transform.rotation;
        //this.transform.rotation = controller.transform.rotation;
        //this.transform.rotation = Quaternion.LookRotation(objectAttachedTo.transform.forward * -1); // Might be able to fix this with initial rotation of cone being changed
    }

    void checkForInput()
    {
        //Dont allow size to change if object is in hand - check by getting child object

        FlashlightSelection childSelector = this.transform.GetComponent<FlashlightSelection>();
        if (!childSelector.holdingObject())
        {
            Vector2 touchpad = (device.GetAxis(EVRButtonId.k_EButton_Axis0)); // Getting reference to the touchpad


            if (touchpad.y > 0.7f) // Touchpad up
            {
                this.transform.localScale += new Vector3(0f, 0f, resizeSpeed);
            }

            else if (touchpad.y < -0.7f) // Touchpad down
            {
                if (this.transform.localScale.z > 0f)
                {
                    this.transform.localScale -= new Vector3(0f, 0f, resizeSpeed);
                }
            }
            
            else if (touchpad.x > 0.7f) // Touchpad right 
            {
                    this.transform.localScale += new Vector3(resizeSpeed, resizeSpeed, 0f);
            }
            else if (touchpad.x < -0.7f) // Touchpad left
            {
                if (this.transform.localScale.x > 0f && this.transform.localScale.y > 0f)
                {
                    this.transform.localScale -= new Vector3(resizeSpeed, resizeSpeed, 0f);
                }     
            }
        }
    }
}