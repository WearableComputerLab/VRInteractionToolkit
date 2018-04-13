using UnityEngine;

public class iSith : MonoBehaviour {

    /* iSith implementation by Kieran May
     * University of South Australia
     * 
     * */

    private SteamVR_TrackedObject trackedObj;
    private SteamVR_Controller.Device controller;

    public GameObject laserPrefab;
    private GameObject laser;
    private Transform laserTransform;
    private Vector3 hitPoint;
    public GameObject mirroredCube;
    public GameObject pointOfInteraction;

    private void ShowLaser(RaycastHit hit) {
        mirroredCube.SetActive(false);
        laser.SetActive(true);
        laserTransform.position = Vector3.Lerp(trackedObj.transform.position, hitPoint, .5f);
        laserTransform.LookAt(hitPoint);
        laserTransform.localScale = new Vector3(laserTransform.localScale.x, laserTransform.localScale.y, hit.distance);
    }
    public static Vector3 leftController = new Vector3(0, 0, 0);
    public static Vector3 rightController = new Vector3(0, 0, 0);

    public static Vector3 leftLaser = new Vector3(0, 0, 0);
    public static Vector3 rightLaser = new Vector3(0, 0, 0);

    private void interactionPosition() {
        //print(trackedObj.name);
        if (trackedObj.name == "Controller (left)" && trackedObj.transform.position != null) {
            leftController = trackedObj.transform.position;
            leftLaser = laser.transform.position;
            //print("leftController:" + leftController);
            //print("rightController:" + rightController);
        } else if (trackedObj.name == "Controller (right)" && trackedObj.transform.position != null) {
            rightController = trackedObj.transform.position;
            rightLaser = laser.transform.position;
            //print("rightController:" + rightController);
        }
        print(leftController);
        print(rightController);

        //Vector3 crossed = Vector3.Cross(leftController, rightController);
        Vector3 crossed = Vector3.Lerp(leftLaser, rightLaser, 0.5f);
        print("crossedval:" + crossed);
        pointOfInteraction.transform.localPosition = crossed;
        //float controllerDist = Vector3.Distance(leftController, rightController);
        //print("dist:"+ controllerDist);
        //pointOfInteraction.transform.position = leftController;
    }

    private void ShowLaser() {
        laser.SetActive(true);
        mirroredCube.SetActive(true);
    }

    /*private bool pickedUpObject = false; //ensure only 1 object is picked up at a time
    private GameObject tempObjectStored;
    void PickupObject(GameObject obj) {
        Vector3 controllerPos = trackedObj.transform.forward;
        if (trackedObj != null) {
            if (controller.GetTouchDown(SteamVR_Controller.ButtonMask.Trigger) && pickedUpObject == false) {
                //obj.GetComponent<Collider>().attachedRigidbody.isKinematic = true;
                obj.transform.SetParent(trackedObj.transform);
                extendDistance = Vector3.Distance(controllerPos, obj.transform.position);
                tempObjectStored = obj; // Storing the object as an instance variable instead of using the obj parameter fixes glitch of it not properly resetting on TriggerUp
                pickedUpObject = true;
            }
            if (controller.GetTouchUp(SteamVR_Controller.ButtonMask.Trigger) && pickedUpObject == true) {
                //obj.GetComponent<Collider>().attachedRigidbody.isKinematic = false;
                tempObjectStored.transform.SetParent(null);
                pickedUpObject = false;
            }
        }
    }*/

    private float extendDistance = 0f;
    private float cursorSpeed = 20f; // Decrease to make faster, Increase to make slower

    void mirroredObject() {
        Vector3 controllerPos = trackedObj.transform.forward;
        float distance_formula_on_vector = Mathf.Sqrt(controllerPos.x * controllerPos.x + controllerPos.y * controllerPos.y + controllerPos.z * controllerPos.z);
        Vector3 mirroredPos = trackedObj.transform.position;

        mirroredPos.x = mirroredPos.x + (100f / (distance_formula_on_vector)) * controllerPos.x;
        mirroredPos.y = mirroredPos.y + (100f / (distance_formula_on_vector)) * controllerPos.y;
        mirroredPos.z = mirroredPos.z + (100f / (distance_formula_on_vector)) * controllerPos.z;

        mirroredCube.transform.position = mirroredPos;
        mirroredCube.transform.rotation = trackedObj.transform.rotation;
    }

    void Awake() {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
    }

    void Start() {
        laser = Instantiate(laserPrefab);
        if (trackedObj.name == "Controller (left)") {
            laser.name = "laserLeft";
        } else if (trackedObj.name == "Controller (right)") {
            laser.name = "laserRight";
        }
        laserTransform = laser.transform;
    }

    void Update() {
        controller = SteamVR_Controller.Input((int)trackedObj.index);
        interactionPosition();
        mirroredObject();
        ShowLaser();
        Ray ray = Camera.main.ScreenPointToRay(trackedObj.transform.position);
        RaycastHit hit;
        if (Physics.Raycast(trackedObj.transform.position, transform.forward, out hit, 100)) {
           //print("hit:" + hit.transform.name);
            hitPoint = hit.point;
            /*PickupObject(hit.transform.gameObject);
            if (pickedUpObject == true) {
                PadScrolling(hit.transform.gameObject);
            }*/
            ShowLaser(hit);
        }
        /*} else {
            laser.SetActive(false);
        }*/
    }

}
