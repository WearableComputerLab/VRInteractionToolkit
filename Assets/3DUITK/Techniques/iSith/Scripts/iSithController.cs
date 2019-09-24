using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

[ExecuteInEditMode]
public class iSithController : MonoBehaviour {

    public GameObject laserPrefab;
    public iSithLaser laserL = null;
    public iSithLaser laserR = null;
    public GameObject interactionObject;

    private float lastLocationLDistance = -1;
    private float lastLocationRDistance = -1;

    public enum SelectionController {
        LeftController,
        RightController
    }

    public SelectionController selectionController = SelectionController.RightController;
    void Awake() {
        GameObject leftController = null, rightController = null;
#if SteamVR_Legacy
        if (laserR.controller == null || laserL.controller == null) {
            // returns if controllers already set up
            SteamVR_ControllerManager CameraRigObject = FindObjectOfType<SteamVR_ControllerManager>();
            // lasers not set up yet so will try to run auto attach
            // Locates the camera rig and its child controllers
            leftController = CameraRigObject.left;
            rightController = CameraRigObject.right;

            laserL.controller = leftController;
            laserR.controller = rightController;
        }
#elif SteamVR_2
        if (laserR.controller == null || laserL.controller == null) {
            // returns if controllers already set up
            SteamVR_Behaviour_Pose[] controllers = FindObjectsOfType<SteamVR_Behaviour_Pose>();
            if (controllers.Length > 1) {
                leftController = controllers[0].inputSource.ToString() == "LeftHand" ? controllers[0].gameObject : controllers[1].inputSource.ToString() == "LeftHand" ? controllers[1].gameObject : null;
                rightController = controllers[0].inputSource.ToString() == "RightHand" ? controllers[0].gameObject : controllers[1].inputSource.ToString() == "RightHand" ? controllers[1].gameObject : null;
            } else if (controllers.Length == 1) {
                leftController = controllers[0].inputSource.ToString() == "LeftHand" ? controllers[0].gameObject : null;
                rightController = controllers[0].inputSource.ToString() == "RightHand" ? controllers[0].gameObject : null;
            } else {
                return;
            }
            laserL.controller = leftController;
            laserR.controller = rightController;
        }
#endif
    }

    void setCubeLocation() {
        // assuming 1 is pointing controller for test
        Vector3 d1 = laserL.controller.transform.forward;
        Vector3 d2 = laserR.controller.transform.forward;

        Vector3 p1 = laserL.controller.transform.position;
        Vector3 p2 = laserR.controller.transform.position;

        // as these two vectors will probably create skew lines (on different planes) have to calculate the points on the lines that are
        // closest to eachother and then getting the midpoint between them giving a fake 'intersection'
        // This is achieved by utilizing parts of the fromula to find the shortest distance between two skew lines
        Vector3 n1 = Vector3.Cross(d1, (Vector3.Cross(d2, d1)));
        Vector3 n2 = Vector3.Cross(d2, (Vector3.Cross(d1, d2)));

        // Figuring out point 1
        Vector3 localPoint1 = p1 + ((Vector3.Dot((p2 - p1), n2)) / (Vector3.Dot(d1, n2))) * d1;

        // Figuring out point 2
        Vector3 localPoint2 = p2 + ((Vector3.Dot((p1 - p2), n1)) / (Vector3.Dot(d2, n1))) * d2;

        Vector3 location = (localPoint1 + localPoint2) / 2f;



        float distance_formula_on_vector = Mathf.Sqrt(d1.x * d1.x + d1.y * d1.y + d1.z * d1.z);

        float num = (distance_formula_on_vector * (localPoint1.x - p1.x)) / d1.x;

        if (num > 0) {
            interactionObject.transform.position = location;
            lastLocationLDistance = Vector3.Distance(p1, localPoint1);
            lastLocationRDistance = Vector3.Distance(p2, localPoint2);
        } else {
            // Because the skew calculation includes behind the controllers must account for when the smalles point is behind
            // if last location distance != -1 will calculate the last location of the cross over point so object doesnt jump around wildly
            // otherwise will just place between controllers
            if (lastLocationLDistance != -1) {
                Vector3 normalizedLDirectionPlusDistance = d1.normalized * lastLocationLDistance;
                Vector3 normalizedRDirectionPlusDistance = d2.normalized * lastLocationRDistance;
                Vector3 lPos = p1 + normalizedLDirectionPlusDistance;
                Vector3 rPos = p2 + normalizedRDirectionPlusDistance;
                Vector3 finalLocal = (lPos + rPos) / 2f;
                interactionObject.transform.position = finalLocal;
            } else {
                interactionObject.transform.position = (p1 + p2) / 2f;
            }


        }

    }


    public float AngleDir(Vector3 fwd, Vector3 targetDir, Vector3 up) // chang eto own code
    {
        Vector3 perp = Vector3.Cross(fwd, targetDir);
        float dir = Vector3.Dot(perp, up);

        if (dir > 0.0f) {
            return 1.0f;
        } else if (dir < 0.0f) {
            return -1.0f;
        } else {
            return 0.0f;
        }
    }

    // Update is called once per frame
    void Update() {
        if (Application.isPlaying) {
            setCubeLocation();
        }

#if SteamVR_Legacy
        // Resets the controller to select with if it is changed
        SteamVR_ControllerManager CameraRigObject = FindObjectOfType<SteamVR_ControllerManager>();
        SteamVR_TrackedObject leftController = CameraRigObject.left.GetComponent<SteamVR_TrackedObject>();
        SteamVR_TrackedObject rightController = CameraRigObject.right.GetComponent<SteamVR_TrackedObject>();
        iSithGrabObject component = GetComponentInChildren<iSithGrabObject>();
        if (selectionController == SelectionController.LeftController && component.trackedObj != leftController) {
            component.trackedObj = leftController;
        } else if (selectionController == SelectionController.RightController && component.trackedObj != rightController) {
            component.trackedObj = rightController;
        }
#elif SteamVR_2
        GameObject GleftController = null, GrightController = null;
        SteamVR_Behaviour_Pose[] controllers = FindObjectsOfType<SteamVR_Behaviour_Pose>();
        if (controllers.Length > 1) {
            GleftController = controllers[0].inputSource.ToString() == "LeftHand" ? controllers[0].gameObject : controllers[1].inputSource.ToString() == "LeftHand" ? controllers[1].gameObject : null;
            GrightController = controllers[0].inputSource.ToString() == "RightHand" ? controllers[0].gameObject : controllers[1].inputSource.ToString() == "RightHand" ? controllers[1].gameObject : null;
        } else if (controllers.Length == 1) {
            GleftController = controllers[0].inputSource.ToString() == "LeftHand" ? controllers[0].gameObject : null;
            GrightController = controllers[0].inputSource.ToString() == "RightHand" ? controllers[0].gameObject : null;
        } else {
            return;
        }
        SteamVR_Behaviour_Pose rightController = GrightController.GetComponent<SteamVR_Behaviour_Pose>();
        SteamVR_Behaviour_Pose leftController = GleftController.GetComponent<SteamVR_Behaviour_Pose>();

        iSithGrabObject component = GetComponentInChildren<iSithGrabObject>();
        if (selectionController == SelectionController.LeftController && component.trackedObj != leftController) {
            component.trackedObj = leftController;
        } else if (selectionController == SelectionController.RightController && component.trackedObj != rightController) {
            component.trackedObj = rightController;
        }
#endif
    }
}
