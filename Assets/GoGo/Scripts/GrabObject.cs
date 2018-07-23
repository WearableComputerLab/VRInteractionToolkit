using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabObject : MonoBehaviour {

     // Allows to choose if the script purley selects or has full manipulation
    public enum InteractionType { Selection, Manipulation };
    public InteractionType interactionType;
    public GameObject selection; // holds the selected object

    public SteamVR_TrackedObject trackedObj;
    private GameObject collidingObject;
    private GameObject objectInHand;

    public Material MaterialToHighlightObjects;
    private Material unhighlightedObject;

    private SteamVR_Controller.Device Controller
    {
        get { return SteamVR_Controller.Input((int)trackedObj.index); }
    }

    void OnEnable()
    {
        var render = SteamVR_Render.instance;
        if (render == null)
        {
            enabled = false;
            return;
        }
    }

    void Awake()
    {
        //trackedObj = GetComponent<SteamVR_TrackedObject>();
    }

    private void SetCollidingObject(Collider col)
    {

        if (collidingObject || !col.GetComponent<Rigidbody>())
        {
            return;
        }

        collidingObject = col.gameObject;
    }


    public void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == 8 && objectInHand == null)
        {
            unhighlightedObject = other.GetComponent<Renderer>().material;
            other.GetComponent<Renderer>().material = MaterialToHighlightObjects;
        }
        
        
        SetCollidingObject(other);
    }


    public void OnTriggerStay(Collider other)
    {
        SetCollidingObject(other);
    }


    public void OnTriggerExit(Collider other)
    {
        if (!collidingObject)
        {
            return;
        }
        if(other.gameObject.layer == 8)
        {
            other.GetComponent<Renderer>().material = unhighlightedObject;
        }
        
        collidingObject = null;
    }

    private void pickUpObject()
    {
        
        objectInHand = collidingObject;
        if (objectInHand.layer == 8)
        {
            objectInHand.GetComponent<Renderer>().material = unhighlightedObject;
        }
        collidingObject = null;

        var joint = AddFixedJoint();
        joint.connectedBody = objectInHand.GetComponent<Rigidbody>();
    }

    private FixedJoint AddFixedJoint()
    {
        FixedJoint fx = gameObject.AddComponent<FixedJoint>();
        fx.breakForce = 20000;
        fx.breakTorque = 20000;
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

    // Update is called once per frame
    void Update () {
        if (Controller.GetHairTriggerDown())
        {
            if (collidingObject)
            {
                if(interactionType == InteractionType.Selection) {
                    // Pure selection
                    print("selected " + collidingObject);
                    selection = collidingObject;
                } else {
                    // Manipulation
                    pickUpObject();
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
