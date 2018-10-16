using UnityEngine;

public class ImagePlane_FramingHands : MonoBehaviour {

    /* ImagePlane_FramingHands implementation by Kieran May
     * University of South Australia
     * 
     * TODO
     * - Making the laser scale/increase based on the dist apart of both controllers should fix alot of general issues
     * */
    internal bool objSelected = false;

    public GameObject controllerRight;
    public GameObject controllerLeft;
    public GameObject cameraHead; // Camera etye
    public GameObject cameraRig;

    private SteamVR_TrackedObject trackedObjL;
    private SteamVR_TrackedObject trackedObjR;
    private SteamVR_Controller.Device controllerL;
    private SteamVR_Controller.Device controllerR;
    public GameObject laserPrefab;
    private GameObject laser;
    private Transform laserTransform;
    private Vector3 hitPoint;
    private GameObject mirroredCube;
    public GameObject pointOfInteraction;
    private GameObject selectedObject;
    private Transform oldParent;


    


    private void ShowLaser(RaycastHit hit) {
        float controllerDist = Vector3.Distance(trackedObjL.transform.position, trackedObjR.transform.position);
        mirroredCube.SetActive(false);
        laser.SetActive(true);
        laserTransform.position = Vector3.Lerp(pointOfInteraction.transform.position, hitPoint, .5f);
        laserTransform.LookAt(hitPoint);
        laserTransform.localScale = new Vector3(controllerDist, controllerDist, hit.distance);
        InstantiateObject(hit.transform.gameObject);
    }

    public static Vector3 leftController = new Vector3(0, 0, 0);
    public static Vector3 rightController = new Vector3(0, 0, 0);

    public static Vector3 leftLaser = new Vector3(0, 0, 0);
    public static Vector3 rightLaser = new Vector3(0, 0, 0);

    void createOffSet(GameObject obj) {
        Vector3 controllerPos = pointOfInteraction.transform.forward;
        Vector3 pos = pointOfInteraction.transform.position;
        float distance_formula_on_vector = Mathf.Sqrt(controllerPos.x * controllerPos.x + controllerPos.y * controllerPos.y + controllerPos.z * controllerPos.z);
        pos.x += (extendDistance / (distance_formula_on_vector)) * controllerPos.x;
        pos.y += (extendDistance / (distance_formula_on_vector)) * controllerPos.y;
        pos.z += (extendDistance / (distance_formula_on_vector)) * controllerPos.z;
        obj.transform.position = pos;
    }

    private void interactionPosition() {
        Vector3 crossed = Vector3.Lerp(trackedObjL.transform.position, trackedObjR.transform.position, 0.5f);
        pointOfInteraction.transform.localPosition = crossed;
        pointOfInteraction.transform.localRotation = Quaternion.RotateTowards(trackedObjR.transform.rotation, trackedObjL.transform.rotation, 0);
        pointOfInteraction.transform.localRotation *= Quaternion.Euler(75, 0, 0);
    }

    internal void resetProperties() {
        objSelected = false;
        selectedObject.transform.SetParent(oldParent);
        cameraHead.transform.localScale = new Vector3(1f, 1f, 1f);
        cameraRig.transform.localScale = new Vector3(1f, 1f, 1f);
        cameraRig.transform.localPosition = new Vector3(0f, 0f, 0f);
    }

