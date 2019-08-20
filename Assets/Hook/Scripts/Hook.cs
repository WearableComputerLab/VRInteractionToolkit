using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Events;
using Valve.VR;

// Information on hook technique
// http://www.eecs.ucf.edu/isuelab/publications/pubs/Cashion_Jeffrey_A_201412_PhD.pdf pf 13

// TODO: Highlight closest object maybe?

public class Hook : MonoBehaviour {
#if SteamVR_Legacy
    public SteamVR_TrackedObject trackedObj = null;
    private SteamVR_Controller.Device Controller {
        get {
            return SteamVR_Controller.Input((int)trackedObj.index);
        }
    }
#elif SteamVR_2
    public SteamVR_Action_Boolean m_controllerPress;
    public SteamVR_Behaviour_Pose trackedObj = null;
#else
    public GameObject trackedObj = null;
#endif

    // User can specify layers of objects that the hook will be able to select
    public LayerMask interactionLayers;

    // Allows to choose if the script purley selects or has full manipulation
    public enum InteractionType { Selection, Manipulation };
    public InteractionType interactionType;
    public GameObject selection; // holds the selected object

    //  Used lists instead of arrays incase we want future optimization where it dynamically changes
    //  Instead of looping through every object in the scene
    private List<HookObject> nearbyObjects;
    int countOfIncreases;
    private GameObject objectInHand;
    public bool checkForNewlySpawnedObjects = true;
    public GameObject currentlyHovered = null; // To hold the closest object
    public UnityEvent selectedObject; // Invoked when an object is selected

    public UnityEvent hovered; // Invoked when an object is hovered by technique
    public UnityEvent unHovered; // Invoked when an object is no longer hovered by the technique
    private GameObject lastHovered = null; // To check if changed


    // Use this for initialization
    void Start() {
        nearbyObjects = new List<HookObject>();
        populateGameObjectList();
        // Set count of Increases to 1/3 of the list (int incase is odd and cannot use odd)
        countOfIncreases = nearbyObjects.Count / 3;

        // Controller needs a rigidbody to grab objects in hook
        Rigidbody body = trackedObj.gameObject.AddComponent<Rigidbody>();
        body.isKinematic = true;
    }

    public enum ControllerState {
        TRIGGER_UP, TRIGGER_DOWN, NONE
    }

    private ControllerState controllerEvents() {
#if SteamVR_Legacy
        if (Controller.GetHairTriggerDown()) {
            return ControllerState.TRIGGER_DOWN;
        }
        if (Controller.GetHairTriggerUp()) {
            return ControllerState.TRIGGER_UP;
        }
#elif SteamVR_2
        if (m_controllerPress.GetStateDown(trackedObj.inputSource)) {
            return ControllerState.TRIGGER_DOWN;
        }
        if (m_controllerPress.GetStateUp(trackedObj.inputSource)) {
            return ControllerState.TRIGGER_UP;
        }
#endif
        return ControllerState.NONE;
    }

    // Update is called once per frame
    void Update() {
        updateHovered();
        updateHookList();

        // Pressing trigger to grab object
        if (controllerEvents() == ControllerState.TRIGGER_DOWN) {
            GrabObject();
        }
        if (controllerEvents() == ControllerState.TRIGGER_UP) {
            if (objectInHand) {
                ReleaseObject();
            }
        }
        // printing list for testing
        print("Nearby objects: " + nearbyObjects.Count);
    }

    private void updateHookList() {
        // Remove any null objects from list if they were destroyed
        int listLength = nearbyObjects.Count;
        for (int i = 0; i < listLength; i++) {
            if (!nearbyObjects[i].checkStillExists()) {
                // doesnt exist anymore so remove
                nearbyObjects.RemoveAt(i);
                print("removed");
            }
            listLength--;
        }

        // Check all the objects still exist
        foreach (HookObject each in nearbyObjects) {
            each.setDistance(trackedObj.gameObject);
        }
        // Reordering all the nearbyobjects by their newly set distance
        nearbyObjects = nearbyObjects.OrderBy(w => w.lastDistance).ToList();
        // now increase or decrease score 
        int count = 0;
        foreach (HookObject each in nearbyObjects) {
            if (count < countOfIncreases) {
                each.increaseScore();
                count++;
            } else {
                each.decreaseScore();
            }
        }
    }

    private void updateHovered() {
        currentlyHovered = nearbyObjects.ElementAt<HookObject>(0).ContainingObject;
        if (currentlyHovered != null && currentlyHovered != lastHovered) {
            // Hovering a new object 
            unHovered.Invoke();
            hovered.Invoke();
        }
    }

    void populateGameObjectList() {
        var allObjects = FindObjectsOfType<GameObject>();
        foreach (GameObject each in allObjects) {
            if (interactionLayers == (interactionLayers | (1 << each.layer))) //only works on selectable objects.
            {
                nearbyObjects.Add(new HookObject(each));
            }
        }
    }

    // If for example new objects are spaned to the scene the user can access the hook with that object and add that object to the hook with this method
    public void addNewlySpawnedObjectToHook(GameObject newObject) {
        nearbyObjects.Add(new HookObject(newObject));
    }

    private void GrabObject() {
        if (nearbyObjects.Count > 0) {
            GameObject objectToSelect = currentlyHovered;
            if (interactionType == InteractionType.Selection) {
                // Pure selection
                selection = objectToSelect;
            } else {
                // Manipulation
                selection = objectToSelect;
                objectInHand = objectToSelect;
                objectInHand.transform.position = trackedObj.transform.position;

                var joint = AddFixedJoint();
                joint.connectedBody = objectInHand.GetComponent<Rigidbody>();
                joint.connectedBody.useGravity = false; // turn of gravity while grabbing
            }
            selectedObject.Invoke();
        }
    }


    private FixedJoint AddFixedJoint() {
        FixedJoint fx = trackedObj.gameObject.AddComponent<FixedJoint>();
        fx.breakForce = Mathf.Infinity;
        fx.breakTorque = Mathf.Infinity;
        return fx;
    }

    private void ReleaseObject() {
        if (trackedObj.GetComponent<FixedJoint>()) {
            trackedObj.GetComponent<FixedJoint>().connectedBody = null;
            Destroy(trackedObj.GetComponent<FixedJoint>());
            objectInHand.GetComponent<Rigidbody>().useGravity = true;
#if SteamVR_Legacy
            objectInHand.GetComponent<Rigidbody>().velocity = Controller.velocity;
            objectInHand.GetComponent<Rigidbody>().angularVelocity = Controller.angularVelocity;
#elif SteamVR_2
            objectInHand.GetComponent<Rigidbody>().velocity = trackedObj.GetVelocity();
            objectInHand.GetComponent<Rigidbody>().angularVelocity = trackedObj.GetAngularVelocity();
#endif
        }
        objectInHand = null;
    }
}
