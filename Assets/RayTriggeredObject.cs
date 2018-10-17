using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayTriggeredObject : MonoBehaviour {

    void OnTriggerEnter(Collider other) {
        print("Trigger:" + other.name);
    }
    void OnCollisionEnter(Collision collision) {
        //print("Collision:" + collision.name);
        print(collision);
    }
}
