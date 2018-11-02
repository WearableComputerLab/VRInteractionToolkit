using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WorldInMiniature : MonoBehaviour {

    /* World In Miniature implementation by Kieran May
     * University of South Australia
     * 
     * */

    internal SteamVR_TrackedObject trackedObj;
    internal SteamVR_TrackedObject trackedObjO; //tracked object other
    private SteamVR_Controller.Device controller;
    internal SteamVR_Controller.Device controllerO; //controller other
    public GameObject worldInMinParent;
    GameObject[] allSceneObjects;
    
    public bool WiMactive = false;
    public List<string> ignorableObjectsString = new List<string>{ "[CameraRig]", "Directional Light", "background"};
    public float scaleAmount = 20f;
    public LayerMask interactableLayer;
    public Material outlineMaterial;

    public enum InteractionType { Selection, Manipulation_Movement, Manipulation_Full };
    public InteractionType interacionType;

    public enum ControllerPicked { Left_Controller, Right_Controller };
    public ControllerPicked controllerPicked;

	public GameObject controllerRight;
	public GameObject controllerLeft;
	public GameObject cameraHead;

	private List<GameObject> listOfChildren = new List<GameObject>();
	private void findClonedObject(GameObject obj){
		if (null == obj)
			return;
		foreach (Transform child in obj.transform){
			if (null == child)
				continue;
			if (child.gameObject.GetComponent<Rigidbody> () != null) {
				child.gameObject.GetComponent<Rigidbody> ().isKinematic = true;
			}
			listOfChildren.Add(child.gameObject);
			findClonedObject(child.gameObject);
		}
	}

    void createWiM() {
        if (controller.GetPressDown(SteamVR_Controller.ButtonMask.ApplicationMenu)) {
            if (WiMactive == false) {
                WiMactive = true;
                print("Create world clone");
                for (int i = 0; i < allSceneObjects.Length; i++) {
                    if (!ignorableObjectsString.Contains(allSceneObjects[i].name)) {
                        GameObject cloneObject = Instantiate(allSceneObjects[i], new Vector3(0f, 0f, 0f), Quaternion.identity) as GameObject;
                        //cloneObject.transform.name = allSceneObjects[i].name;
                        cloneObject.transform.SetParent(worldInMinParent.transform, false);
                        if (cloneObject.gameObject.GetComponent<Rigidbody>() == null) {
                            cloneObject.gameObject.AddComponent<Rigidbody>();
                        }
                        cloneObject.gameObject.GetComponent<Rigidbody>().isKinematic = true;
                        //cloneObject.gameObject.AddComponent<Collider>();
                        //cloneObject.GetComponent<Collider>().attachedRigidbody.isKinematic = true;
                        cloneObject.transform.localScale = new Vector3(allSceneObjects[i].transform.lossyScale.x / scaleAmount, allSceneObjects[i].transform.lossyScale.y / scaleAmount, allSceneObjects[i].transform.lossyScale.z / scaleAmount);
                        cloneObject.transform.localRotation = Quaternion.identity;
                        if (cloneObject.transform.GetComponent<Renderer>() != null) {
                            //cloneObject.transform.GetComponent<Renderer>().material.color = Color.red;
                        }
                        float posX = allSceneObjects[i].transform.position.x / scaleAmount;
                        float posY = allSceneObjects[i].transform.position.y / scaleAmount;
                        float posZ = allSceneObjects[i].transform.position.z / scaleAmount;
                        cloneObject.transform.localPosition = new Vector3(posX, posY, posZ);
                    }
                }
				findClonedObject (worldInMinParent);
                //worldInMinParent.transform.SetParent(null);
                //worldInMinParent.transform.localEulerAngles = new Vector3(0f, cameraHead.transform.localEulerAngles.y-45f, 0f);
                worldInMinParent.transform.localEulerAngles = new Vector3(0f, trackedObj.transform.localEulerAngles.y - 45f, 0f);
                worldInMinParent.transform.Rotate(0, tiltAroundY, 0);
                //worldInMinParent.transform.localPosition -= new Vector3(0f, worldInMinParent.transform.position.y / 1.25f, 0f);
            } else if (WiMactive == true) {
                WiMactive = false;
                foreach (Transform child in worldInMinParent.transform) {
                    Destroy(child.gameObject);
                }
                worldInMinParent.transform.localPosition = new Vector3(0f, 0f, 0f);
                worldInMinParent.transform.SetParent(trackedObj.transform);
                resetAllProperties();
            }
        }
    }

    public GameObject selectedObject;

    public GameObject currentObjectCollided;


    internal bool objectPicked = false;
    internal Transform oldParent;

    private void resetAllProperties() {
        worldInMinParent.transform.localScale = new Vector3(1f, 1f, 1f);
        worldInMinParent.transform.localPosition = new Vector3(0f, 0f, 0f);
        worldInMinParent.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
    }

	// Use this for initialization
	void Start () {
        allSceneObjects = SceneManager.GetActiveScene().GetRootGameObjects();
        //allSceneObjects = FindObjectsOfType<GameObject>();
        worldInMinParent.transform.SetParent(trackedObj.transform);
        resetAllProperties();

		//adding colliders and collider scripts to controllers for WIM if they don't allready exist
		SphereCollider col;
		if ((col = trackedObj.transform.gameObject.GetComponent<SphereCollider> ()) == null) {
			
			col = trackedObj.transform.gameObject.AddComponent<SphereCollider> ();
			col.isTrigger = true;
			col.radius = 0.05f;
			trackedObj.transform.gameObject.AddComponent<ControllerColliderWIM> ();
		}
		SphereCollider col0;
		if((col0 = trackedObjO.transform.gameObject.GetComponent<SphereCollider> ()) == null) {
			
			col0 = trackedObjO.transform.gameObject.AddComponent<SphereCollider> ();
			col0.isTrigger = true;
			col0.radius = 0.05f;
			trackedObjO.transform.gameObject.AddComponent<ControllerColliderWIM> ();
		}
    }

    void Awake() {
        worldInMinParent = this.transform.Find("WorldInMinParent").gameObject;
        if (controllerPicked == ControllerPicked.Right_Controller) {
            trackedObj = controllerRight.GetComponent<SteamVR_TrackedObject>();
            trackedObjO = controllerLeft.GetComponent<SteamVR_TrackedObject>();
        } else if (controllerPicked == ControllerPicked.Left_Controller) {
            trackedObj = controllerLeft.GetComponent<SteamVR_TrackedObject>();
            trackedObjO = controllerRight.GetComponent<SteamVR_TrackedObject>();
        } else {
            print("Couldn't detect trackedObject, please specify the controller type in the settings.");
            Application.Quit();
        }
    }
    private float tiltAroundY = 0f;
    public float tiltSpeed = 2f; //2x quicker than normal
    // Update is called once per frame
    void Update () {
        controller = SteamVR_Controller.Input((int)trackedObj.index);
        controllerO = SteamVR_Controller.Input((int)trackedObjO.index);
        createWiM();
        if (WiMactive == true) {
            tiltAroundY = controller.GetAxis(Valve.VR.EVRButtonId.k_EButton_Axis0).y;
            if (controller.GetPress(SteamVR_Controller.ButtonMask.Touchpad)) {
                worldInMinParent.transform.Rotate(0, tiltAroundY* tiltSpeed, 0);
            }
        }
        if (controllerO.GetPressUp(SteamVR_Controller.ButtonMask.Trigger) && selectedObject == true) {
            selectedObject.transform.SetParent(oldParent);
            //print("changed pos:" + selectedObject.transform.localPosition);
            GameObject realObject = GameObject.Find(selectedObject.name);
            print(realObject.transform.position + " | " + realObject.transform.localPosition);
            print(selectedObject.transform.position + " | " + selectedObject.transform.localPosition);
            realObject.transform.localPosition = selectedObject.transform.localPosition;
            //realObject.transform.localPosition = new Vector3(selectedObject.transform.localPosition.x*scaleAmount, selectedObject.transform.localPosition.y*scaleAmount, selectedObject.transform.localPosition.z*scaleAmount);
            realObject.transform.localEulerAngles = selectedObject.transform.localEulerAngles;
            objectPicked = false;
        }

        }
}