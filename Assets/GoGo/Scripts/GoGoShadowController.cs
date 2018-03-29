using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class GoGoShadowController : MonoBehaviour {

    public GameObject theController;

    public SteamVR_TrackedObject trackedObj;
    private Hand refHand;

    private float distanceToExtend = 0f;
    
    private SteamVR_Controller.Device device;
    private Vector3 vector_from_device;
    private Vector3 device_origin;
    private bool remote_go_go = false;
    bool extendingForward = true; // If not in extendingmode the arm will retract
    bool extending = false;
    float extensionSpeed = 0.02f;
    bool ranAlready = false;
    

    // Use this for initialization
    void Start () {
        //trackedObj = this.GetComponent<SteamVR_TrackedObject>();
        refHand = this.GetComponent<Hand>();
        print(refHand.enabled);
    }
	
	// Update is called once per frame
	void Update () {

        //this.GetComponentInChildren<SteamVR_RenderModel>().gameObject.SetActive(false);
        Renderer[] renderers = this.transform.parent.GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            if (renderer.material.name == "Standard (Instance)")
            {
                renderer.enabled = true;
            }
        }
        if(refHand.controller != null) // Quick fix need a better way to wait until controller has been assigned
        {
            checkForAction();
            moveControllerForward();
        }
        
    }

    void moveControllerForward()
    {
        if (extending)
        {
            if (extendingForward)
            {
                increaseDistanceToExtend();
            }
            else
            {
                decreaseDistanceToExtend();
            }
        }

        // Using the origin and the forward vector of the remote the extended positon of the remote can be calculated
        Vector3 theVector = theController.transform.forward;
        Vector3 pose = theController.transform.position;

        float distance_formula_on_vector = Mathf.Sqrt(theVector.x * theVector.x + theVector.y * theVector.y + theVector.z * theVector.z);
        // Using formula to find a point which lies at distance on a 3D line from vector and direction
        pose.x = pose.x + (distanceToExtend / (distance_formula_on_vector)) * theVector.x;
        pose.y = pose.y + (distanceToExtend / (distance_formula_on_vector)) * theVector.y;
        pose.z = pose.z + (distanceToExtend / (distance_formula_on_vector)) * theVector.z;

        transform.position = pose;
        transform.rotation = theController.transform.rotation;
    }

    void checkForAction()
    {
        device = refHand.controller;

        if (!ranAlready)
        {
            //device = SteamVR_Controller.Input((int)trackedObj.index);
            ranAlready = true;
        }
        if (device.GetPressDown(SteamVR_Controller.ButtonMask.Grip))
        {
            Debug.Log("Trigger Press");
            device.TriggerHapticPulse(1000);
        }
        Vector2 touchpad = (device.GetAxis(EVRButtonId.k_EButton_Axis0)); // Getting reference to the touchpad
        if (touchpad.y > 0.7f && device.GetPressDown(SteamVR_Controller.ButtonMask.Axis0)) // top side of touchpad and pushing down
        {
            extending = true;
            extendingForward = true;
        }
        else if (touchpad.y < -0.7f && device.GetPressDown(SteamVR_Controller.ButtonMask.Axis0)) // bottom side of touchpad and pushing down
        {
            extending = true;
            extendingForward = false;
        }
        else if (device.GetPressUp(SteamVR_Controller.ButtonMask.Axis0))
        {
            extending = false;
        }
    }
    public void increaseDistanceToExtend()
    {
        distanceToExtend += extensionSpeed;
    }

    public void decreaseDistanceToExtend()
    {
        if (distanceToExtend > 0)
        {
            distanceToExtend -= extensionSpeed;
        }
        else
        {
            extending = false;
            device.TriggerHapticPulse(1000);
        }

    }
}
