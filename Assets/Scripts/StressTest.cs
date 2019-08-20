using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StressTest : MonoBehaviour {
    /*
    public int ObjectsSpawnCount = 50;
    private int objectsSpawned = 0;
    private SteamVR_TrackedObject trackedObj;
    private SteamVR_Controller.Device device;
    public GameObject frameRateText;
	// Use this for initialization
	void Start () {
        trackedObj = this.GetComponent<SteamVR_TrackedObject>();
        InvokeRepeating("UpdateFrames", 0f, 0.1f);
	}

    private void OnApplicationQuit() {
        print("Application ended after " + Time.time + " seconds");
        print("Final FPS: " + 1.0f / Time.deltaTime);
        print("Objects spawned: " + objectsSpawned);
    }

    void spawnObjects() {
        if (device.GetPressDown(SteamVR_Controller.ButtonMask.ApplicationMenu)) {
            for (int i=0; i<ObjectsSpawnCount; i++) {
                float y = Random.Range(6, 30);
                float z = Random.Range(-50, 50);
                float x = Random.Range(-65, 35);
                float s = Random.Range(0.5f, 5f);
                GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                sphere.transform.position = new Vector3(x, y, z);
                sphere.transform.localScale = new Vector3(s, s, s);
                objectsSpawned++;
            }
        }
    }

    float tempTime;
	// Update is called once per frame
	void Update () {
        tempTime += Time.deltaTime;
        if (tempTime > 0.1) {
            tempTime = 0;
            frameRateText.GetComponent<TextMesh>().text = "FPS: " + 1.0f / Time.deltaTime;
        }

        print("Current Frames:"+ 1.0f / Time.deltaTime);
        device = SteamVR_Controller.Input((int)trackedObj.index);
        spawnObjects();

    }*/
}
