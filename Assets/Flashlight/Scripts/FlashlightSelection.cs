/*
 *  Script to be attatched to controllers to allow the flashlight
 *  for the VR technique for the Flaslight to pick up objects as described
 *  for the flaslight for the HTC Vive.
 *  
 *  Copyright(C) 2018  Ian Hanan
 *
 *  This program is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 * 
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with this program.If not, see<http://www.gnu.org/licenses/>.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 *  Keeping info on things I have done here for now
 *  So that each flashlight has no collision with eachother put them on seperate layers
*/
public class FlashlightSelection : MonoBehaviour {


    // Allows to choose if the script purley selects or has full manipulation
    public enum InteractionType { Selection, Manipulation };
    public InteractionType interactionType;
    public GameObject selection; // holds the selected object

    public SteamVR_TrackedObject theController;

    private GameObject trackedObj;
    private List<GameObject> collidingObjects;

    public Material highlightMaterial;
    private ObjectHighlighter highlighter;

    private GameObject objectInHand;

    public int[] layersOfObjectsToSelect;

    private GameObject objectHoveredOver;

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
        highlighter = new ObjectHighlighter(highlightMaterial);
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
        //trackedObj = GetComponent<SteamVR_TrackedObject>();
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
        highlighter.deHighlighobject(other.gameObject);
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

            // highlighting object
            if(objectInHand == null) {
                highlighter.highlightObject(collidingObjects[indexOfSmallest]);
            }

            return collidingObjects[indexOfSmallest];
        }
        return null;
    }

    private void GrabObject()
    {
        objectInHand = objectHoveredOver;

        if(objectHoveredOver != null) {
            collidingObjects.Remove(objectInHand);

            var joint = AddFixedJoint();
            joint.connectedBody = objectInHand.GetComponent<Rigidbody>();
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


            // TODO: Fix this so that it throws with the correct velocity applied
            //objectInHand.GetComponent<Rigidbody>().velocity = GetComponent<Rigidbody>().velocity;
            //objectInHand.GetComponent<Rigidbody>().angularVelocity = GetComponent<Rigidbody>().angularVelocity;

            
            objectInHand.GetComponent<Rigidbody>().velocity = Controller.velocity;
            objectInHand.GetComponent<Rigidbody>().angularVelocity = Controller.angularVelocity;
        }

        objectInHand = null;
    }

    // Update is called once per frame
    void Update()
    {

        objectHoveredOver = getObjectHoveringOver();
        
        
        print(collidingObjects.Count);
        if (Controller.GetHairTriggerDown())
        {
            if (collidingObjects.Count > 0)
            {
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