    //Tham's scale method
    public void ScaleAround(Transform target, Transform pivot, Vector3 scale) {
        Transform pivotParent = pivot.parent;
        Vector3 pivotPos = pivot.position;
        pivot.parent = target;
        target.localScale = scale;
        target.position += pivotPos - pivot.position;
        pivot.parent = pivotParent;
    }
    float Disteh;
    float Disteo;
    float scaleAmount;
    //Scale camera down attempt
    Vector3 oldHeadScale;
    Vector3 oldCameraRigScale;
    private void InstantiateObject(GameObject obj) {
        if (controllerL.GetPressDown(SteamVR_Controller.ButtonMask.Trigger)) {
            if (objSelected == false && obj.transform.name != "Mirrored Cube") {
                selectedObject = obj;
                oldParent = selectedObject.transform.parent;
                float dist = Vector3.Distance(pointOfInteraction.transform.position, selectedObject.transform.position);
                selectedObject.transform.SetParent(pointOfInteraction.transform);
                selectedObject.transform.localPosition = new Vector3(0f, 0f, 0f);

                Vector3 controllerPos = pointOfInteraction.transform.forward;
                Vector3 pos = pointOfInteraction.transform.position;
                float distance_formula_on_vector = Mathf.Sqrt(controllerPos.x * controllerPos.x + controllerPos.y * controllerPos.y + controllerPos.z * controllerPos.z);
                float distextended = 0.25f;
                pos.x += (distextended / (distance_formula_on_vector)) * controllerPos.x;
                pos.y += (distextended / (distance_formula_on_vector)) * controllerPos.y;
                pos.z += (distextended / (distance_formula_on_vector)) * controllerPos.z;
                obj.transform.position = pos;

                selectedObject.transform.localScale = new Vector3(selectedObject.transform.localScale.x / dist, selectedObject.transform.localScale.y / dist, selectedObject.transform.localScale.z / dist);
                print("Scaled to:"+selectedObject.transform.localScale.x);
                //float dist = Vector3.Distance(pointOfInteraction.transform.position, selectedObject.transform.position);
                //print(dist);

                objSelected = true;
                laser.SetActive(false);

                Disteh = Vector3.Distance(cameraHead.transform.position, pointOfInteraction.transform.position);
                Disteo = Vector3.Distance(cameraHead.transform.position, obj.transform.position);
                print("cameraHead:" + cameraHead.transform.position);
                print("hand:" + pointOfInteraction.transform.position);
                print("object:" + obj.transform.localPosition);

                scaleAmount = Disteo / Disteh;
                print("scale amount:" + scaleAmount);
                oldHeadScale = cameraHead.transform.localScale;
                oldCameraRigScale = cameraRig.transform.localScale;
                ScaleAround(cameraRig.transform, cameraHead.transform, new Vector3(scaleAmount, scaleAmount, scaleAmount));
                //selectedObject.transform.localScale = new Vector3(scaleAmount, scaleAmount, scaleAmount);
                Vector3 eyeProportion = cameraHead.transform.localScale / scaleAmount;
                //Keep eye distance proportionate to original position
                cameraHead.transform.localScale = eyeProportion;
            } else if (objSelected == true) {
                resetProperties();
            }
        }
    }

    private void WorldGrab() {
        if (controllerL.GetPressDown(SteamVR_Controller.ButtonMask.ApplicationMenu)) { // temp
            //Resetting everything back to normal
            objSelected = false;
            selectedObject.transform.SetParent(oldParent);
            cameraHead.transform.localScale = new Vector3(1f, 1f, 1f);
            cameraRig.transform.localScale = new Vector3(1f, 1f, 1f);
            cameraRig.transform.localPosition = new Vector3(0f, 0f, 0f);
        }
    }

    private void ShowLaser() {
        laser.SetActive(true);
        leftLaser = laser.transform.position;
        mirroredCube.SetActive(true);
    }


    private float extendDistance = 0f;
    private float cursorSpeed = 20f; // Decrease to make faster, Increase to make slower

    void mirroredObject() {
        Vector3 controllerPos = cameraHead.transform.forward;
        float distance_formula_on_vector = Mathf.Sqrt(controllerPos.x * controllerPos.x + controllerPos.y * controllerPos.y + controllerPos.z * controllerPos.z);
        Vector3 mirroredPos = pointOfInteraction.transform.position;

        mirroredPos.x = mirroredPos.x + (100f / (distance_formula_on_vector)) * controllerPos.x;
        mirroredPos.y = mirroredPos.y + (100f / (distance_formula_on_vector)) * controllerPos.y;
        mirroredPos.z = mirroredPos.z + (100f / (distance_formula_on_vector)) * controllerPos.z;

        mirroredCube.transform.position = mirroredPos;
        mirroredCube.transform.rotation = pointOfInteraction.transform.rotation;
    }

    void Awake() {

        trackedObjL = controllerRight.GetComponent<SteamVR_TrackedObject>();
        trackedObjR = controllerLeft.GetComponent<SteamVR_TrackedObject>();
        mirroredCube = this.transform.Find("Mirrored Cube").gameObject;
    }

    void Start() {
        laser = Instantiate(laserPrefab);
        laserTransform = laser.transform;
    }

    void castRay() {
        interactionPosition();
        mirroredObject();
        ShowLaser();
        Ray ray = Camera.main.ScreenPointToRay(pointOfInteraction.transform.position);
        RaycastHit hit;
        if (Physics.Raycast(pointOfInteraction.transform.position, cameraHead.transform.forward, out hit, 100)) {
            hitPoint = hit.point;
            ShowLaser(hit);
        }
    }

    void Update() {
        controllerL = SteamVR_Controller.Input((int)trackedObjL.index);
        controllerR = SteamVR_Controller.Input((int)trackedObjR.index);
        //if (objSelected == false) {
            castRay();
         if (objSelected == true) {
            WorldGrab(); //Using the ScaledWorldGrab to scale down the world
        }
    }

}