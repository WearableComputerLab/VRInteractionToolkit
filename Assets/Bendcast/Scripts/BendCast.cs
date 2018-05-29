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

public class BendCast : MonoBehaviour
{

    public Material MaterialToHighlightObjects;
    private Material unhighlightedObject;

    private SteamVR_TrackedObject trackedObj;
    private GameObject currentlyPointingAt;
    private Vector3 castingBezierFrom;

    // Bend in ray is built from multiple other rays
    private int numOfLasers = 20; // how many rays to use for the bend (the more the smoother) MUST BE EVEN
    public GameObject laserPrefab;
    private GameObject[] lasers;
    private Transform[] laserTransform;
    private Vector3 hitPoint;

    private Vector3 p1PointLocation;

    public int[] layersOfObjectsToBendTo;
    // Use this for initialization
    void Start()
    {
        // Initalizing all the lasers
        GameObject laserHolder = new GameObject();
        laserHolder.name = "LaserRays";

        lasers = new GameObject[numOfLasers];
        laserTransform = new Transform[numOfLasers];
        for (int i = 0; i < numOfLasers; i++)
        {
            GameObject laserPart = Instantiate(laserPrefab, new Vector3((float)i, 1, 0), Quaternion.identity) as GameObject;
            laserTransform[i] = laserPart.transform;
            lasers[i] = laserPart;
            laserPart.transform.parent = laserHolder.transform;
        }
    }
    void Awake()
    {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
    }

    // Update is called once per frame
    void Update()
    {
        checkSurroundingObjects();
        castLaserCurve();
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

        // p1 is the closest point on the forward vector line coming out of the remote to the object
        //Vector3 p1 = p2 - currentlyPointingAt.transform.forward;

        return Mathf.Pow(1f - t, 2f) * p0 + 2f * (1f - t) * t * p1PointLocation + Mathf.Pow(t, 2) * p2;
    }

    float distBetweenVectors(Vector3 one, Vector3 two)
    {
        return Mathf.Sqrt(Mathf.Pow(one.x - two.x, 2) + Mathf.Pow(one.y - two.y, 2) + Mathf.Pow(one.z - two.z, 2));
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
            float distBetweenParts = distBetweenVectors(nextPart, positionOfLastLaserPart);

            laserTransform[i].position = Vector3.Lerp(positionOfLastLaserPart, nextPart, .5f);
            laserTransform[i].LookAt(nextPart);
            laserTransform[i].localScale = new Vector3(laserTransform[i].localScale.x, laserTransform[i].localScale.y, distBetweenParts);
            positionOfLastLaserPart = nextPart;

            valueToSearchBezierBy += (1f / numOfLasers);
        }
    }

    void checkSurroundingObjects()
    {
        if (layersOfObjectsToBendTo.Length == 0)
        {
            return;
        }

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
            for (int j = 0; j < layersOfObjectsToBendTo.Length; j++)
            {
                if (allObjects[i].layer == layersOfObjectsToBendTo[j])
                {
                    // Object can only have one layer so can do calculation for object here
                    Vector3 objectPosition = allObjects[i].transform.position;

                    // Finding closest to ray by creating a perpendicular plane using the formula that uses the point and then finds the distance between
                    // that point and where a plane created from the vector intersects the laser
                    float tValueFromFormulaExplained = (forwardVectorFromRemote.x * objectPosition.x + forwardVectorFromRemote.x * objectPosition.y
                        + forwardVectorFromRemote.z * objectPosition.z - forwardVectorFromRemote.x * positionOfRemote.x - forwardVectorFromRemote.y * positionOfRemote.y
                        - forwardVectorFromRemote.z * positionOfRemote.z) / (Mathf.Pow(forwardVectorFromRemote.x, 2) + Mathf.Pow(forwardVectorFromRemote.y, 2) + Mathf.Pow(forwardVectorFromRemote.z, 2));

                    Vector3 newPoint = new Vector3(forwardVectorFromRemote.x * tValueFromFormulaExplained + positionOfRemote.x, forwardVectorFromRemote.y * tValueFromFormulaExplained + positionOfRemote.y
                        , forwardVectorFromRemote.z * tValueFromFormulaExplained + positionOfRemote.z);

                    float distanceBetweenRayAndPoint = Mathf.Sqrt(Mathf.Pow(newPoint.x - objectPosition.x, 2) + Mathf.Pow(newPoint.y - objectPosition.y, 2) + Mathf.Pow(newPoint.z - objectPosition.z, 2));
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
            if(currentlyPointingAt != null && currentlyPointingAt != objectWithShortestDistance)
            {
                currentlyPointingAt.GetComponent<Renderer>().material = unhighlightedObject;
                unhighlightedObject = objectWithShortestDistance.GetComponent<Renderer>().material;
            }
            
            currentlyPointingAt = objectWithShortestDistance;
            castingBezierFrom = trackedObj.transform.position;

            currentlyPointingAt.GetComponent<Renderer>().material = MaterialToHighlightObjects;
        }
    }
}
