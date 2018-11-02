/*
 *  BendCast - Similar to a ray-cast except it will bend towards the closest object
 *  VR Interaction technique for the HTC Vive.
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
using Valve.VR;
using Valve.VR.InteractionSystem;
using UnityEngine.Events;
using System;

public class BendCast : MonoBehaviour
{
	public LayerMask interactionLayers;

    public GameObject leftController; // Reference to the steam VR left controller
    public GameObject rightController; // Reference to the steam VR right controller

    public enum SetController {left, right, notSet}; // So the player can choose which controller this bendcast will use
    public SetController setController = SetController.left; // the set controller

    private SteamVR_TrackedObject trackedObj;  // used by the script to keep track of the controller

    // Allows to choose if the script purley selects or has full manipulation
    public enum InteractionType { Selection, Manipulation };
    public InteractionType interactionType;
    public GameObject lastSelectedObject; // holds the selected object

    public GameObject currentlyPointingAt;
    private Vector3 castingBezierFrom;

    // Bend in ray is built from multiple other rays
    private int numOfLasers = 20; // how many rays to use for the bend (the more the smoother) MUST BE EVEN
    public GameObject laserPrefab;
    private GameObject[] lasers;
    private Transform[] laserTransform;
    private Vector3 hitPoint;

    private Vector3 p1PointLocation; // used fot the bezier curve

	public UnityEvent selectedObject; // Invoked when an object is selected

	public UnityEvent hovered; // Invoked when an object is hovered by technique
	public UnityEvent unHovered; // Invoked when an object is no longer hovered by the technique



    private SteamVR_Controller.Device Controller
    {
        get { return SteamVR_Controller.Input((int)trackedObj.index); }
    }

    private GameObject laserHolderGameobject;

    // Use this for initialization
    void Start()
    {
        if(setController == SetController.left) {
            trackedObj = leftController.GetComponent<SteamVR_TrackedObject>();
        } else if (setController == SetController.right) {
            trackedObj = rightController.GetComponent<SteamVR_TrackedObject>();
        }

        // Initalizing all the lasers
        laserHolderGameobject = new GameObject();
        laserHolderGameobject.transform.parent = this.transform;
        laserHolderGameobject.gameObject.name = trackedObj.name + " Laser Rays";

        lasers = new GameObject[numOfLasers];
        laserTransform = new Transform[numOfLasers];
        for (int i = 0; i < numOfLasers; i++)
        {
            GameObject laserPart = Instantiate(laserPrefab, new Vector3((float)i, 1, 0), Quaternion.identity) as GameObject;
            laserTransform[i] = laserPart.transform;
            lasers[i] = laserPart;
            laserPart.transform.parent = laserHolderGameobject.transform;
        }
    }
    void Awake()
    {

    }

    // Update is called once per frame
    void Update()
    {
        checkSurroundingObjects();
        castLaserCurve();

        if(Controller.GetHairTriggerDown() && currentlyPointingAt != null) {
            lastSelectedObject = currentlyPointingAt;
            if(interactionType == InteractionType.Selection) {
                // Pure Selection            
                
                print("selected" + currentlyPointingAt);
                
            } else if(interactionType == InteractionType.Manipulation) {

            }
            selectedObject.Invoke();
        }
        if (Controller.GetHairTriggerUp())
        {
            if (lastSelectedObject != null)
            {
                ReleaseObject();
            }
        }
    }

    private void ReleaseObject()
    {

        if (GetComponent<FixedJoint>())
        {
 
            GetComponent<FixedJoint>().connectedBody = null;
            Destroy(GetComponent<FixedJoint>());
       
            lastSelectedObject.GetComponent<Rigidbody>().velocity = Controller.velocity;
            lastSelectedObject.GetComponent<Rigidbody>().angularVelocity = Controller.angularVelocity;
        }
    }

    // Using a bezier! Idea from doing flexible pointer
    Vector3 GetBezierPosition(float t)
    {
        if (currentlyPointingAt == null)
        {
            // Fix for more appropriate later
            return new Vector3(0, 0, 0);
        }


        Vector3 p0 = castingBezierFrom;
        Vector3 p2 = currentlyPointingAt.transform.position;

        return Mathf.Pow(1f - t, 2f) * p0 + 2f * (1f - t) * t * p1PointLocation + Mathf.Pow(t, 2) * p2;
    }

    void castLaserCurve()
    {
        float valueToSearchBezierBy = 0f;
        Vector3 positionOfLastLaserPart = castingBezierFrom;

        valueToSearchBezierBy += (1f / numOfLasers);

        for (int i = 0; i < numOfLasers; i++)
        {
            lasers[i].SetActive(true);
            Vector3 pointOnBezier = GetBezierPosition(valueToSearchBezierBy);
            Vector3 nextPart = new Vector3(pointOnBezier.x, pointOnBezier.y, pointOnBezier.z);
            float distBetweenParts = Vector3.Distance(nextPart, positionOfLastLaserPart);

            laserTransform[i].position = Vector3.Lerp(positionOfLastLaserPart, nextPart, .5f);
            laserTransform[i].LookAt(nextPart);
            laserTransform[i].localScale = new Vector3(laserTransform[i].localScale.x, laserTransform[i].localScale.y, distBetweenParts);
            positionOfLastLaserPart = nextPart;

            valueToSearchBezierBy += (1f / numOfLasers);
        }
    }


    void checkSurroundingObjects()
    {

        Vector3 forwardVectorFromRemote = trackedObj.transform.forward;
        Vector3 positionOfRemote = trackedObj.transform.position;

        // This way is quite innefficient but is the way described for the bendcast.
        // Might make an example of a way that doesnt loop through everything
        var allObjects = FindObjectsOfType<GameObject>();

        float shortestDistance = float.MaxValue;

        GameObject objectWithShortestDistance = null;
        // Loop through objects and look for closest (if of a viable layer)
        for (int i = 0; i < allObjects.Length; i++)
        {
            // dont have to worry about executing twice as an object can only be on one layer
			if (interactionLayers == (interactionLayers | (1 << allObjects[i].layer)))
            {
                // Check if object is on plane projecting in front of VR remote. Otherwise ignore it. (we dont want our laser aiming backwards)
                Vector3 forwardParallelToDirectionPointing = Vector3.Cross(forwardVectorFromRemote, trackedObj.transform.up);
                Vector3 targObject = trackedObj.transform.position-allObjects[i].transform.position;
                Vector3 perp = Vector3.Cross(forwardParallelToDirectionPointing, targObject);
                float side = Vector3.Dot(perp, trackedObj.transform.up);
                if(side < 0) {
                        // Object can only have one layer so can do calculation for object here
                    Vector3 objectPosition = allObjects[i].transform.position;

                    // Using vector algebra to get shortest distance between object and vector 
                    Vector3 forwardControllerToObject = trackedObj.transform.position - objectPosition;
                    Vector3 controllerForward = trackedObj.transform.forward;
                    float distanceBetweenRayAndPoint = Vector3.Magnitude(Vector3.Cross(forwardControllerToObject,controllerForward))/Vector3.Magnitude(controllerForward);
                    
        

                    Vector3 newPoint = new Vector3(forwardVectorFromRemote.x * distanceBetweenRayAndPoint + positionOfRemote.x, forwardVectorFromRemote.y * distanceBetweenRayAndPoint + positionOfRemote.y
                            , forwardVectorFromRemote.z * distanceBetweenRayAndPoint + positionOfRemote.z);

                    if (distanceBetweenRayAndPoint < shortestDistance)
                    {
                        shortestDistance = distanceBetweenRayAndPoint;
                        objectWithShortestDistance = allObjects[i];
                        p1PointLocation = newPoint;
                    }
                }
                
            }         
        }
        if (objectWithShortestDistance != null)
        {
            // Activiating laser gameobject in case it isnt active
            laserHolderGameobject.SetActive(true);
            
            // Invoke un-hover if object with shortest distance is now different to currently hovered
            if(currentlyPointingAt != objectWithShortestDistance) {
                unHovered.Invoke();
            }

            // setting the object that is being pointed at
            currentlyPointingAt = objectWithShortestDistance;
            
            hovered.Invoke(); // Broadcasting that object is hovered

            castingBezierFrom = trackedObj.transform.position;

        } else {
            // Laser didnt reach any object so will disable
            laserHolderGameobject.SetActive(false);
            currentlyPointingAt = null;
            lastSelectedObject = null;
        }
    }
}
