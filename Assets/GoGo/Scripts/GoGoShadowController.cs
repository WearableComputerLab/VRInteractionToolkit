using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class GoGoShadowController : MonoBehaviour {

    public GameObject theController;

    public SteamVR_TrackedObject trackedObj;

    public GameObject theModel;

    private float distanceToExtend = 0f;

    private SteamVR_Controller.Device device;
    private Vector3 vector_from_device;
    private Vector3 device_origin;
    private bool remote_go_go = false;
    bool extendingForward = true; // If not in extendingmode the arm will retract
    bool extending = false;
    float extensionSpeed = 0.02f;
    bool ranAlready = false;

    // TODO: THIS IS A HACK. NEED TO FIND A METHOD THAT JUST WAITS UNTIL THE MODEL IS INITALIZED INSTEAD OF CALLING OVER AND OVER
    void makeModelChild()
    {
        if(theModel.transform.childCount > 0)
        {
            theModel.transform.parent = this.transform;
        }
        
    }

    // Use this for initialization
    void Start () {
        //trackedObj = this.GetComponent<SteamVR_TrackedObject>();
        CopySpecialComponents(theController, this.gameObject);

    }

    private void CopySpecialComponents(GameObject _sourceGO, GameObject _targetGO)
    {
        foreach (var component in _sourceGO.GetComponentsInChildren<Component>())
        {
            var componentType = component.GetType();
            if (componentType != typeof(Transform) &&
                componentType != typeof(MeshFilter) &&
                componentType != typeof(MeshRenderer)
                )
            {
                Debug.Log("Found a component of type " + component.GetType());
                UnityEditorInternal.ComponentUtility.CopyComponent(component);
                UnityEditorInternal.ComponentUtility.PasteComponentAsNew(_targetGO);
                Debug.Log("Copied " + component.GetType() + " from " + _sourceGO.name + " to " + _targetGO.name);
            }
        }
    }

    // Update is called once per frame
    void Update () {
        makeModelChild();
        //this.GetComponentInChildren<SteamVR_RenderModel>().gameObject.SetActive(false);
        Renderer[] renderers = this.transform.parent.GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            if (renderer.material.name == "Standard (Instance)")
            {
                renderer.enabled = true;
            }
        }
          checkForAction();
          moveControllerForward();

        
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
        device = SteamVR_Controller.Input((int)trackedObj.index);

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
        if (touchpad.y > 0f && device.GetPressDown(SteamVR_Controller.ButtonMask.Axis0)) // top side of touchpad and pushing down
        {
            extending = true;
            extendingForward = true;
        }
        else if (touchpad.y < 0f && device.GetPressDown(SteamVR_Controller.ButtonMask.Axis0)) // bottom side of touchpad and pushing down
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
