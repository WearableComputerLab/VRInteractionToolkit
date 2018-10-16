using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerDropAction : MonoBehaviour {

    private SteamVR_Controller.Device deviceL;
    private SteamVR_Controller.Device deviceR;
    private SteamVR_TrackedObject trackedObjL;
    private SteamVR_TrackedObject trackedObjR;
    private DockerData dockerData;

    void OnTriggerStay(Collider col) {
        if (this.transform.name == col.transform.name) {
            if(deviceL != null && deviceL.GetPressUp(SteamVR_Controller.ButtonMask.Trigger) || deviceR != null && deviceR.GetPressUp(SteamVR_Controller.ButtonMask.Trigger)) {
                col.transform.position = this.transform.position;
                col.transform.rotation = this.transform.rotation;
                dockerData.incrementDockerCount();
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
    }
}
