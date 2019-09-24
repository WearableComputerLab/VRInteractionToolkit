using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using Valve.VR;

public class BubbleCursor2D : MonoBehaviour {

    /* 2D Bubble Cursor implementation by Kieran May
     * University of South Australia
     * 
     * A flawless 2D implementation of the Bubble Cursor I created as a prototype before implementing the 3D Bubble Cursor.
     * Much of the code shares similiarities with my 3D VR Bubble Cursor implementation.
     * */

    private GameObject[] circleObjects;
    private float startRadius = 0f;
    public GameObject closestPointer;
    public GameObject closestPointerb;
    public GameObject objectBubble;
    private bool objectInBubble = false;

    private GameObject objectInsideBubble;

    private void OnDrawGizmos() {
        Gizmos.color = Color.yellow;
        CircleCollider2D col = this.GetComponent<CircleCollider2D>();
        Gizmos.DrawWireSphere(this.transform.position, col.radius / 2);
    }

    // Use this for initialization
    void Start() {
        circleObjects = GameObject.FindGameObjectsWithTag("circleObject");
        startRadius = GetComponent<CircleCollider2D>().radius;
        this.transform.GetComponent<Renderer>().material.color = Color.blue;
    }

    private void OnTriggerStay2D(Collider2D collision) {
        if (objectInBubble == false) {
            //print("object inside bubble:" + collision.transform.name);
            objectInsideBubble = collision.gameObject;
            objectInBubble = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (objectInBubble == true) {
            //print("object left bubble:" + collision.transform.name);
            objectInBubble = false;
        }
    }

    private float[][] ClosestObject() {
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
            float dist = Vector3.Distance(Input.mousePosition, circleObjects[i].transform.position);
            dist -= circleObjects[i].GetComponent<CircleCollider2D>().radius * circleObjects[i].transform.localScale.x;
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
            //allDists[i][2] = circleObjects[i].GetComponent<CircleCollider2D>().radius;
        }
        float[][] arraytest = allDists.OrderBy(row => row[0]).ToArray();
        arraytest = allDists.OrderBy(row => row[0]).ToArray();
        printArray(arraytest);
        return arraytest;
    }

    void printArray(float[][] arraytest) {
        //print("On object: " + circleObjects[(int)arraytest[0][1]].name + " | at dist:" + arraytest[0][0]);
        for (int i = 0; i < arraytest.Length; i++) {
            print(arraytest[i][0] + " | name: " + circleObjects[(int)arraytest[i][1]].name);
        }
    }

    private float[][] resortArray(float[][] arraytest) {
        float[][] newarray = arraytest.OrderBy(row => row[0]).ToArray();
        return newarray;
    }

    readonly float bubbleOffset = 20f;
    float minRadius = 0f;

