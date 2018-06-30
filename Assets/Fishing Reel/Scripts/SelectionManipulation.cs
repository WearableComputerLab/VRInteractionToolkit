using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionManipulation : MonoBehaviour {

    internal SteamVR_TrackedObject trackedObj;
    internal GameObject manipulationIcons;
    private SteamVR_Controller.Device controller;
    float[] posX = { -1, 0, 1, 2, 3 };
    Transform[] iconChildren;
    Transform iconHighlighter;
    int counter = 0;

	// Use this for initialization
	void Start () {
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
	}
	
	// Update is called once per frame
	void Update () {
        controller = SteamVR_Controller.Input((int)trackedObj.index);
        if (controller.GetPressDown(SteamVR_Controller.ButtonMask.Touchpad)) {
            Vector2 touchpad = (controller.GetAxis(Valve.VR.EVRButtonId.k_EButton_Axis0));
            if (touchpad.x > 0.7f) {
                print("Moved right..");
                if (counter < 4) {
                    iconHighlighter.transform.localPosition += new Vector3(1f, 0f, 0f);
                    counter += 1;
                }
            } else if (touchpad.x < -0.7f) {
                print("Moved left..");
                if (counter > 0) {
                    counter -= 1;
                    iconHighlighter.transform.localPosition -= new Vector3(1f, 0f, 0f);
                }
            }
        }
    }
}
