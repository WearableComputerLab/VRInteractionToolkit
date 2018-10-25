using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
public class globalDocker : MonoBehaviour {
	public GameObject[] outlineObjects = new GameObject[4];

    public static int sceneCounter = 0;
	public enum RecordController {LEFT, RIGHT, BOTH};

	public GameObject selectionTechnique;
	
    public RecordController recordController;

	public static List<string> logInfo = new List<string>();
	
	public void writeToFile() {
        //StreamWriter writer = new StreamWriter(Application.dataPath + "testing.txt", true);
        string dest = "docker_trial" + ".csv";
        StreamWriter writer = null;
        //using(writer = File.AppendText(dest)) {
        //StreamWriter writer = new StreamWriter(dest, true);
        int count = 1;
        bool foundPath = false;
        while(foundPath == false) {
            if(File.Exists(dest)) {
                dest = "docker_trial" + count + ".csv";
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

	void Start () {
		if (sceneCounter == 0) {
			logInfo.Add("Technique, Stage, Time, LeftH_Movement, RightH_Movement, Head_Movement");
		} else if (sceneCounter == 4) {
			writeToFile();
		}
		sceneCounter++;
		outlineObjects[sceneCounter-1].SetActive(true);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
