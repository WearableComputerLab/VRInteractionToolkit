using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class DockerData : MonoBehaviour {

    /*
    public globalDocker globalScript;
    public SteamVR_TrackedObject trackedObjL;
    public SteamVR_TrackedObject trackedObjR;
    public SteamVR_TrackedObject trackedObjH;

    

    public static float sceneCounter = 0;

    public static float totalDistL;
    public static float totalDistR;
    public static float totalDistH;

    public static float totalTime;

    internal int dockerCount = 0;
    private readonly int dockerObjects = 4;





    internal void incrementDockerCount() {
        //print("Increment called..");
        dockerCount++;
        if (dockerCount == 1 && trackedObjL.GetComponent<CountDistance>().counting == false || dockerCount == 1 && trackedObjR.GetComponent<CountDistance>().counting == false) {
            trackedObjL.GetComponent<CountDistance>().counting = true;
            trackedObjR.GetComponent<CountDistance>().counting = true;
            trackedObjH.GetComponent<CountDistance>().counting = true;
        }
        if (dockerCount >= dockerObjects) {
            totalDistL += trackedObjL.GetComponent<CountDistance>().totalDistance;
            totalDistR += trackedObjR.GetComponent<CountDistance>().totalDistance;
            totalDistH += trackedObjH.GetComponent<CountDistance>().totalDistance;
            totalTime += trackedObjH.GetComponent<CountDistance>().countTimer;
            print("Left Hand Movement:" + trackedObjL.GetComponent<CountDistance>().totalDistance);
            print("Right Hand Movement:" + trackedObjR.GetComponent<CountDistance>().totalDistance);
            print("Head Movement:" + trackedObjH.GetComponent<CountDistance>().totalDistance);
            print("Time taken:" + trackedObjH.GetComponent<CountDistance>().countTimer);

            print("OVERALL Left Hand Movement:"+ totalDistL);
            print("OVERALL Right Hand Movement:"+ totalDistR);
            print("OVERALL Head Movement:" + totalDistH);
            print("OVERALL Time Taken:" + totalTime);
            sceneCounter++;
            if (globalScript.recordController == globalDocker.RecordController.LEFT) {
                globalDocker.logInfo.Add(globalScript.selectionTechnique.name + "," + sceneCounter + "," + totalTime + ","+totalDistL + "," +0+ "," +totalDistH);
            } else if (globalScript.recordController == globalDocker.RecordController.RIGHT) {
                globalDocker.logInfo.Add(globalScript.selectionTechnique.name + "," + sceneCounter + "," + totalTime + ","+0 + "," +totalDistR+ "," +totalDistH);
            } else if (globalScript.recordController == globalDocker.RecordController.BOTH) {
                globalDocker.logInfo.Add(globalScript.selectionTechnique.name + "," + sceneCounter + "," + totalTime + ","+totalDistL + "," +totalDistR+ "," +totalDistH);
            }

            //DockerData.logInfo.Add("Technique, Stage, Time, LeftH_Movement, RightH_Movement, Head_Movement");
            trackedObjL.GetComponent<CountDistance>().resetProperties();
            trackedObjR.GetComponent<CountDistance>().resetProperties();
            trackedObjH.GetComponent<CountDistance>().resetProperties();

            totalDistL = 0f;
            totalDistR = 0f;
            totalDistH = 0f;
            totalTime = 0f;
            print("Completed docker task..");
            
            string currentSceneName = SceneManager.GetActiveScene().name;
            SceneManager.LoadScene(currentSceneName);
        }
    }

    // Use this for initialization
    void Start () {
        /* if (sceneCounter > 1) {
            outlineObjects[0].SetActive(true);
        }*/
        /* for (int i=0; i<4; i++) {
            if (sceneCounter-1 == i) {
                outlineObjects[i].SetActive(true);
            } else {
                outlineObjects[i].SetActive(false);
            }
        }*/
    //}
	
}
