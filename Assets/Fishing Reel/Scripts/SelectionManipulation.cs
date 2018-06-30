using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionManipulation : MonoBehaviour {

    internal SteamVR_TrackedObject trackedObj;
    internal GameObject selectedObject;
    internal GameObject manipulationIcons;
    internal bool inManipulationMode;
    internal bool colourPickerEnabled;
    internal bool manipulationMovementEnabled;
    private SteamVR_Controller.Device controller;
    private GameObject oldSelectedObject;
    float[] posX = { -1, 0, 1, 2, 3 };
    Transform[] iconChildren;
    internal Transform iconHighlighter;
    internal int index = 0;

	// Use this for initialization
	void Start () {
        this.gameObject.AddComponent<ColorPicker>();
        this.GetComponent<ColorPicker>().trackedObj = trackedObj;
        inManipulationMode = false;
        colourPickerEnabled = false;
        manipulationMovementEnabled = false;
        iconChildren = new Transform[5];
        int count = 0;
        foreach (Transform child in manipulationIcons.transform) {
            if (child.name != "Icon_Highlighter") {
                iconChildren[count] = child;
                count++;
            } else {
                iconHighlighter = child;
            }
        }
        manipulationIcons.SetActive(false);
	}

    void selectIcon() {
        if (controller.GetPressDown(SteamVR_Controller.ButtonMask.Trigger) && inManipulationMode == true) {
            print("Made it in here..");
            if (index == 0) { // Regular movement
                print("Moving object");
                //manipulationMovementEnabled = true;
                colourPickerEnabled = false;
                //inManipulationMode = false;
                //manipulationIcons.SetActive(false);
            } else if (index == 1) { // Delete the object
                print("Deleting object:" + selectedObject.name);
                Destroy(selectedObject);
                inManipulationMode = false;
                colourPickerEnabled = false;
                manipulationIcons.SetActive(false);
                iconHighlighter.transform.localPosition = new Vector3(-1f, 0f, 0f);
                index = 0;
            } else if (index == 2) { // Change colour
                colourPickerEnabled = true;
            } else if (index == 3) { // Increase size
                colourPickerEnabled = false;
            } else if (index == 4) { // Decrease size
                colourPickerEnabled = false;
            }
        }
    }

    void navigateOptions() {
        if (controller.GetPressDown(SteamVR_Controller.ButtonMask.Touchpad)) {
            Vector2 touchpad = (controller.GetAxis(Valve.VR.EVRButtonId.k_EButton_Axis0));
            if (colourPickerEnabled == false) {
                if (touchpad.x > 0.7f) {
                    print("Moved right..");
                    if (index < 4) {
                        iconHighlighter.transform.localPosition += new Vector3(1f, 0f, 0f);
                        index += 1;
                    }
                } else if (touchpad.x < -0.7f) {
                    print("Moved left..");
                    if (index > 0) {
                        index -= 1;
                        iconHighlighter.transform.localPosition -= new Vector3(1f, 0f, 0f);
                    }
                }
            }
        }
    }
	
	// Update is called once per frame
	void Update () {
        controller = SteamVR_Controller.Input((int)trackedObj.index);
        navigateOptions();
        selectIcon();
        if (selectedObject != null && selectedObject != oldSelectedObject && inManipulationMode == false) {
            oldSelectedObject = selectedObject;
            this.GetComponent<ColorPicker>().selectedObj = selectedObject;
            manipulationIcons.transform.position = new Vector3(selectedObject.transform.position.x, selectedObject.transform.position.y * 2.2f, selectedObject.transform.position.z);
            manipulationIcons.transform.localEulerAngles = Camera.main.transform.localEulerAngles;
            inManipulationMode = true;
            manipulationIcons.SetActive(true);
            System.Threading.Thread.Sleep(150); // Mini-delay to fix synchronization issues.. Gotta find a better way to do this
        }
    }
}
