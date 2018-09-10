using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.IO;
using UnityEngine.SceneManagement;

public class AppManagerVR : MonoBehaviour {

    public static int STATE = 0;
    private GameObject[] buttons = new GameObject[20];
    public GameObject buttonsParent;
    private int target = 0;
    private bool firstTarget = false;
    private float distance = 200f;
    private float amplitude = 50f;
    private float prevHitTime;
    public float buttonSize = 1f;
    private Vector3[] ogPositions = new Vector3[20];
    private bool gameRunning = false;

    public GameObject[,] linkedButtons = new GameObject[10, 2];

    public List<string> logInfo = new List<string>();

    // Use this for initialization
    void Start() {
        if(gameRunning == true) {
            int counter = 0;
            foreach(GameObject button in buttons) {
                button.transform.localPosition = ogPositions[counter];
                counter++;
            }
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
        //logInfo.Add(idText.text + " | " + distText.text + " | " + amplText.text);
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

    private void setButtonSize(GameObject[] buttons) {
        int count = 0;
        foreach(GameObject button in buttons) {
            button.transform.localScale = new Vector3(buttonSize, buttonSize, buttonSize);
            ///button.transform.localPosition = new Vector2(button.transform.localPosition.x * (100f), button.transform.localPosition.y * (distance / 100f));
            ogPositions[count] = button.transform.localPosition;

            button.transform.localScale = new Vector3(amplitude/10f, amplitude/10f, amplitude/10f);
            //print("size:" + button.GetComponent<Renderer>().bounds.size);
            //button.transform.localPosition = new Vector3(button.transform.localPosition.x * (distance / 100f), button.transform.localPosition.y * (distance / 100f), button.transform.localPosition.z * (distance / 100f));
            button.transform.localPosition = new Vector3(button.transform.localPosition.x * (distance / 100f), button.transform.localPosition.y * (distance / 100f), button.transform.localPosition.z * (distance / 100f));
            count++;
        }
    }

    private int selectionIndex = 0;

    public void newTargetVR() {
        if(firstTarget == false || selectionIndex == 0) {
            if(target >= 1 && Input.anyKeyDown) {
                linkedButtons[target - 1, 1].GetComponent<Renderer>().material.color = Color.white;
                prevHitTime = 0f;
                linkedButtons[target, 0].GetComponent<Renderer>().material.color = Color.red;
                linkedButtons[target, 0].transform.SetSiblingIndex(16);
                selectionIndex = 1;
                firstTarget = true;
            }
            if(target == 0) {
                linkedButtons[target, 0].GetComponent<Renderer>().material.color = Color.red;
                linkedButtons[target, 0].transform.SetSiblingIndex(16);
                selectionIndex = 1;
                firstTarget = true;
            }
        } else {
            if(Input.anyKeyDown) {
                linkedButtons[target, 0].GetComponent<Renderer>().material.color = Color.white;
                linkedButtons[target, 1].GetComponent<Renderer>().material.color = Color.red;
                linkedButtons[target, 1].transform.SetSiblingIndex(16);
                selectionIndex = 0;
                target++;
                prevHitTime = 0f;
                if(target == 10) { //completed..
                    if(completedStates.Count == 5) {
                        print("Loading next scene..");
                        writeToFile();
                        loadNextLevel();
                        return;
                    }
                    linkedButtons[9, 0].GetComponent<Renderer>().material.color = Color.white;
                    linkedButtons[9, 1].GetComponent<Renderer>().material.color = Color.white;
                    print("Completed.." + completedStates.Count);
                    print("RESTART WITH CHANGES NOW");
                    int state = Random.Range(0, 5);
                    while(completedStates.Contains(state)) {
                        state = Random.Range(0, 5);
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
    private float[,] states = new float[,] { { 400f, 20f }, { 350, 60f }, { 300, 40 }, { 500, 10f }, { 100, 50 } };

    public void loadNextLevel() {
        SceneManager.LoadScene(Application.loadedLevel + 1);
    }

    public void writeToFile() {
        //StreamWriter writer = new StreamWriter(Application.dataPath + "testing.txt", true);
        string dest = "trial" + ".txt";
        StreamWriter writer = null;
        //using(writer = File.AppendText(dest)) {
        //StreamWriter writer = new StreamWriter(dest, true);
        int count = 1;
        bool foundPath = false;
        while(foundPath == false) {
            if(File.Exists(dest)) {
                dest = "trial" + count + ".txt";
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



    // Update is called once per frame
    void Update() {
        newTargetVR();
        //print("prev hit time:"+prevHitTime);
        if(target >= 1) {
            prevHitTime += Time.deltaTime;
        }
    }
}
