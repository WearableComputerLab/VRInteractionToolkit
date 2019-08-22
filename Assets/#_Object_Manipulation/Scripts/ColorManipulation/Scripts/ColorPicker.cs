using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class ColorPicker : MonoBehaviour {
#if SteamVR_Legacy
    internal SteamVR_TrackedObject trackedObj;
    private SteamVR_Controller.Device controller;
#elif SteamVR_2
    internal SteamVR_Behaviour_Pose trackedObj;
    internal SteamVR_Action_Boolean m_controllerPress;
    internal SteamVR_Action_Vector2 m_touchpadAxis;
#else
    internal GameObject trackedObj;
#endif
    private float hue, saturation, val = 1f;

    private GameObject blackWheel;
    private GameObject canvasHolder;
    internal GameObject selectedObj;

    // Use this for initialization
    void Start () {
        blackWheel = GameObject.Find("Black Wheel");
        canvasHolder = GameObject.Find("CanvasHolder");
        canvasHolder.transform.SetParent(trackedObj.transform);
        canvasHolder.SetActive(false);

    }

    //Code from VRTK
    private float CalculateTouchpadAxisAngle(Vector2 axis) {
        float angle = Mathf.Atan2(axis.y, axis.x) * Mathf.Rad2Deg;
        angle = 90.0f - angle;
        if (angle < 0) {
            angle += 360.0f;
        }
        return angle;
    }


    private void PadScrolling() {
#if SteamVR_Legacy
        if (controller.GetAxis().y != 0) {
            float touchpadAngle = CalculateTouchpadAxisAngle(controller.GetAxis());
            ChangedHueSaturation(controller.GetAxis(), touchpadAngle);
        }
#elif SteamVR_2
        if (m_touchpadAxis.GetAxis(trackedObj.inputSource).y != 0) {
            float touchpadAngle = CalculateTouchpadAxisAngle(m_touchpadAxis.GetAxis(trackedObj.inputSource));
            ChangedHueSaturation(m_touchpadAxis.GetAxis(trackedObj.inputSource), touchpadAngle);
        }
#endif
    }

    private void ChangedHueSaturation(Vector2 touchpadAxis, float touchpadAngle) {
        float normalAngle = touchpadAngle - 90;
        if (normalAngle < 0) {
            normalAngle = 360 + normalAngle;
        }

        float rads = normalAngle * Mathf.PI / 180;
        float maxX = Mathf.Cos(rads);
        float maxY = Mathf.Sin(rads);

        float curX = touchpadAxis.x;
        float curY = touchpadAxis.y;

        float percentX = Mathf.Abs(curX / maxX);
        float percentY = Mathf.Abs(curY / maxY);

        hue = normalAngle / 360.0f;
        saturation = (percentX + percentY) / 2;
        UpdateColor();
    }

    private void UpdateColor() {
        if (selectedObj != null) {
            Color color = Color.HSVToRGB(hue, saturation, val);
            selectedObj.GetComponent<Renderer>().material.color = color;
        }
    }

    public enum ControllerState {
        TRIGGER_DOWN, NONE
    }

    private ControllerState controllerEvents() {
#if SteamVR_Legacy
        if (controller.GetPressDown(Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger)) {
            return ControllerState.TRIGGER_DOWN;
        }
#elif SteamVR_2
        if (m_controllerPress.GetStateDown(trackedObj.inputSource)) {
            return ControllerState.TRIGGER_DOWN;
        }
#endif
        return ControllerState.NONE;
    }

    void pickColour() {
        if (controllerEvents() == ControllerState.TRIGGER_DOWN) {
            print("Colour has been chosen.");
            canvasHolder.SetActive(false);
            //this.GetComponent<SelectionManipulation>().inManipulationMode = false;
            this.GetComponent<SelectionManipulation>().colourPickerEnabled = false;
            //this.GetComponent<SelectionManipulation>().manipulationIcons.SetActive(false);
            this.GetComponent<SelectionManipulation>().iconHighlighter.transform.localPosition = new Vector3(-1f, 0f, 0f);
            this.GetComponent<SelectionManipulation>().index = 0;
        }
    }

	// Update is called once per frame
	void Update () {
#if SteamVR_Legacy
        controller = SteamVR_Controller.Input((int)trackedObj.index);
#elif SteamVR_2

#endif
        if (this.GetComponent<SelectionManipulation>().colourPickerEnabled == true) {
            if (canvasHolder.activeInHierarchy == false) {
                canvasHolder.SetActive(true);
            } else {
                PadScrolling();
                pickColour();
            }
        }

    }
}
