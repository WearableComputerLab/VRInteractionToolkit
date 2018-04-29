using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spindle : MonoBehaviour {

    public bool spindleAndWheel = false;

    public SteamVR_TrackedObject trackedObj1;
    public SteamVR_TrackedObject trackedObj2;
    public GameObject interactionObject;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        setPositionOfInteraction();

    }

    void setPositionOfInteraction()
    {

        Vector3 midPoint = (trackedObj1.transform.position + trackedObj2.transform.position) / 2f;
        interactionObject.transform.position = midPoint;



        Vector3 newRotation = trackedObj2.transform.localEulerAngles;


        //interactionObject.transform.forward = trackedObj2.transform.forward;
        interactionObject.transform.LookAt(trackedObj2.transform);

        Vector3 rotation = new Vector3(0, 0, interactionObject.transform.eulerAngles.z + trackedObj2.transform.eulerAngles.z);

        interactionObject.transform.Rotate(rotation);

    }
}
