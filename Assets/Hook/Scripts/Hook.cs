using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

// Information on hook technique
// http://www.eecs.ucf.edu/isuelab/publications/pubs/Cashion_Jeffrey_A_201412_PhD.pdf pf 13

public class Hook : MonoBehaviour {

    // User can specify layers of objects that the hook will be able to select
    public int[] layersOfObjectsToSelect;

    // Allows to choose if the script purley selects or has full manipulation
    public enum InteractionType { Selection, Manipulation };
    public InteractionType interactionType;
    public GameObject selection; // holds the selected object

    //  Used lists instead of arrays incase we want future optimization where it dynamically changes
    //  Instead of looping through every object in the scene
    List<HookObject> nearbyObjects;
    int countOfIncreases;

    private GameObject objectInHand;
    public SteamVR_TrackedObject trackedObj;
    public bool checkForNewlySpawnedObjects = true;

    private SteamVR_Controller.Device Controller
    {
        get { return SteamVR_Controller.Input((int)trackedObj.index); }
    }

    // Use this for initialization
    void Start () {
        nearbyObjects = new List<HookObject>();
        populateGameObjectList();
        // Set count of Increases to 1/3 of the list (int incase is odd and cannot use odd)
        countOfIncreases = nearbyObjects.Count / 3;
    }
	
	// Update is called once per frame
	void Update () {
		foreach(HookObject each in nearbyObjects)
        {
            each.setDistance(this.gameObject);
        }
        nearbyObjects = nearbyObjects.OrderBy(w => w.lastDistance).ToList();
        // now increase or decrease score 
        int count = 0;
        foreach(HookObject each in nearbyObjects)
        {
            if (count < countOfIncreases)
            {
                each.increaseScore();
                count++;
            } else
            {
                each.decreaseScore();
            }
        }

        // Pressing trigger to grab object
        if (Controller.GetHairTriggerDown())
        {
            GrabObject();
        }
        if (Controller.GetHairTriggerUp())
        {
            if (objectInHand)
            {
                ReleaseObject();
            }
        }

        // printing list for testing
        print(nearbyObjects.Count);
	}

    void populateGameObjectList()
    {
        var allObjects = FindObjectsOfType<GameObject>();
        foreach (GameObject each in allObjects)
        {
            
            if (layersOfObjectsToSelect.Contains(each.layer)) //only works on selectable objects.
            {
                nearbyObjects.Add(new HookObject(each));
            }
        }
    }

    private void GrabObject()
    {
        if(nearbyObjects.Count > 0)
        {
            GameObject objectToSelect = nearbyObjects.ElementAt<HookObject>(0).ContainingObject;
            if(interactionType == InteractionType.Selection) {
                // Pure selection
                print("selected " + objectToSelect);
                selection = objectToSelect;
            } else {
                // Manipulation
                objectInHand = objectToSelect;
                objectInHand.transform.position = trackedObj.transform.position;

                var joint = AddFixedJoint();
                joint.connectedBody = objectInHand.GetComponent<Rigidbody>();
            }           
        }
    }


    private FixedJoint AddFixedJoint()
    {
        FixedJoint fx = gameObject.AddComponent<FixedJoint>();
        fx.breakForce = Mathf.Infinity;
        fx.breakTorque = Mathf.Infinity;
        return fx;
    }

    private void ReleaseObject()
    {
        if (GetComponent<FixedJoint>())
        {

            GetComponent<FixedJoint>().connectedBody = null;
            Destroy(GetComponent<FixedJoint>());

            objectInHand.GetComponent<Rigidbody>().velocity = Controller.velocity;
            objectInHand.GetComponent<Rigidbody>().angularVelocity = Controller.angularVelocity;
        }

        objectInHand = null;
    }

}
