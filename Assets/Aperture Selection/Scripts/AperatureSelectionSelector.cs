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

    public List<int> layersOfObjectsToSelect;

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
        List<GameObject> viableObjects = new List<GameObject>();

        Vector3 forwardVectorFromRemote = trackedObj.transform.forward;
        Vector3 positionOfRemote = trackedObj.transform.position;

        foreach (GameObject potentialObject in collidingObjects)
        {
            
            // dont have to worry about executing twice as an object can only be on one layer
            if (layersOfObjectsToSelect.Contains(potentialObject.layer))
            {
                // Object can only have one layer so can do calculation for object here
                Vector3 objectPosition = potentialObject.transform.position;

                // Using vector algebra to get shortest distance between object and vector 
                Vector3 forwardControllerToObject = trackedObj.transform.position - objectPosition;
                Vector3 controllerForward = trackedObj.transform.forward;
                float distanceBetweenRayAndPoint = Vector3.Magnitude(Vector3.Cross(forwardControllerToObject,controllerForward))/Vector3.Magnitude(controllerForward);               
                
                distancesFromCenterOfCone.Add(distanceBetweenRayAndPoint);
                viableObjects.Add(potentialObject);
            }
            
        }

        if(viableObjects.Count > 0 && distancesFromCenterOfCone.Count > 0)
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

            if(objectHoveredOver != viableObjects[indexOfSmallest]) {
                unHovered.Invoke();
            }
            
            return viableObjects[indexOfSmallest];
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

            objectInHand.GetComponent<Rigidbody>().velocity = Controller.velocity * Vector3.Distance(Controller.transform.pos, objectInHand.transform.position);
            objectInHand.GetComponent<Rigidbody>().angularVelocity = Controller.angularVelocity;
        }

        objectInHand = null;
    }

    // Update is called once per frame
    void Update()
    {
        objectHoveredOver = getObjectHoveringOver();
        hovered.Invoke();
        
        print(collidingObjects.Count);
        if (Controller.GetHairTriggerDown())
        {
            if (collidingObjects.Count > 0)
            {
                selectedObject.Invoke();
                if(interactionType == InteractionType.Selection) {
                    // Pure selection
                    selection = objectHoveredOver;
                } else if(interactionType == InteractionType.Manipulation) {
                    //Manipulation
                    GrabObject();
                }    
                selection = objectHoveredOver;         
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
