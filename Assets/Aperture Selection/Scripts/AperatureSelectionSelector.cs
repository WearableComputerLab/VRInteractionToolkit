using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AperatureSelectionSelector : MonoBehaviour {


    // Allows to choose if the script purley selects or has full manipulation
    public enum InteractionType { Selection, Manipulation };
    public InteractionType interactionType;
    public GameObject selection; // holds the selected object

    public SteamVR_TrackedObject theController;

    private GameObject trackedObj;
    private List<GameObject> collidingObjects;

    public UnityEvent selectedObject; // Invoked when an object is selected

    public UnityEvent hovered; // Invoked when an object is hovered by technique
    public UnityEvent unHovered; // Invoked when an object is no longer hovered by the technique

    private GameObject objectInHand;

    public int[] layersOfObjectsToSelect;

    public GameObject objectHoveredOver;

    private SteamVR_Controller.Device Controller
    {
        get { return SteamVR_Controller.Input((int)theController.index); }
    }

    // Checks if holding object in hand
    public bool holdingObject()
    {
        return objectInHand != null;
    }

    void OnEnable()
    {
        collidingObjects = new List<GameObject>();
        trackedObj = this.transform.gameObject;
        var render = SteamVR_Render.instance;
        if (render == null)
        {
            enabled = false;
            return;
        }
    }

    void Awake()
    {
        
    }

    private void SetCollidingObject(Collider col)
    {
        if (collidingObjects.Contains(col.gameObject) || !col.GetComponent<Rigidbody>())
        {
            return;
        }
        collidingObjects.Add(col.gameObject);
    }


    public void OnTriggerEnter(Collider other)
    {
        SetCollidingObject(other);
    }


    public void OnTriggerStay(Collider other)
    {
        SetCollidingObject(other);
    }


    public void OnTriggerExit(Collider other)
    {
        if (!collidingObjects.Contains(other.gameObject))
        {
            return;
        }
        collidingObjects.Remove(other.gameObject);
    }

    private GameObject getObjectHoveringOver()
    {
        List<double> distancesFromCenterOfCone = new List<double>();

        Vector3 forwardVectorFromRemote = trackedObj.transform.forward;
        Vector3 positionOfRemote = trackedObj.transform.position;

        foreach (GameObject potentialObject in collidingObjects)
        {
            // Only doing if the object is on a layer where the object can be picked up
            for (int i = 0; i < layersOfObjectsToSelect.Length; i++)
            {
                // dont have to worry about executing twice as an object can only be on one layer
                if (potentialObject.layer == layersOfObjectsToSelect[i])
                {
                    // Object can only have one layer so can do calculation for object here
                    Vector3 objectPosition = potentialObject.transform.position;

                    // Finding closest to ray by creating a perpendicular plane using the formula that uses the point and then finds the distance between
                    // that point and where a plane created from the vector intersects the laser
                    float tValueFromFormulaExplained = (forwardVectorFromRemote.x * objectPosition.x + forwardVectorFromRemote.x * objectPosition.y
                        + forwardVectorFromRemote.z * objectPosition.z - forwardVectorFromRemote.x * positionOfRemote.x - forwardVectorFromRemote.y * positionOfRemote.y
                        - forwardVectorFromRemote.z * positionOfRemote.z) / (Mathf.Pow(forwardVectorFromRemote.x, 2) + Mathf.Pow(forwardVectorFromRemote.y, 2) + Mathf.Pow(forwardVectorFromRemote.z, 2));

                    Vector3 newPoint = new Vector3(forwardVectorFromRemote.x * tValueFromFormulaExplained + positionOfRemote.x, forwardVectorFromRemote.y * tValueFromFormulaExplained + positionOfRemote.y
                        , forwardVectorFromRemote.z * tValueFromFormulaExplained + positionOfRemote.z);

                    double distanceBetweenRayAndPoint = Mathf.Sqrt(Mathf.Pow(newPoint.x - objectPosition.x, 2) + Mathf.Pow(newPoint.y - objectPosition.y, 2) + Mathf.Pow(newPoint.z - objectPosition.z, 2));
                    distancesFromCenterOfCone.Add(distanceBetweenRayAndPoint);
                }
            }
        }

        if(collidingObjects.Count > 0 && distancesFromCenterOfCone.Count > 0)
        {
            // Find the smallest object by distance
            int indexOfSmallest = 0;
            double smallest = distancesFromCenterOfCone[0];
            for (int index = 0; index < distancesFromCenterOfCone.Count; index++)
            {
                if (distancesFromCenterOfCone[index] < smallest)
                {
                    indexOfSmallest = index;
                    smallest = distancesFromCenterOfCone[index];
                }
            }

            if(objectHoveredOver != collidingObjects[indexOfSmallest]) {
                unHovered.Invoke();
            } 
            hovered.Invoke();
            
            return collidingObjects[indexOfSmallest];
        }
        unHovered.Invoke();
        return null;
    }

    private void GrabObject()
    {
        objectInHand = objectHoveredOver;

        if(objectHoveredOver != null) {
            collidingObjects.Remove(objectInHand);

            var joint = AddFixedJoint();
            objectInHand.GetComponent<Rigidbody>().velocity = Vector3.zero; // Setting velocity to 0 so can catch without breakforce effecting it
            joint.connectedBody = objectInHand.GetComponent<Rigidbody>();
        }
    }

    private FixedJoint AddFixedJoint()
    {
        
        FixedJoint fx = gameObject.AddComponent<FixedJoint>();
        fx.breakForce = 1000;
        fx.breakTorque = Mathf.Infinity;
        return fx;
    }

    private void ReleaseObject()
    {

        if (GetComponent<FixedJoint>())
        {

            GetComponent<FixedJoint>().connectedBody = null;
            Destroy(GetComponent<FixedJoint>());


            // TODO: Fix this so that it throws with the correct velocity applied
            //objectInHand.GetComponent<Rigidbody>().velocity = GetComponent<Rigidbody>().velocity;
            //objectInHand.GetComponent<Rigidbody>().angularVelocity = GetComponent<Rigidbody>().angularVelocity;

            
            //objectInHand.GetComponent<Rigidbody>().velocity = Controller.velocity;
            //objectInHand.GetComponent<Rigidbody>().angularVelocity = Controller.angularVelocity;

            objectInHand.GetComponent<Rigidbody>().velocity = Controller.velocity * Vector3.Distance(Controller.transform.pos, objectInHand.transform.position);
            objectInHand.GetComponent<Rigidbody>().angularVelocity = Controller.angularVelocity;
        }

        objectInHand = null;
    }

    // Update is called once per frame
    void Update()
    {
        print("collding objects: " + collidingObjects.Count);
        objectHoveredOver = getObjectHoveringOver();
        
        print(collidingObjects.Count);
        if (Controller.GetHairTriggerDown())
        {
            if (collidingObjects.Count > 0)
            {
                selectedObject.Invoke();
                if(interactionType == InteractionType.Selection) {
                    // Pure selection
                    print("selected " + objectHoveredOver);
                    selection = objectHoveredOver;
                } else if(interactionType == InteractionType.Manipulation) {
                    //Manipulation
                    GrabObject();
                }             
            }
        }


        if (Controller.GetHairTriggerUp())
        {
            if (objectInHand)
            {
                ReleaseObject();
            }
        }
    }
}
