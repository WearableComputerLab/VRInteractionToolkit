using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DockerData : MonoBehaviour {



    public SteamVR_TrackedObject trackedObjL;
    public SteamVR_TrackedObject trackedObjR;
    public SteamVR_TrackedObject trackedObjH;

    public static float totalDistL;
    public static float totalDistR;
    public static float totalDistH;

    internal int dockerCount = 0;
    private readonly int dockerObjects = 4;

    internal void incrementDockerCount() {
        dockerCount++;
        totalDistL += trackedObjL.GetComponent<CountDistance>().totalDistance;
        totalDistR += trackedObjR.GetComponent<CountDistance>().totalDistance;
        totalDistH += trackedObjH.GetComponent<CountDistance>().totalDistance;
        print("Left Hand Movement:" + trackedObjL.GetComponent<CountDistance>().totalDistance);
        print("Right Hand Movement:" + trackedObjR.GetComponent<CountDistance>().totalDistance);
        print("Head Movement:" + trackedObjH.GetComponent<CountDistance>().totalDistance);
        if (dockerCount == 1 && trackedObjL.GetComponent<CountDistance>().counting == false || dockerCount == 1 && trackedObjR.GetComponent<CountDistance>().counting == false) {
            trackedObjL.GetComponent<CountDistance>().counting = true;
            trackedObjR.GetComponent<CountDistance>().counting = true;
            trackedObjH.GetComponent<CountDistance>().counting = true;
        }
        if (dockerCount >= dockerObjects) {
            print("OVERALL Left Hand Movement:"+ totalDistL);
            print("OVERALL Right Hand Movement:"+ totalDistR);
            print("OVERALL Head Movement:" + totalDistH);
            trackedObjL.GetComponent<CountDistance>().resetDistance();
            trackedObjR.GetComponent<CountDistance>().resetDistance();
            trackedObjH.GetComponent<CountDistance>().resetDistance();
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
        
        
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
