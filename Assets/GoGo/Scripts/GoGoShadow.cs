using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class GoGoShadow : MonoBehaviour {

    private Camera playerCamera;

#if SteamVR_Legacy
    public SteamVR_TrackedObject trackedObj; 
    private SteamVR_Controller.Device device;
#elif SteamVR_2
    public SteamVR_Behaviour_Pose trackedObj;
    public SteamVR_Action_Boolean m_touchpadPress;
#else
    public GameObject trackedObj;
#endif

    public GameObject cameraRig; // So shadow can attach itself to the camera rig on game start

    public enum ToggleArmLengthCalculator {
        on,
        off
    }
    // If toggled on the user can press down on the touchpad with their arm extended to take a measurement of the arm
    // If it is off the user must inut a manual estimate of what the users arm length would be
    public ToggleArmLengthCalculator armLengthCalculator = ToggleArmLengthCalculator.off;

    public float armLength; // Either manually inputted or will be set to the arm length when calculated

    public float distanceFromHeadToChest = 0.3f; // estimation of the distance from the users headset to their chest area

    public GameObject theController; // controller for the gogo to access inout

    public GameObject theModel; // the model of the controller that will be shadowed for gogo use

    public float extensionVariable = 10f; // this variable in the equation controls the multiplier for how far the arm can extend with small movements

    bool calibrated = false;
    Vector3 chestPosition;
    Vector3 relativeChestPos;



    void makeModelChild() {
        if (this.transform.childCount == 0) {
            if (theModel.GetComponent<SteamVR_RenderModel>() != null) { // The steamVR_RenderModel is generated after code start so we cannot parent right away or it wont generate. 
                if (theModel.transform.childCount > 0) {
                    theModel.transform.parent = this.transform;
                    // Due to the transfer happening at a random time down the line we need to re-align the model inside the shadow controller to 0 so nothing is wonky.
                    theModel.transform.localPosition = Vector3.zero;
                    theModel.transform.localRotation = Quaternion.identity;
                }
            } else {
                // If it is just a custom model we can immediately parent
                theModel.transform.parent = this.transform;
                // Due to the transfer happening at a random time down the line we need to re-align the model inside the shadow controller to 0 so nothing is wonky.
                theModel.transform.localPosition = Vector3.zero;
                theModel.transform.localRotation = Quaternion.identity;
            }
        }

    }

    // Might have to have a manuel calibration for best use
    float getDistanceToExtend() {
        // estimating chest position using an assumed distance from head to chest and then going that distance down the down vector of the camera. This will not allways be optimal especially when leaning is involved.
        // To improve gogo to suite your needs all you need to do is implement your own algorithm to estimate chest (or shoulder for even high accuracy) position and set the chest position vector to match it

        Vector3 direction = playerCamera.transform.up * -1;
        Vector3 normalizedDirectionPlusDistance = direction.normalized * distanceFromHeadToChest;
        chestPosition = playerCamera.transform.position + normalizedDirectionPlusDistance;

        float distChestPos = Vector3.Distance(trackedObj.transform.position, chestPosition);

        float D = (2f * armLength) / 3f; // 2/3 of users arm length

        //D = 0;
        if (distChestPos >= D) {
            float extensionDistance = distChestPos + (extensionVariable * (float)Math.Pow(distChestPos - D, 2));
            // Dont need both here as we only want the distance to extend by not the full distance
            // but we want to keep the above formula matching the original papers formula so will then calculate just the distance to extend below
            return extensionDistance - distChestPos;
        }
        return 0; // dont extend
    }

    // Use this for initialization
    void Start() {
        this.transform.parent = cameraRig.transform;
        if (Camera.main != null) {
            playerCamera = Camera.main;
        } else {
            playerCamera = cameraRig.GetComponentInChildren<Camera>();
        }
        makeModelChild();
    }


    // Update is called once per frame
    void Update() {
        makeModelChild();
        //this.GetComponentInChildren<SteamVR_RenderModel>().gameObject.SetActive(false);
        Renderer[] renderers = this.transform.parent.GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers) {
            if (renderer.material.name == "Standard (Instance)") {
                renderer.enabled = true;
            }
        }
        checkForAction();
        moveControllerForward();
    }

    void moveControllerForward() {
        // Using the origin and the forward vector of the remote the extended positon of the remote can be calculated
        //Vector3 theVector = theController.transform.forward;
        Vector3 theVector = theController.transform.position - chestPosition;

        Vector3 pose = theController.transform.position;
        Quaternion rot = theController.transform.rotation;

        float distance_formula_on_vector = Mathf.Sqrt(theVector.x * theVector.x + theVector.y * theVector.y + theVector.z * theVector.z);

        float distanceToExtend = getDistanceToExtend();

        if (distanceToExtend != 0) {
            // Using formula to find a point which lies at distance on a 3D line from vector and direction
            pose.x = pose.x + (distanceToExtend / (distance_formula_on_vector)) * theVector.x;
            pose.y = pose.y + (distanceToExtend / (distance_formula_on_vector)) * theVector.y;
            pose.z = pose.z + (distanceToExtend / (distance_formula_on_vector)) * theVector.z;
        }

        transform.position = pose;
        transform.rotation = rot;
    }

    public enum ControllerState {
        TOUCHPAD_UP, NONE
    }

    private ControllerState controllerEvents() {
#if SteamVR_Legacy
        if (device.GetPressUp(SteamVR_Controller.ButtonMask.Axis0)) {
            return ControllerState.TOUCHPAD_UP;
        }
#elif SteamVR_2
        if (m_touchpadPress.GetStateUp(trackedObj.inputSource)) {
            return ControllerState.TOUCHPAD_UP;
        }
#endif
        return ControllerState.NONE;
    }

    void checkForAction() {
#if SteamVR_Legacy
        device = SteamVR_Controller.Input((int)trackedObj.index);
#endif
        if (armLengthCalculator == ToggleArmLengthCalculator.on && controllerEvents() == ControllerState.TOUCHPAD_UP) //(will only register if arm length calculator is on)
        {
            armLength = Vector3.Distance(trackedObj.transform.position, chestPosition);
            calibrated = true;
        }
    }
}
