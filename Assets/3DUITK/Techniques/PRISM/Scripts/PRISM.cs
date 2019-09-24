using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class PRISM : MonoBehaviour {
#if SteamVR_Legacy
    private SteamVR_TrackedObject trackedObj;
    private SteamVR_Controller.Device Controller
    {
        get { return SteamVR_Controller.Input((int)trackedObj.index); }
    }
#elif SteamVR_2
    private SteamVR_Behaviour_Pose trackedObj;
    public SteamVR_Action_Boolean m_touchpadPress;
#else
    public GameObject trackedObj;
#endif


    public GameObject theController;
    public GameObject laserPrefab;
    private GameObject laser;
    private Transform laserTransform;
    private Vector3 hitPoint;

    private Vector3 lastDirectionPointing;
    private Quaternion lastRotation;
    private Vector3 lastPosition;
    public GameObject theModel;
    private bool ARMOn = false;

    // Quick solution to highlight on select - maybe find a better way?
    public Material MaterialToHighlightObjects;
    private Material unhighlightedObject;
    private GameObject currentlyPointingAt;

    public float minV = 0.1f;
    public float scaledMotionVeclocity = 0.2f;
    public float maxV = 0.4f;
    public float scaledMotionMultiplier = 0.5f;

    // Using the hack from gogo shadow - will have to fix them all once find a better way
    void makeModelChild()
    {
        if (theModel.transform.childCount > 0)
        {
            theModel.transform.parent = this.transform;
        }

    }

    private void ShowLaser(RaycastHit hit)
    {
        laser.SetActive(true);
        laserTransform.position = Vector3.Lerp(this.transform.position, hitPoint, .5f);
        laserTransform.LookAt(hitPoint);
        laserTransform.localScale = new Vector3(laserTransform.localScale.x, laserTransform.localScale.y,
            hit.distance);

        
        // highlighting the object
        if(hit.transform.gameObject.layer == 8)
        {
            if (currentlyPointingAt == null)
            {
                // no object previouslly was highlighted so just highlight this one
                currentlyPointingAt = hit.transform.gameObject;
                unhighlightedObject = currentlyPointingAt.GetComponent<Renderer>().material;
                currentlyPointingAt.GetComponent<Renderer>().material = MaterialToHighlightObjects;
            }
            else if (hit.transform.gameObject != currentlyPointingAt)
            {
                // unhighlight previous one and highlight this one
                currentlyPointingAt.GetComponent<Renderer>().material = unhighlightedObject;
                currentlyPointingAt = hit.transform.gameObject;
                currentlyPointingAt.GetComponent<Renderer>().material = MaterialToHighlightObjects;
            }
        } else
        {
            currentlyPointingAt.GetComponent<Renderer>().material = unhighlightedObject;
        }
    }

    private void ShowLaser()
    {
        // removing highlight from previously highlighted object
        if (currentlyPointingAt != null)
        {
            // remove highlight from previously highlighted object 
            currentlyPointingAt.GetComponent<Renderer>().material = unhighlightedObject;
            currentlyPointingAt = null;
        }

        // This is to make it extend infinite. There is DEFINATELY an easier way to do this. Find it later!
        Vector3 theVector = this.transform.forward;
        hitPoint = this.transform.position;
        float distance_formula_on_vector = Mathf.Sqrt(theVector.x * theVector.x + theVector.y * theVector.y + theVector.z * theVector.z);
        // Using formula to find a point which lies at distance on a 3D line from vector and direction
        hitPoint.x = hitPoint.x + (100 / (distance_formula_on_vector)) * theVector.x;
        hitPoint.y = hitPoint.y + (100 / (distance_formula_on_vector)) * theVector.y;
        hitPoint.z = hitPoint.z + (100 / (distance_formula_on_vector)) * theVector.z;

        laser.SetActive(true);
        laserTransform.position = Vector3.Lerp(this.transform.position, hitPoint, .5f);
        laserTransform.LookAt(hitPoint);
        laserTransform.localScale = new Vector3(laserTransform.localScale.x, laserTransform.localScale.y,
           100);
    }

    void Awake()
    {
#if SteamVR_Legacy
        trackedObj = theController.GetComponent<SteamVR_TrackedObject>();
#elif SteamVR_2
        trackedObj = theController.GetComponent<SteamVR_Behaviour_Pose>();
#endif

    }

    // Use this for initialization
    void Start () {

        laser = Instantiate(laserPrefab);
        laserTransform = laser.transform;

        lastDirectionPointing = trackedObj.transform.forward;
        lastRotation = trackedObj.transform.rotation;
        lastPosition = trackedObj.transform.position;
    }

    void toggleARM()
    {
        if(!ARMOn)
        {
            lastDirectionPointing = trackedObj.transform.forward;
            lastRotation = trackedObj.transform.rotation;
            lastPosition = trackedObj.transform.position;
        }
        ARMOn = !ARMOn;
    }
	
    void updatePositionAndRotationToFollowController()
    {
        this.transform.position = trackedObj.transform.position;
        Quaternion rotationOfDevice = trackedObj.transform.rotation;
        if (ARMOn)
        {

            // scaled down by factor of 10
            this.transform.rotation = Quaternion.Lerp(lastRotation, trackedObj.transform.rotation, 0.5f);
            this.transform.position = Vector3.Lerp(lastPosition, trackedObj.transform.position, 0.5f);
            print("On");
        } else
        {
            this.transform.rotation = trackedObj.transform.rotation;
            this.transform.position = trackedObj.transform.position;
        }
    }

    public enum ControllerState {
        TOUCHPADDOWN, NONE
    }

    private ControllerState controllerEvents() {
#if SteamVR_Legacy
        if (Controller.GetPressDown(SteamVR_Controller.ButtonMask.Touchpad)){
            return ControllerState.TOUCHPADDOWN;
        }
#elif SteamVR_2
        if (m_touchpadPress.GetStateDown(trackedObj.inputSource)) {
            return ControllerState.TOUCHPADDOWN;
        }
#endif
        return ControllerState.NONE;
    }

    // Update is called once per frame
    void Update () {
        makeModelChild();
        updatePositionAndRotationToFollowController();
        
        RaycastHit hit;
        if (Physics.Raycast(this.transform.position, this.transform.forward, out hit, 100))
        {
            hitPoint = hit.point;
            ShowLaser(hit);
        }
        else
        {
            ShowLaser();
        }

        if (controllerEvents() == ControllerState.TOUCHPADDOWN)
        {
            toggleARM();
        }
        
    }
}
