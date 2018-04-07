using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class FlexiblePointer : MonoBehaviour
{

    private SteamVR_TrackedObject trackedObj;

    public float pointerLength = 1f;
    private float curve = 0f;

    private float[] point0; // Hand location
    private float[] point1; // The curve
    private float[] point2; // The distance

    // Laser vars
    private int numOfLasers = 20;
    public GameObject laserPrefab;
    private GameObject[] lasers;
    private Transform[] laserTransform;
    private Vector3 hitPoint;

    private SteamVR_Controller.Device Controller
    {
        get { return SteamVR_Controller.Input((int)trackedObj.index); }
    }

    void Awake()
    {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
    }

    // Use this for initialization
    void Start()
    {
        // Setting up points
        point0 = new float[3];
        point1 = new float[3];
        point2 = new float[3];

        // Initalizing all the lasers
        lasers = new GameObject[numOfLasers];
        laserTransform = new Transform[numOfLasers];
        for (int i=0; i < numOfLasers; i++)
        {
            GameObject laserPart = Instantiate(laserPrefab, new Vector3((float)i, 1, 0), Quaternion.identity) as GameObject;
            laserTransform[i] = laserPart.transform;
            lasers[i] = laserPart;
        }
        
        setPoint0and1();
    }

    

    void setPoint0and1()
    {
        // setting test points
        Vector3 controllerPos = trackedObj.transform.position;
        Vector3 forwardVector = trackedObj.transform.forward;

        float distance_formula_on_vector = Mathf.Sqrt(forwardVector.x * forwardVector.x + forwardVector.y * forwardVector.y + forwardVector.z * forwardVector.z);

        // Hand position and first points
        point0[0] = controllerPos.x;
        point0[1] = controllerPos.y;
        point0[2] = controllerPos.z;

        // Point extended along line
        point2[0] = controllerPos.x + (pointerLength / (distance_formula_on_vector)) * forwardVector.x;
        point2[1] = controllerPos.y + (pointerLength / (distance_formula_on_vector)) * forwardVector.y;
        point2[2] = controllerPos.z + (pointerLength / (distance_formula_on_vector)) * forwardVector.z;

        setCurve();
    }

    // Initalizes point1 's curve
    void setCurve()
    {
        // Midpoint of the points 0 and 1
        float midX = (point0[0] + point1[0]) / 2;
        float midY = (point0[1] + point1[1]) / 2;
        float midZ = (point0[2] + point1[2]) / 2;

        midX = midX + curve; // curving out a bit just a test
        midY = midY + curve;
        midZ = midZ + curve;

        point1[0] = midX;
        point1[1] = midY;
        point1[2] = midZ;
    }

    // Update is called once per frame
    void Update()
    {
        setPoint0and1();
        castBezierRay();
    }

    void castBezierRay()
    {
        float valueToSearchBezierBy = 0f;

        Vector3 positionOfLastLaserPart = trackedObj.transform.position;
        for (int i = 0; i < numOfLasers; i++) 
        {
            lasers[i].SetActive(true);
            float[] pointOnBezier = getBezierPoint(valueToSearchBezierBy);
            Vector3 nextPart = new Vector3(pointOnBezier[0], pointOnBezier[1], pointOnBezier[2]);
            float distBetweenParts = Mathf.Sqrt(Mathf.Pow(nextPart.x - positionOfLastLaserPart.x, 2) + Mathf.Pow(nextPart.y - positionOfLastLaserPart.y, 2) + Mathf.Pow(nextPart.z - positionOfLastLaserPart.z, 2));

            laserTransform[i].position = Vector3.Lerp(positionOfLastLaserPart, nextPart, .5f);
            laserTransform[i].LookAt(nextPart);
            laserTransform[i].localScale = new Vector3(laserTransform[i].localScale.x, laserTransform[i].localScale.y,
        distBetweenParts);

            positionOfLastLaserPart = nextPart;
            valueToSearchBezierBy += (1f / numOfLasers);
        }

        /*
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(trackedObj.transform.position);
        if (Physics.Raycast(trackedObj.transform.position, transform.forward, out hit, 100))
        {
            hitPoint = hit.point;
            ShowLaser(hit);
        }     
        */
    }

    // t being betweek 0 and 1 to get a spot on the curve
    float[] getBezierPoint(float t)
    {
        float[] thePoint = new float[3];
        // Formula used to get position on point
        thePoint[0] = (1 - t) * (1 - t) * point0[0] + 2 * (1 - t) * t * point1[0] + t * t * point2[0];  // x
        thePoint[1] = (1 - t) * (1 - t) * point0[1] + 2 * (1 - t) * t * point1[1] + t * t * point2[1];  // y
        thePoint[2] = (1 - t) * (1 - t) * point0[2] + 2 * (1 - t) * t * point1[2] + t * t * point2[2];  // z
        return thePoint;
    }
}
