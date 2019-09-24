using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingObject : MonoBehaviour {

    bool moving = true;
    Hook[] allHooks;

    public float bounds = 25; 

    void Update()
    {
        if (moving) {
            this.transform.Translate(transform.forward * 5f * Time.deltaTime, Space.World);
        }

        Vector3 pos = this.transform.position;
        if(pos.x > bounds || pos.x < bounds*-1 || pos.y > bounds || pos.y < bounds*-1 || pos.z > bounds || pos.z < bounds*-1) {
            Destroy(gameObject);
        }
        
    }

    void Start() {
        allHooks = FindObjectsOfType<Hook>();
		foreach(Hook hook in allHooks) {
			hook.addNewlySpawnedObjectToHook(this.gameObject);
            hook.selectedObject.AddListener(stopMovingOnceSelected);
		}
    }

    void stopMovingOnceSelected() {
        foreach(Hook hook in allHooks) {
			if(hook.selection == this.gameObject) {
                moving = false;
            }
		}      
    }
}
