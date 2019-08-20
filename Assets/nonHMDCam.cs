using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class nonHMDCam : MonoBehaviour {
    public Camera cam;
#if SteamVR_Legacy
     public SteamVR_TrackedObject trackedObj;
#elif SteamVR_2
    public SteamVR_Behaviour_Pose trackedObj;
#else
    public GameObject trackedObj;
#endif


    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        cam.transform.parent.transform.rotation = trackedObj.transform.rotation;
        cam.transform.parent.transform.position = new Vector3(trackedObj.transform.position.x, trackedObj.transform.position.y, -2f);
        //Vector3 zOff = new Vector3(trackedObj.transform.position.x, trackedObj.transform.position.y, trackedObj.transform.position.z - 4f);
        //cam.transform.parent.transform.position = Vector3.Lerp(trackedObj.transform.position, zOff, 0.2f);
    }
}
