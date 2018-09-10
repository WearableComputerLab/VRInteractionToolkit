using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class disttest : MonoBehaviour {

    public GameObject obj1;
    public GameObject obj2;
    // Use this for initialization
    void Start () {
        print(Vector3.Distance(obj1.transform.position, obj2.transform.position));
	}
	
	// Update is called once per frame
	void Update () {
        print(Vector3.Distance(obj1.transform.position, obj2.transform.position));
    }
}
