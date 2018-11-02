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
    internal bool changeSizeEnabled;
    private SteamVR_Controller.Device controller;
    private GameObject oldSelectedObject;
    internal Transform startParent;
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
        startParent = this.transform;
        iconChildren = new Transform[6];
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
        manipulationIcons.transform.SetParent(null);
    }

    private void resetManipulationMenu() {
        if (controller.GetPressDown(SteamVR_Controller.ButtonMask.ApplicationMenu) && inManipulationMode == true) {
            inManipulationMode = false;
            colourPickerEnabled = false;
            changeSizeEnabled = false;
            print(startParent);
            manipulationIcons.transform.SetParent(startParent);
            manipulationIcons.SetActive(false);
            manipulationIcons.transform.SetParent(null);
            iconHighlighter.transform.localPosition = new Vector3(-1f, 0f, 0f);
            index = 0;
        }
    }

    float tempLocalScale = 0f;
    void selectIcon() {
        if (index == 0) { // Return
            print("Return object");
            colourPickerEnabled = false;
            changeSizeEnabled = false;
            manipulationIcons.transform.SetParent(startParent);
            manipulationIcons.SetActive(false);
            manipulationIcons.transform.SetParent(null);
            inManipulationMode = false;
        } else if (index == 1) { // Regular movement
            print("Moving object");
            manipulationMovementEnabled = true;
            colourPickerEnabled = false;
            changeSizeEnabled = false;
            moveObject();
        } else if (index == 2) { // Delete the object
            print("Deleting object:" + selectedObject.name);
            Destroy(selectedObject);
            inManipulationMode = false;
            colourPickerEnabled = false;
            changeSizeEnabled = false;
            manipulationIcons.transform.SetParent(startParent);
            manipulationIcons.SetActive(false);
            manipulationIcons.transform.SetParent(null);
            iconHighlighter.transform.localPosition = new Vector3(-1f, 0f, 0f);
            index = 0;
        } else if (index == 3) { // Change colour
            colourPickerEnabled = true;
            changeSizeEnabled = false;
        } else if (index == 4) { // Change size
            //print("Increasing size..");
            if (changeSizeEnabled == false) {
                tempLocalScale = selectedObject.transform.localScale.x;
            }
            changeSizeEnabled = true;
            colourPickerEnabled = false;
        }
    }

    private float sizeIncrease = 0f;
    private float cursorSpeed = 20f;

    private void confirmSize() {
        if (controller.GetPressDown(SteamVR_Controller.ButtonMask.Trigger) && changeSizeEnabled == true && tempLocalScale != selectedObject.transform.localScale.x) {
            print("Size has been chosen.");
            changeSizeEnabled = false;
            iconHighlighter.transform.localPosition = new Vector3(-1f, 0f, 0f);
            index = 0;
        }
    }


    private bool pickedUpObject = false;
    private Transform oldParent;
    private void moveObject() {
        if (controller.GetPressDown(SteamVR_Controller.ButtonMask.Trigger) && pickedUpObject == false) {
            print("picked up object");
            oldParent = selectedObject.transform.parent;
            pickedUpObject = true;
            selectedObject.transform.SetParent(trackedObj.transform);
        }
        else if (controller.GetPressDown(SteamVR_Controller.ButtonMask.Trigger) && pickedUpObject == true) {
            print("dropped object");
            pickedUpObject = false;
            manipulationMovementEnabled = false;
            iconHighlighter.transform.localPosition = new Vector3(-1f, 0f, 0f);
            index = 0;
            if (oldParent != null) {
                selectedObject.transform.SetParent(oldParent);
            } else {
                selectedObject.transform.SetParent(null);
            }
        }
    }

    private float sizeIncreaseRate = 0.01f;
    private float sizeDecreaseRate = 0.01f;

    private void changeSize() {
        Vector3 controllerPos = trackedObj.transform.forward;
        if (controller.GetAxis().y != 0) {
            if (controller.GetAxis().y > 0.7f) {
                print("Increasing size");
                selectedObject.transform.localScale = new Vector3(selectedObject.transform.localScale.x + sizeIncreaseRate, selectedObject.transform.localScale.y + sizeIncreaseRate, selectedObject.transform.localScale.z + sizeIncreaseRate);
            } else if (controller.GetAxis().y < -0.7f && selectedObject.transform.localScale.x > 0f) {
                print("Decreasing size");
                selectedObject.transform.localScale = new Vector3(selectedObject.transform.localScale.x - sizeDecreaseRate, selectedObject.transform.localScale.y - sizeDecreaseRate, selectedObject.transform.localScale.z - sizeDecreaseRate);
            }
        }
    }

    void navigateOptions() {
        if (controller.GetPressDown(SteamVR_Controller.ButtonMask.Touchpad) && inManipulationMode == true) {
            Vector2 touchpad = (controller.GetAxis(Valve.VR.EVRButtonId.k_EButton_Axis0));
            if (colourPickerEnabled == false && changeSizeEnabled == false) {
                if (touchpad.x > 0.7f) {
                    //print("Moved right..");
                    if (index < 4) {
                        iconHighlighter.transform.localPosition += new Vector3(1f, 0f, 0f);
                        index += 1;
                    }
                } else if (touchpad.x < -0.7f) {
                    //print("Moved left..");
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
        resetManipulationMenu();
        if (changeSizeEnabled == true) {
            changeSize();
            confirmSize();
        }
        if (manipulationMovementEnabled == true) {
            moveObject();
        }
        if (controller.GetPressDown(SteamVR_Controller.ButtonMask.Trigger)) {
            if (inManipulationMode == false && selectedObject != null && selectedObject.name != "Mirrored Cube" && manipulationIcons.activeInHierarchy == false) {
                print(selectedObject);
                this.GetComponent<ColorPicker>().selectedObj = selectedObject;
                print("position set:" + manipulationIcons.transform.position);
                manipulationIcons.transform.localEulerAngles = Camera.main.transform.localEulerAngles;
                inManipulationMode = true;
                manipulationIcons.SetActive(true);
                manipulationIcons.transform.SetParent(selectedObject.transform);
                //manipulationIcons.transform.position = new Vector3(selectedObject.transform.position.x, selectedObject.transform.position.y * 2.2f, selectedObject.transform.position.z);
                manipulationIcons.transform.localPosition = new Vector3(0f, 1f, 0f);
                manipulationIcons.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
            } else if (inManipulationMode == true && manipulationIcons.activeInHierarchy == true) {
                selectIcon();
            }
        }
    }
}
