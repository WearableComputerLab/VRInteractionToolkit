using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Valve.VR;

public class LaserPointer : MonoBehaviour {
#if SteamVR_Legacy
    private SteamVR_TrackedObject trackedObj;
    private SteamVR_Controller.Device controller;
#elif SteamVR_2
    private SteamVR_Behaviour_Pose trackedObj;
    public SteamVR_Action_Boolean m_touchpad;
#else
    public GameObject trackedObj;
#endif
    private GameObject[] circleObjects;
    public GameObject cursor;

    private float startRadius = 0f;
    public GameObject laserPrefab;
    private GameObject laser;
    private Transform laserTransform;
    private Vector3 hitPoint;
    private Vector3 hitPoint2D;

    private void ShowLaser(RaycastHit hit) {
        //transformCursor(hitPoint);
        laser.SetActive(true);
        laserTransform.position = Vector3.Lerp(trackedObj.transform.position, hitPoint, .5f);
        laserTransform.LookAt(hitPoint);
        laserTransform.localScale = new Vector3(laserTransform.localScale.x, laserTransform.localScale.y, hit.distance);
    }

    private void transformCursor(Vector3 position) {
        cursor.transform.position = new Vector3(position.x, position.y, position.z+-0.5f);
    }

    /*private void handleBubble() {
        float[][] lowestDistances = ClosestObject();
        float ClosestCircleRadius = lowestDistances[0][0] + circleObjects[(int)lowestDistances[0][1]].GetComponent<SphereCollider>().radius;
        float SecondClosestCircleRadius = lowestDistances[1][0] - circleObjects[(int)lowestDistances[1][1]].GetComponent<SphereCollider>().radius;
        float closestValue = Mathf.Min(ClosestCircleRadius, SecondClosestCircleRadius);
        print("FIRST:" + ClosestCircleRadius * 2);
        print("SECOND:" + SecondClosestCircleRadius * 2);
        if (ClosestCircleRadius * 2 < SecondClosestCircleRadius * 2) {
            cursor.GetComponent<SphereCollider>().radius = (closestValue + ClosestCircleRadius);
            print("TARGET:" + lowestDistances[0][1]);
        } else {
            cursor.GetComponent<SphereCollider>().radius = (closestValue + SecondClosestCircleRadius);
            print("TARGET:" + lowestDistances[1][1]);
        }
    }*/

    private void ShowLaser2D(RaycastHit2D hit) {
        laser.SetActive(true);
        laserTransform.position = Vector3.Lerp(trackedObj.transform.position, hitPoint, .5f);
        laserTransform.LookAt(hitPoint);
        laserTransform.localScale = new Vector3(laserTransform.localScale.x, laserTransform.localScale.y, hit.distance);
    }

    /*private float[][] ClosestObject() {
        float[] lowestDists = new float[4];
        lowestDists[0] = 0; // 1ST Lowest Distance
        lowestDists[1] = 0; // 2ND Lowest Distance
        lowestDists[2] = 0; // 1ST Lowest Index
        lowestDists[3] = 0; // 2ND Lowest Index
        float lowestDist = 0;
        float[][] allDists = new float[circleObjects.Length][];
        for (int i = 0; i < circleObjects.Length; i++) {
            allDists[i] = new float[2];
        }

        int lowestValue = 0;
        for (int i = 0; i < circleObjects.Length; i++) {
            //float getX = circleObjects[i].transform.position.x;
            //float getY = circleObjects[i].transform.position.y;
            //float dist = Mathf.Abs(getX - Input.mousePosition.x) + Mathf.Abs(getY - Input.mousePosition.y);
            float dist = Vector3.Distance(cursor.transform.position, circleObjects[i].transform.position) / 2f;
            if (i == 0) {
                lowestDist = dist;
                lowestValue = 0;
            } else {
                if (dist < lowestDist) {
                    lowestDist = dist;
                    lowestValue = i;
                }
            }
            allDists[i][0] = dist;
            allDists[i][1] = i;
        }
        float[][] arraytest = allDists.OrderBy(row => row[0]).ToArray();
        return arraytest;
    }*/


    void Awake() {
#if SteamVR_Legacy
        trackedObj = GetComponent<SteamVR_TrackedObject>();
#elif SteamVR_2
        trackedObj = GetComponent<SteamVR_Behaviour_Pose>();
#endif
    }

    void Start() {
        laser = Instantiate(laserPrefab);
        laserTransform = laser.transform;
    }

    public enum ControllerState {
        TOUCHPAD_PRESS, TOUCHPAD_DOWN, NONE
    }

    private ControllerState controllerEvents() {
#if SteamVR_Legacy
        if (controller.GetPress(SteamVR_Controller.ButtonMask.Touchpad)) {
            return ControllerState.TOUCHPAD_PRESS;
        }
        if (controller.GetPressDown(SteamVR_Controller.ButtonMask.Touchpad)) {
            return ControllerState.TOUCHPAD_DOWN;
        }
#elif SteamVR_2
        if (m_touchpad.GetState(trackedObj.inputSource)) {
            return ControllerState.TOUCHPAD_PRESS;
        }
        if (m_touchpad.GetStateDown(trackedObj.inputSource)) {
            return ControllerState.TOUCHPAD_DOWN;
        }
#endif
        return ControllerState.NONE;
    }

    // Update is called once per frame
    void Update() {
        //handleBubble();
#if SteamVR_Legacy
        controller = SteamVR_Controller.Input((int)trackedObj.index);
#endif
        if (controllerEvents() == ControllerState.TOUCHPAD_PRESS) {
            RaycastHit hit;
            if (Physics.Raycast(trackedObj.transform.position, transform.forward, out hit, 100)) {
                print("hit:" + hit.transform.name);
                if (controllerEvents() == ControllerState.TOUCHPAD_DOWN) {
                    hitPoint = hit.point;
                }
                transformCursor(hitPoint);
                ShowLaser(hit);
            }

            /*RaycastHit2D myhit = Physics2D.Raycast(trackedObj.transform.position, transform.up, 10);
            if (myhit.collider != null) {
                print("hit2d:" + myhit.collider.name);
                hitPoint2D = myhit.point;
                ShowLaser2D(myhit);
            }*/


        } else {
            laser.SetActive(false);
        }
    }
}
