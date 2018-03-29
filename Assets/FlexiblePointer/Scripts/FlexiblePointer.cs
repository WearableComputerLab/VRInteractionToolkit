using UnityEngine;

public class FlexiblePointer : MonoBehaviour {

    /* Flexible Pointer implementation by Kieran May
     * University of South Australia
     * 
     * -Very early stages, currently only shoots laser
     * */

    private SteamVR_TrackedObject trackedObj;
    private SteamVR_Controller.Device controller;

    public GameObject laserPrefab;
    private GameObject laser;
    private Transform laserTransform;
    private Vector3 hitPoint;
    private Vector3 hitPoint2D;

    //Giving a weird get_FrameCount error in the console for some reason?
    /*int rightIndex = SteamVR_Controller.GetDeviceIndex(SteamVR_Controller.DeviceRelation.Rightmost);
    /int leftIndex = SteamVR_Controller.GetDeviceIndex(SteamVR_Controller.DeviceRelation.Leftmost);*/

    private void ShowLaser(RaycastHit hit) {
        laser.SetActive(true);
        laserTransform.position = Vector3.Lerp(trackedObj.transform.position, hitPoint, .5f);
        laserTransform.LookAt(hitPoint);
        laserTransform.localScale = new Vector3(laserTransform.localScale.x, laserTransform.localScale.y, hit.distance);
    }

    public float thickness = 0.002f;
    float dist = 100f;

    private void ShowLaser() {
        laser.SetActive(true);
        //laserTransform.position = Vector3.Lerp(trackedObj.transform.position, hitPoint, .5f);
        laserTransform.transform.localScale = new Vector3(thickness * 5f, thickness * 5f, dist);

        //laserTransform.LookAt(hitPoint);
        //laserTransform.localScale = new Vector3(laserTransform.localScale.x, laserTransform.localScale.y, laserTransform.localScale.z);
    }


    void Awake() {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
    }

    void Start() {
        laser = Instantiate(laserPrefab);
        laserTransform = laser.transform;
    }

    // Update is called once per frame
    void Update() {
        controller = SteamVR_Controller.Input((int)trackedObj.index);
        Ray ray = Camera.main.ScreenPointToRay(trackedObj.transform.position);
        if (controller.GetPress(SteamVR_Controller.ButtonMask.Touchpad)) {
            RaycastHit hit;
            if (Physics.Raycast(trackedObj.transform.position, transform.forward, out hit, 100)) {
                print("hit:" + hit.transform.name);
                hitPoint = hit.point;
                ShowLaser(hit);
            } else {
                hitPoint = ray.GetPoint(10);
                ShowLaser();
            }
        } else {
            laser.SetActive(false);
        }
    }
}
