using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DockerData : MonoBehaviour {



    public SteamVR_TrackedObject trackedObjL;
    public SteamVR_TrackedObject trackedObjR;
    
    internal int dockerCount = 0;
    private readonly int dockerObjects = 4;

    internal void incrementDockerCount() {
        dockerCount++;
        if (dockerCount >= dockerObjects) {
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
