using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorPicker : MonoBehaviour {

    private float hue, saturation, val = 1f;

    private GameObject blackWheel;
    private GameObject canvasHolder;
    internal GameObject selectedObj;
    internal SteamVR_TrackedObject trackedObj;
    private SteamVR_Controller.Device controller;

    // Use this for initialization
    void Start () {
        blackWheel = GameObject.Find("Black Wheel");
        canvasHolder = GameObject.Find("CanvasHolder");
        canvasHolder.transform.SetParent(trackedObj.transform);

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
        if (controller.GetAxis().y != 0) {
            float touchpadAngle = CalculateTouchpadAxisAngle(controller.GetAxis());
            ChangedHueSaturation(controller.GetAxis(), touchpadAngle);
        }
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

    void pickColour() {
        if (controller.GetPressDown(SteamVR_Controller.ButtonMask.Trigger)) {
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
        controller = SteamVR_Controller.Input((int)trackedObj.index);
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