    // Update is called once per frame
    void Update() {
        float getX = Input.mousePosition.x; //replaced with input.mousePos.x, y, z etc
        float getY = Input.mousePosition.y;
        float getZ = Input.mousePosition.z;

        this.transform.position = new Vector3(getX, getY, getZ);
        this.transform.rotation = new Quaternion(0f, 0f, 0f, 0f);
        closestPointer.transform.position = new Vector3(getX, getY, getZ);
        closestPointerb.transform.position = new Vector3(getX, getY, getZ);
        float[][] lowestDistances = ClosestObject();
        //We need to re-evaluate the closestCircle here...
        //printArray(lowestDistances);
        printArray(lowestDistances);
        float local1 = circleObjects[(int)lowestDistances[0][1]].GetComponent<CircleCollider2D>().radius * circleObjects[(int)lowestDistances[0][1]].transform.localScale.x;
        float local2 = circleObjects[(int)lowestDistances[1][1]].GetComponent<CircleCollider2D>().radius * circleObjects[(int)lowestDistances[1][1]].transform.localScale.x;
        float avg = local1 - local2;
        //float avg = minRadius;
        print("avg radius:" + avg);
        float ClosestCircleRadius = lowestDistances[0][0] + (circleObjects[(int)lowestDistances[0][1]].GetComponent<CircleCollider2D>().radius * circleObjects[(int)lowestDistances[0][1]].transform.localScale.x) + (circleObjects[(int)lowestDistances[0][1]].GetComponent<CircleCollider2D>().radius * circleObjects[(int)lowestDistances[0][1]].transform.localScale.x);
        float SecondClosestCircleRadius = lowestDistances[1][0] - (circleObjects[(int)lowestDistances[1][1]].GetComponent<CircleCollider2D>().radius * circleObjects[(int)lowestDistances[1][1]].transform.localScale.x) + (circleObjects[(int)lowestDistances[1][1]].GetComponent<CircleCollider2D>().radius * circleObjects[(int)lowestDistances[1][1]].transform.localScale.x);
        print("minus radius:"+circleObjects[(int)lowestDistances[1][1]].GetComponent<CircleCollider2D>().radius);
        //print("avg radius:"+avg);
        float closestValue = Mathf.Min(ClosestCircleRadius, SecondClosestCircleRadius);
        //print("FIRST:" + ClosestCircleRadius*2);
        //print("SECOND:" + SecondClosestCircleRadius*2);
        print("FIRST closest radius:" + ClosestCircleRadius + " | closest value:" + closestValue + " | name: " + circleObjects[(int)lowestDistances[0][1]].name + " | dist: "+lowestDistances[0][0]);
        print("SECOND closest radius:" + SecondClosestCircleRadius + " | closest value:" + closestValue + " | name: " + circleObjects[(int)lowestDistances[1][1]].name + " | dist: " + lowestDistances[1][0]);
        if (ClosestCircleRadius  < SecondClosestCircleRadius) {
            this.GetComponent<CircleCollider2D>().radius = (closestValue + ClosestCircleRadius);
            //objectBubble.GetComponent<RectTransform>().sizeDelta = new Vector2(ClosestCircle.GetComponent<RectTransform>().sizeDelta.x + bubbleOffset, ClosestCircle.GetComponent<RectTransform>().sizeDelta.y + bubbleOffset);
            //objectBubble.transform.position = ClosestCircle.transform.position;

            //radiusBubble.transform.localScale = new Vector3(closestValue + ClosestCircleRadius, closestValue + ClosestCircleRadius, closestValue + ClosestCircleRadius);
            //radiusBubble.transform.localScale = new Vector2(closestValue + ClosestCircleRadius, closestValue + ClosestCircleRadius)/2f;
            //USED FOR SECONDARY OBJECT BUBBLE
            objectBubble.GetComponent<RectTransform>().sizeDelta = new Vector2(0f, 0f);
            print("TARGET:" + lowestDistances[0][1] + " | 0");
            //minRadius = circleObjects[(int)lowestDistances[0][1]].GetComponent<CircleCollider2D>().radius - circleObjects[(int)lowestDistances[1][1]].GetComponent<CircleCollider2D>().radius;
        } else {
            this.GetComponent<CircleCollider2D>().radius = (closestValue + SecondClosestCircleRadius);
            //objectBubble.GetComponent<RectTransform>().sizeDelta = new Vector2(SecondClosestCircle.GetComponent<RectTransform>().sizeDelta.x + bubbleOffset, SecondClosestCircle.GetComponent<RectTransform>().sizeDelta.y + bubbleOffset);
            //objectBubble.transform.position = SecondClosestCircle.transform.position;

            //radiusBubble.transform.localScale = new Vector3(closestValue + SecondClosestCircleRadius, closestValue + SecondClosestCircleRadius, closestValue + SecondClosestCircleRadius);
            //radiusBubble.transform.localScale = new Vector2(closestValue + SecondClosestCircleRadius, closestValue + SecondClosestCircleRadius)/2f;
            objectBubble.transform.position = circleObjects[(int)lowestDistances[0][1]].transform.position;
            objectBubble.GetComponent<RectTransform>().sizeDelta = new Vector2(circleObjects[(int)lowestDistances[0][1]].GetComponent<RectTransform>().sizeDelta.x + bubbleOffset, circleObjects[(int)lowestDistances[0][1]].GetComponent<RectTransform>().sizeDelta.y + bubbleOffset);
            print("TARGET:" + lowestDistances[1][1] + " | 1");
        }
    }
}