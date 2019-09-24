using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerDropAction : MonoBehaviour {
    /*
    private SteamVR_Controller.Device deviceL;
    private SteamVR_Controller.Device deviceR;
    private SteamVR_TrackedObject trackedObjL;
    private SteamVR_TrackedObject trackedObjR;
    private DockerData dockerData;

    private List<string> logInfo = new List<string>();

    void OnTriggerStay(Collider col) {
        //print("collding");
        if (this.transform.name == col.transform.name) {
            //print("trying to connect");
            //print("Device isnt null L" + deviceL != null);
            //print("Device isnt null R" + deviceR != null);
            if(deviceL != null && deviceL.GetPress(SteamVR_Controller.ButtonMask.Trigger) || deviceR != null && deviceR.GetPress(SteamVR_Controller.ButtonMask.Trigger)) {
                col.transform.position = this.transform.position;
                col.transform.rotation = this.transform.rotation;
                col.GetComponent<Rigidbody>().velocity = Vector3.zero;
                col.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
                if (col.transform.gameObject.GetComponent<Renderer>().material.color != Color.green) {
                    dockerData.incrementDockerCount();
                }
                col.transform.gameObject.GetComponent<Renderer>().material.color = Color.green;
            }
        }
    }

    void Start() {
        dockerData = this.transform.parent.GetComponent<DockerData>();
        trackedObjL = dockerData.trackedObjL;
        trackedObjR = dockerData.trackedObjR;
    }
	
	// Update is called once per frame
	void Update () {
        if((int)trackedObjL.index != -1) {
            deviceL = SteamVR_Controller.Input((int)trackedObjL.index);
        }
        if((int)trackedObjR.index != -1) {
            deviceR = SteamVR_Controller.Input((int)trackedObjR.index);
        }
    }*/
}
