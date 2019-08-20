using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class Spindle : MonoBehaviour {

#if SteamVR_Legacy
    public SteamVR_TrackedObject trackedObj1;
    public SteamVR_TrackedObject trackedObj2;
#elif SteamVR_2
    public SteamVR_Behaviour_Pose trackedObj1;
    public SteamVR_Behaviour_Pose trackedObj2;
#else
    public GameObject trackedObj1;
    public GameObject trackedObj2;
#endif

    public bool spindleAndWheel = false;
    public GameObject interactionObject;

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

        Vector3 rotation;
        if(spindleAndWheel) {
            rotation = new Vector3(0, 0, interactionObject.transform.eulerAngles.z + trackedObj2.transform.eulerAngles.z);
        } else {
            rotation = new Vector3(0,0,0);
        }
        

        interactionObject.transform.Rotate(rotation);

    }
}
