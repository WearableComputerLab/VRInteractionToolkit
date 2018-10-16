using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DockerData : MonoBehaviour {

    public SteamVR_TrackedObject trackedObjL;
    public SteamVR_TrackedObject trackedObjR;

    internal int dockerCount = 0;
    private readonly int dockerObjects = 4;

    internal void incrementDockerCount() {
        dockerCount++;
        if (dockerCount >= dockerObjects) {
            print("Completed docker task..");
        }
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
