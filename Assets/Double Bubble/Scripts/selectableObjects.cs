using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class selectableObjects : MonoBehaviour {

    private BubbleSelection bubbleSelection;
    public GameObject radiusBubble;

    private void Start() {
        bubbleSelection = radiusBubble.GetComponent<BubbleSelection>();
    }

    private void OnTriggerStay(Collider collider) {
        if (collider.gameObject.tag == "InteractableObjects" && !bubbleSelection.selectableObjects.Contains(collider.gameObject)) {
            bubbleSelection.selectableObjects.Add(collider.gameObject);
        }
    }

}
