using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoverOnRayHit : MonoBehaviour {

    // Quick solution to highlight on select - maybe find a better way?
    public Material MaterialToHighlightObjects;
    private Material unhighlightedObject;

    // Use this for initialization
    void Start () {
        unhighlightedObject = this.GetComponent<Renderer>().material;
    }
	
	// Update is called once per frame
	void Update () {
        this.GetComponent<Renderer>().material = unhighlightedObject;
    }

    public void OnRayHit()
    {
        this.GetComponent<Renderer>().material = MaterialToHighlightObjects;
    }
}
