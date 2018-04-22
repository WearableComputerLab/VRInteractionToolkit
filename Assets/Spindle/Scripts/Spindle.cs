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


        // Test forward
        Vector3 theVector = trackedObj2.transform.forward;
        Vector3 pose = new Vector3();

        float distance_formula_on_vector = Mathf.Sqrt(theVector.x * theVector.x + theVector.y * theVector.y + theVector.z * theVector.z);
        // Using formula to find a point which lies at distance on a 3D line from vector and direction
        pose.x = midPoint.x + (10f / (distance_formula_on_vector)) * theVector.x;
        pose.y = midPoint.y + (10f / (distance_formula_on_vector)) * theVector.y;
        pose.z = midPoint.z + (10f / (distance_formula_on_vector)) * theVector.z;



        Vector3 targetPostition = new Vector3(trackedObj1.transform.position.x,
                                       trackedObj1.transform.position.y,
                                       interactionObject.transform.position.z);
        interactionObject.transform.LookAt(targetPostition);

        
            
    }
}
