using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoGo : MonoBehaviour {

    private bool HideTrueController = false;

    // Use this for initialization
    void Start () {
        
    }
	
	// Update is called once per frame
	void Update () {
        HideController();
    }

    // Hides the controller so we only show the "shadow controller"
    public void HideController()
    {
        //this.GetComponentInChildren<SteamVR_RenderModel>().gameObject.SetActive(false);
        Renderer[] renderers = this.transform.parent.GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            if (renderer.material.name == "Standard (Instance)")
            {
                renderer.enabled = HideTrueController;
            }
        }
    }
}
