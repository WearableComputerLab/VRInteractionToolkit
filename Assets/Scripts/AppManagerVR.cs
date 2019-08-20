using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.IO;
using UnityEngine.SceneManagement;

public class AppManagerVR : MonoBehaviour {
    /*
    public GameObject script;
    public SteamVR_TrackedObject trackedObjL;
    public SteamVR_TrackedObject trackedObjR;
    private SteamVR_Controller.Device controllerL;
    private SteamVR_Controller.Device controllerR;

    public static int STATE = 0;
    private GameObject[] buttons = new GameObject[20];
    public GameObject buttonsParent;
    private int target = 0;
    private bool firstTarget = false;
    private float distance = 250f;
    private float amplitude = 40f;
    private float prevHitTime;
    public float buttonSize = 1f;
    private Vector3[] ogPositions = new Vector3[20];
    private bool gameRunning = false;

    public GameObject[,] linkedButtons = new GameObject[10, 2];

    private List<string> logInfo = new List<string>();

    void Awake() {
        script.SetActive(true);
        if (script.transform.parent.gameObject.activeInHierarchy == false) {
            script.transform.parent.gameObject.SetActive(true);
        }
    }

    // Use this for initialization
    void Start() {
        if(gameRunning == true) {
            int counter = 0;
            foreach(GameObject button in buttons) {
                button.transform.localPosition = ogPositions[counter];
                counter++;
            }
        }
        if (STATE == 0) {
            logInfo.Add("Technique, Obj Type, ID, Distance, Amplitude, Error, Time");
        }
        STATE++;
        firstTarget = false;
        gameRunning = true;
        target = 0;
        int count = 0;
        foreach(Transform child in buttonsParent.transform) {
            buttons[count] = child.gameObject;
            count++;
        }
        setButtonSize(buttons);
        initialize2DArray();
        newTargetVR();
        print("DISTANCE=" + distance + "|AMPLITUDE=" + amplitude);
        print("ID="+ Mathf.Log((distance / amplitude) + 1, 2));
        //SetNewTarget();
        //idText.text = "ID:" + Mathf.Log((distance / amplitude) + 1, 2).ToString();
        //distText.text = "D:" + distance;
        //amplText.text = "A:" + amplitude;
        //logInfo.Add(Mathf.Log((distance / amplitude) + 1, 2) + "," + distance + "," + amplitude + "," + currentScript.ToString());
        //writeToFile();
    }

    private void initialize2DArray() {
        linkedButtons[0, 0] = buttons[0];
        linkedButtons[0, 1] = buttons[1];
        linkedButtons[1, 0] = buttons[2];
        linkedButtons[1, 1] = buttons[3];
        linkedButtons[2, 0] = buttons[4];
        linkedButtons[2, 1] = buttons[5];
        linkedButtons[3, 0] = buttons[6];
        linkedButtons[3, 1] = buttons[7];
        linkedButtons[4, 0] = buttons[8];
        linkedButtons[4, 1] = buttons[9];
        linkedButtons[5, 0] = buttons[10];
        linkedButtons[5, 1] = buttons[11];
        linkedButtons[6, 0] = buttons[12];
        linkedButtons[6, 1] = buttons[13];
        linkedButtons[7, 0] = buttons[14];
        linkedButtons[7, 1] = buttons[15];
        linkedButtons[8, 0] = buttons[16];
        linkedButtons[8, 1] = buttons[17];
        linkedButtons[9, 0] = buttons[18];
        linkedButtons[9, 1] = buttons[19];
    }

    public GameObject cameraRig;

    private void setButtonSize(GameObject[] buttons) {
        int count = 0;
        foreach(GameObject button in buttons) {
            button.transform.localScale = new Vector3(buttonSize, buttonSize, buttonSize);
            ///button.transform.localPosition = new Vector2(button.transform.localPosition.x * (100f), button.transform.localPosition.y * (distance / 100f));
            ogPositions[count] = button.transform.localPosition;

            button.transform.localScale = new Vector3(amplitude/10f, amplitude/10f, amplitude/10f);
            //print("size:" + button.GetComponent<Renderer>().bounds.size);
            //button.transform.localPosition = new Vector3(button.transform.localPosition.x * (distance / 100f), button.transform.localPosition.y * (distance / 100f), button.transform.localPosition.z * (distance / 100f));
            button.transform.localPosition = new Vector3(40f, button.transform.localPosition.y * (distance / 100f), button.transform.localPosition.z * (distance / 100f));
            count++;
        }
        cameraRig.transform.position = new Vector3(cameraRig.transform.position.x, (distance / 10f)/2f, cameraRig.transform.position.z);
    }

    private int selectionIndex = 0;

    public void newTargetVR() {
        //print(states.Length);
        if(firstTarget == false || selectionIndex == 0) {
            if(target >= 1 && controllerR != null && controllerR.GetPressDown(SteamVR_Controller.ButtonMask.Trigger) || target >= 1 && controllerL != null && controllerL.GetPressDown(SteamVR_Controller.ButtonMask.Trigger)) {
                linkedButtons[target - 1, 1].GetComponent<Renderer>().material.color = Color.white;
                if(currentSelectedObject == linkedButtons[target - 1, 1]) {
                    print("Target:" + target + "A | Selection Time:" + prevHitTime * 1000 + " ms" + " | ERROR = FALSE");
                    //logInfo.Add("Target:" + target + "A | Selection Time:" + prevHitTime * 1000 + " ms" + " | ERROR = FALSE");
                    logInfo.Add(currentScript.ToString()+","+ +target+"A,"+ Mathf.Log((distance / amplitude) + 1, 2) + ","+distance+","+amplitude+",0,"+prevHitTime * 1000);
                } else {
                    print("Target:" + target + "A | Selection Time:" + prevHitTime * 1000 + " ms" + " | ERROR = TRUE");
                    logInfo.Add(currentScript.ToString() + "," + +target + "A," + Mathf.Log((distance / amplitude) + 1, 2) + "," + distance + "," + amplitude + ",1," + prevHitTime * 1000);
                }
                prevHitTime = 0f;
                linkedButtons[target, 0].GetComponent<Renderer>().material.color = Color.red;
                //linkedButtons[target, 0].transform.SetSiblingIndex(16);
                selectionIndex = 1;
                firstTarget = true;
            }
            if(target == 0) {
                linkedButtons[target, 0].GetComponent<Renderer>().material.color = Color.red;
                //linkedButtons[target, 0].transform.SetSiblingIndex(16);
                selectionIndex = 1;
                firstTarget = true;
            }
        } else {
            if(controllerR != null && controllerR.GetPressDown(SteamVR_Controller.ButtonMask.Trigger) || controllerL != null && controllerL.GetPressDown(SteamVR_Controller.ButtonMask.Trigger)) {
                if(target != 0) {
                    if(currentSelectedObject == linkedButtons[target, 0]) {
                        print("Target:" + target + "B | Selection Time:" + prevHitTime * 1000 + " ms" + " | ERROR = FALSE");
                        //logInfo.Add("Target:" + target + "B | Selection Time:" + prevHitTime * 1000 + " ms" + " | ERROR = FALSE");
                        logInfo.Add(currentScript.ToString() + "," + +target + "B," + Mathf.Log((distance / amplitude) + 1, 2) + "," + distance + "," + amplitude + ",0," + prevHitTime * 1000);
                    } else {
                        print("Target:" + target + "B | Selection Time:" + prevHitTime * 1000 + " ms" + " | ERROR = TRUE");
                        //logInfo.Add("Target:" + target + "B | Selection Time:" + prevHitTime * 1000 + " ms" + " | ERROR = TRUE");
                        logInfo.Add(currentScript.ToString() + "," + +target + "B," + Mathf.Log((distance / amplitude) + 1, 2) + "," + distance + "," + amplitude + ",1," + prevHitTime * 1000);
                    }
                }


                linkedButtons[target, 0].GetComponent<Renderer>().material.color = Color.white;
                linkedButtons[target, 1].GetComponent<Renderer>().material.color = Color.red;
                //linkedButtons[target, 1].transform.SetSiblingIndex(16);
                //print("TARGET:" + linkedButtons[target, 0].name);
                //print("SELECTED:" + currentSelectedObject);
                selectionIndex = 0;
                target++;
                prevHitTime = 0f;
                if(target == 10) { //completed..
                    if(completedStates.Count == states.Length/2) {
                        print("Loading next scene..");
                        writeToFile();
                        loadNextLevel();
                        return;
                    }
                    linkedButtons[9, 0].GetComponent<Renderer>().material.color = Color.white;
                    linkedButtons[9, 1].GetComponent<Renderer>().material.color = Color.white;
                    print("Completed.." + completedStates.Count);
                    print("RESTART WITH CHANGES NOW");
                    int state = Random.Range(0, states.Length/2);
                    while(completedStates.Contains(state)) {
                        state = Random.Range(0, states.Length/2);
                    }
                    completedStates.Add(state);
                    distance = states[state, 0];
                    amplitude = states[state, 1];
                    Start();
                }
            }
        }
    }
    private List<float> completedStates = new List<float>();
    //private float[,] states = new float[,] { { 400f, 20f }, { 350, 60f }, { 300, 40 }, { 500, 10f }, { 100, 50 } };
    private float[,] states = new float[,] { { 350f, 20f }, { 500, 10f } };
    public void loadNextLevel() {
        //SceneManager.LoadScene(Application.loadedLevel + 1);
        print("Scene has been completed for: "+currentScript+" ..load the next technique");
    }

    public void writeToFile() {
        //StreamWriter writer = new StreamWriter(Application.dataPath + "testing.txt", true);
        string dest = "trial" + ".csv";
        StreamWriter writer = null;
        //using(writer = File.AppendText(dest)) {
        //StreamWriter writer = new StreamWriter(dest, true);
        int count = 1;
        bool foundPath = false;
        while(foundPath == false) {
            if(File.Exists(dest)) {
                dest = "trial" + count + ".csv";
                count++;
            } else {
                print("Found path:" + dest);
                writer = new StreamWriter(dest, true) as StreamWriter;
                foundPath = true;
            }
        }
        print("Writing..");
        for(int i = 0; i < logInfo.Count; i++) {
            print(logInfo[i]);
            writer.Write(logInfo[i]);
            writer.WriteLine();
        }
        print("Writen to file:" + Application.dataPath);
        writer.Close();
    }

    private GameObject currentSelectedObject;
    public enum SCRIPT {FISHING_REEL, BUBBLE_CURSOR, ABSOLUTE_RELATIVE_MAPPING, WORLD_IN_MIN, BENDCAST, APERATURE, FLASHLIGHT, ISITH, HOOK, IMAGEPLANE_STICKYHAND, IMAGEPLANE_FRAMINGHANDS};
    public SCRIPT currentScript;

    // Update is called once per frame
    void Update() {
        if(currentScript == SCRIPT.FISHING_REEL) {
            if(script.GetComponent<FishingReel>().lastSelectedObject != null && !script.GetComponent<FishingReel>().lastSelectedObject.Equals(currentSelectedObject)) {
                currentSelectedObject = script.GetComponent<FishingReel>().lastSelectedObject;
            }
        } else if(currentScript == SCRIPT.BUBBLE_CURSOR) {
            if(script.GetComponent<BubbleCursor>().lastSelectedObject != null && !script.GetComponent<BubbleCursor>().lastSelectedObject.Equals(currentSelectedObject)) {
                currentSelectedObject = script.GetComponent<BubbleCursor>().lastSelectedObject;
            }
        } else if(currentScript == SCRIPT.ABSOLUTE_RELATIVE_MAPPING) {
            if(script.GetComponent<ARMLaser>().lastSelectedObject != null && !script.GetComponent<ARMLaser>().lastSelectedObject.Equals(currentSelectedObject)) {
                currentSelectedObject = script.GetComponent<ARMLaser>().lastSelectedObject;
            }
        } else if(currentScript == SCRIPT.WORLD_IN_MIN) {
            if(script.GetComponent<WorldInMiniature>().selectedObject != null && !script.GetComponent<WorldInMiniature>().selectedObject.Equals(currentSelectedObject)) {
                currentSelectedObject = script.GetComponent<WorldInMiniature>().selectedObject;
            }
        } else if(currentScript == SCRIPT.BENDCAST) {
            if(script.GetComponent<BendCast>().lastSelectedObject != null && !script.GetComponent<BendCast>().lastSelectedObject.Equals(currentSelectedObject)) {
                currentSelectedObject = script.GetComponent<BendCast>().lastSelectedObject;
            }
        } else if(currentScript == SCRIPT.APERATURE) {
            if(script.GetComponent<AperatureSelectionSelector>().selection != null && !script.GetComponent<AperatureSelectionSelector>().selection.Equals(currentSelectedObject)) {
                currentSelectedObject = script.GetComponent<AperatureSelectionSelector>().selection;
            }
        } else if(currentScript == SCRIPT.FLASHLIGHT) {
            if(script.GetComponent<FlashlightSelection>().selection != null && !script.GetComponent<FlashlightSelection>().selection.Equals(currentSelectedObject)) {
                currentSelectedObject = script.GetComponent<FlashlightSelection>().selection;
            }
        } else if(currentScript == SCRIPT.ISITH) {
            if(script.GetComponent<iSithGrabObject>().objectInHand != null && !script.GetComponent<iSithGrabObject>().objectInHand.Equals(currentSelectedObject)) {
                currentSelectedObject = script.GetComponent<iSithGrabObject>().objectInHand;
            }
        } else if(currentScript == SCRIPT.HOOK) {
            if(script.GetComponent<Hook>().selection != null && !script.GetComponent<Hook>().selection.Equals(currentSelectedObject)) {
                currentSelectedObject = script.GetComponent<Hook>().selection;
            }
        } else if(currentScript == SCRIPT.IMAGEPLANE_STICKYHAND) {
            if(script.GetComponent<ImagePlane_StickyHand>().selectedObject != null && !script.GetComponent<ImagePlane_StickyHand>().selectedObject.Equals(currentSelectedObject)) {
                currentSelectedObject = script.GetComponent<ImagePlane_StickyHand>().selectedObject;
            }
        } else if(currentScript == SCRIPT.IMAGEPLANE_FRAMINGHANDS) {
            if(script.GetComponent<ImagePlane_FramingHands>().selectedObject != null && !script.GetComponent<ImagePlane_FramingHands>().selectedObject.Equals(currentSelectedObject)) {
                currentSelectedObject = script.GetComponent<ImagePlane_FramingHands>().selectedObject;
            }
        }

        if((int)trackedObjR.index != -1) {
            controllerR = SteamVR_Controller.Input((int)trackedObjR.index);
        }
        if((int)trackedObjL.index != -1) {
            controllerL = SteamVR_Controller.Input((int)trackedObjL.index);
        }


        newTargetVR();
        //print("prev hit time:"+prevHitTime);
        if(target >= 1) {
            prevHitTime += Time.deltaTime;
        }
    }*/
}
