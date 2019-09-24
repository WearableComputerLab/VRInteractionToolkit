using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SquadMenu : MonoBehaviour {
	
	 /* SQUAD implementation by Kieran May
     * University of South Australia
     * 
     * SQUAD is an extension of Sphere-Casting which places selected objects into a QUAD menu for more precise selection
     * 
     *  Copyright(C) 2019 Kieran May
	 *
	 *  This program is free software: you can redistribute it and/or modify
	 *  it under the terms of the GNU General Public License as published by
	 *  the Free Software Foundation, either version 3 of the License, or
	 *  (at your option) any later version.
	 * 
	 *  This program is distributed in the hope that it will be useful,
	 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
	 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
	 *  GNU General Public License for more details.
	 *
	 *  You should have received a copy of the GNU General Public License
	 *  along with this program.If not, see<http://www.gnu.org/licenses/>.
	 */

    public GameObject panel;
    public GameObject prefabText;
    public Material quadrantMaterial;
    public Material outlineMaterial;
    public Material triangleMaterial;
    public GameObject cameraHead;
    private bool quadrantPicked = false;
    private Transform[] TriangleQuadrant = new Transform[4];
	public bool enableText = false;
    internal SphereCasting sphereCasting;
    public List<GameObject> selectableObjects = new List<GameObject>();

    private void destroyChildGameObjects() {
        for(int i = 0; i < 4; i++) {
            foreach(Transform child in TriangleQuadrant[i].transform) {
                GameObject.Destroy(child.gameObject);
            }
        }
        foreach (Transform child in panel.transform) {
            if (child.gameObject.name != "CreateTrianglesSprite" && !child.gameObject.name.Contains("TriangleQuad")) {
                GameObject.Destroy(child.gameObject);
            } else if (child.gameObject.name.Contains("TriangleQuad") && !child.gameObject.name.Contains("Placeholder")) {
                child.gameObject.transform.GetComponent<Renderer>().material = triangleMaterial;
            }
        }

    }

    public bool isActive() {
        return panel.activeInHierarchy;
    }

    public bool quadrantIsPicked() {
        return quadrantPicked;
    }

    public void hideAllGameObjects() {
        GameObject[] allObjects = SceneManager.GetActiveScene().GetRootGameObjects();
        foreach (GameObject obj in allObjects) {
            if (obj.name != "CreateTriangles" && obj.name != "[CameraRig]" && obj.name != "[SteamVR]") {
                print("Hidden Object:" + obj.name);
                obj.SetActive(false);
            }
        }
    }

    public void showAllGameObjects() {
        GameObject[] allObjects = SceneManager.GetActiveScene().GetRootGameObjects();
        foreach (GameObject obj in allObjects) {
            if (obj.name != "CreateTriangles" && obj.name != "[CameraRig]" && obj.name != "[SteamVR]") {
                print("Hidden Object:" + obj.name);
                obj.SetActive(true);
            }
        }
    }

    public void disableSQUAD() {
        clearList();
		initialLoop = false;
        //showAllGameObjects();
        panel.transform.SetParent(cameraHead.transform);
        panel.SetActive(false);
        SphereCasting.inMenu = false;
        quadrantPicked = false;
        pickedObject = null;
        destroyChildGameObjects();
        print("Squad disabled..");
    }

	/*public void initializeTriangles() {
		panel.transform.SetParent (cameraHead.transform);
		panel.SetActive (false);
		panel.transform.localEulerAngles = new Vector3 (0f, 0f, 0f);
		panel.transform.localPosition = new Vector3 (0f, 0f, 1f);
	}*/

    private bool pickedUpObject = false; //ensure only 1 object is picked up at a time
    private GameObject pickedObject;
    private GameObject lastPickedObject;
    private Material oldPickedObjectMaterial;

    public void selectObject(GameObject obj) {
        //print("picked object:" + pickedObject);
        quadrantPicked = true;
        if(sphereCasting.controllerEvents() == SphereCasting.ControllerState.TRIGGER_DOWN && quadrantIsPicked() == true) {
        //if (controller.GetPressDown(SteamVR_Controller.ButtonMask.Trigger) && quadrantIsPicked() == true && pickedObject == null && obj.transform.parent == panel.transform && !obj.name.Contains("TriangleQuad")) {
            //disableSQUAD();
            //pickedObject = obj;
            //string objName = obj.name.Substring(0, obj.name.Length-7);
            string objName = obj.name;
            //print("obj picked:" + objName);
            pickedObject = GameObject.Find(objName);
            lastPickedObject = pickedObject;
            print("Final picked object:" + objName);
			if (pickedObject.transform.GetComponent<Renderer> () != null) {
				oldPickedObjectMaterial = pickedObject.transform.GetComponent<Renderer> ().material;
				pickedObject.transform.GetComponent<Renderer> ().material = outlineMaterial;
			}
			disableSQUAD();
        }
    }

    private void clearObjects() {

    }

    public void refineQuad(GameObject obj) {
        int val = 0;
        int count = 0;
        if (obj.name == "TriangleQuad North") {
            val = 0;
        } else if(obj.name == "TriangleQuad South") {
            val = 1;
        } else if(obj.name == "TriangleQuad East") {
            val = 2;
        } else if(obj.name == "TriangleQuad West") {
            val = 3;
        }
        List<GameObject> quadObjs = new List<GameObject>();
        foreach (Transform child in TriangleQuadrant[val]) {
            print(child.name+" in: " + TriangleQuadrant[val]);
            quadObjs.Add(child.gameObject);
            count++;
        }
        destroyChildGameObjects();
        print("----- REFINING QUAD -----");
        print(count);
        if(count == 1) {
            selectObject(quadObjs[0]);
        } else {
			initialLoop = true;
            generate2DObjects(quadObjs);
        }
    }

    private void setQuadrants() { //NSEW
        foreach (Transform child in panel.transform) {
            print("quad:"+child.transform.name);
            if (child.name == "TriangleQuad North Placeholder") {
                TriangleQuadrant[0] = child;
            } else if(child.name == "TriangleQuad South Placeholder") {
                TriangleQuadrant[1] = child;
            } else if(child.name == "TriangleQuad East Placeholder") {
                TriangleQuadrant[2] = child;
            } else if (child.name == "TriangleQuad West Placeholder") {
                TriangleQuadrant[3] = child;
            }
        }
    }


    public void enableSQUAD(List<GameObject> obj) {
        if (sphereCasting.trackedObj != null) {
            if (sphereCasting.controllerEvents() == SphereCasting.ControllerState.TRIGGER_DOWN && pickedUpObject == false) {
                print("EnableSquad() called");
                SphereCasting.inMenu = true;
                panel.SetActive(true);
                setQuadrants();
                //hideAllGameObjects();
                generate2DObjects(obj);
                if (lastPickedObject != null) {
                    lastPickedObject.transform.GetComponent<Renderer>().material = oldPickedObjectMaterial;
                }
            }
        }
    }

    private GameObject lastQuad;
    private Material oldMaterial;
    public void hoverQuad(GameObject obj) {
        //print("obj contians:"+obj.name.Contains("TriangleQuad"));
        if(obj.name.Contains("TriangleQuad") && isActive() == true) {
            if(lastQuad == null) {
                oldMaterial = obj.transform.GetComponent<Renderer>().material;
                lastQuad = obj;
                obj.transform.GetComponent<Renderer>().material = quadrantMaterial;
            } else {
                if (lastQuad != obj) {
                    lastQuad.transform.GetComponent<Renderer>().material = oldMaterial;
                    obj.transform.GetComponent<Renderer>().material = quadrantMaterial;
                    lastQuad = obj;
                }
                if(sphereCasting.controllerEvents() == SphereCasting.ControllerState.TRIGGER_DOWN) {
                    print("Quad selected:" + obj.name);
                    refineQuad(obj);
                }
            }
        }
    }


    public void selectQuad(GameObject obj) {
        if (sphereCasting.controllerEvents() == SphereCasting.ControllerState.TRIGGER_DOWN) {
            if (obj.name.Contains("TriangleQuad") && isActive() == true && quadrantPicked == false) {
                Renderer rend = obj.transform.GetComponent<Renderer>();
                //rend.material.color = Color.blue;
                rend.material = quadrantMaterial;
                quadrantPicked = true;
                //obj.transform.GetComponent<Renderer>().material.color = Color.clear;
            }
        }
    }

    //Stores 36 objects

    private GameObject pickedObjText = null;
    private GameObject pickedObj2D = null;
    private GameObject pickedObj = null;
    private int[] imageSlots;
    private int[,] locChanges = new int[,] { {1,2}, { 2, 3 } };
    private float[,] left = new float[,] { { -0.35f, 0.2f },
                                              { -0.4f, 0.1f }, { -0.3f, 0.1f },
                                              { -0.4f, 0f }, { -0.3f, 0f }, {-0.2f, 0f},
                                              { -0.4f, -0.1f }, {-0.3f, -0.1f},
                                              { -0.35f, -0.2f }};

    private float[,] right = new float[,] { { 0.35f, 0.2f },
                                              { 0.4f, 0.1f }, { 0.3f, 0.1f },
                                              { 0.4f, 0f }, { 0.3f, 0f }, {0.2f, 0f},
                                              { 0.4f, -0.1f }, {0.3f, -0.1f},
                                              { 0.35f, -0.2f }};


    private float[,] up = new float[,] { { -0.1f, 0.4f }, { 0f, 0.4f }, { 0.1f, 0.4f },
                                         { -0.1f, 0.3f }, { 0f, 0.3f }, { 0.1f, 0.3f },
                                         { -0.1f, 0.2f }, { 0f, 0.2f }, { 0.1f, 0.2f }};


    private float[,] down = new float[,] { { -0.1f, -0.4f }, { 0f, -0.4f }, { 0.1f, -0.4f },
                                         { -0.1f, -0.3f }, { 0f, -0.3f }, { 0.1f, -0.3f },
                                         { -0.1f, -0.2f }, { 0f, -0.2f }, { 0.1f, -0.2f }};

	private float scaleAmount = 10f;
	private bool initialLoop = false;
    void generate2DObjects(List<GameObject> pickedObject) {
        imageSlots = new int[9];
        panel.transform.SetParent(null);
        //print("Amount of objects selected:" + pickedObject.Count);
        for (int i = 0; i < pickedObject.Count; i++) {
            //print("object:" + pickedObject[i].name + " | count:" + (i+1));
            pickedObj = pickedObject[i];
            int stage = 1;
            if ((i + 1) % 1 == 0) {
                stage = 1;
            } if ((i + 1) % 2 == 0) {
                stage = 2;
            } if ((i + 1) % 3 == 0) {
                stage = 3;
            } if ((i + 1) % 4 == 0) {
                stage = 4;
            }
            //print("object:" + pickedObject[i].name + " | count:" + (i + 1) + " | stage:"+stage);
            pickedObj2D = Instantiate(pickedObject[i], new Vector3(0f, 0f, 0f), Quaternion.identity) as GameObject;
            pickedObj2D.name = pickedObject[i].name;
			//pickedObj2D.layer = 99;
            pickedObj2D.transform.SetParent(panel.transform, false);
			if (initialLoop == false) {
				pickedObj2D.transform.localScale = new Vector3 (pickedObject [i].transform.lossyScale.x / scaleAmount, pickedObject [i].transform.lossyScale.y / scaleAmount, pickedObject [i].transform.lossyScale.z / scaleAmount);
			}
			print (pickedObject[i].transform.lossyScale.x + " | " + pickedObject[i].transform.name);
            //pickedObj2D.transform.localScale = new Vector3(0.0625f, 0.0625f, 0f);
            pickedObj2D.transform.localRotation = Quaternion.identity;
			if (pickedObj2D.GetComponent<Collider> () != null) { Destroy (pickedObj2D.GetComponent<Collider> ());	}
			if (pickedObj2D.GetComponent<Rigidbody> () == null) {
				pickedObj2D.gameObject.AddComponent<Rigidbody> ();
			}
			pickedObj2D.GetComponent<Rigidbody>().isKinematic = true;
            print("New object generated:" + pickedObj2D.name);
            prefabText.GetComponent<TextMesh>().text = pickedObj.gameObject.name;
			if (enableText == true) {
				pickedObjText = Instantiate (prefabText, new Vector3 (0f, 0f, 0f), Quaternion.identity) as GameObject;
				pickedObjText.transform.SetParent (pickedObj2D.transform, false);
				pickedObjText.GetComponent<TextMesh> ().fontSize = 250;
				pickedObjText.transform.localRotation = Quaternion.identity;
			}

            int pos = 0;
            float posX = 0;
            float posY = 0;
            if (stage == 1) {
                imageSlots[0]++;
                pos = imageSlots[0]-1;
                posX = left[pos, 0];
                posY = left[pos, 1];
                pickedObj2D.transform.localPosition = new Vector3(posX, posY, -0.00001f);
                pickedObj2D.transform.SetParent(TriangleQuadrant[3], true);
				if (enableText == true) {
					pickedObjText.transform.localPosition = new Vector3 (0f, -0.6f, 0f);
				}
            } else if (stage == 2) {
                imageSlots[1]++;
                pos = imageSlots[1]-1;
                posX = right[pos, 0];
                posY = right[pos, 1];
                pickedObj2D.transform.localPosition = new Vector3(posX, posY, -0.00001f);
                pickedObj2D.transform.SetParent(TriangleQuadrant[2], true);
				if (enableText == true) {
					pickedObjText.transform.localPosition = new Vector3 (0f, -0.6f, 0f);
				}
            } else if (stage == 3) {
                imageSlots[2]++;
                pos = imageSlots[2] - 1;
                posX = up[pos, 0];
                posY = up[pos, 1];
                pickedObj2D.transform.localPosition = new Vector3(posX, posY, -0.00001f);
                pickedObj2D.transform.SetParent(TriangleQuadrant[0], true);
				if (enableText == true) {
					pickedObjText.transform.localPosition = new Vector3 (0f, -0.6f, 0f);
				}
            } else if (stage == 4) {
                imageSlots[3]++;
                pos = imageSlots[3] - 1;
                posX = down[pos, 0];
                posY = down[pos, 1];
                pickedObj2D.transform.localPosition = new Vector3(posX, posY, -0.00001f);
                pickedObj2D.transform.SetParent(TriangleQuadrant[1], true);
				if (enableText == true) {
					pickedObjText.transform.localPosition = new Vector3 (0f, -0.6f, 0f);
				}
            }
            //pickedObj2D.transform.localPosition = Vector3.zero;
        }
    }

    private void Start() {
        //disableSQUAD();
		//initializeTriangles();
    }

    public void clearList() {
        selectableObjects.Clear();
    }

    public List<GameObject> getSelectableObjects() {
        return selectableObjects;
    }

    public int selectableObjectsCount() {
        return selectableObjects.Count;
    }

    private void OnTriggerStay(Collider collider) {
		if (collider.gameObject.layer != LayerMask.NameToLayer("Ignore Raycast") && !selectableObjects.Contains(collider.gameObject) && collider.gameObject.layer == Mathf.Log(this.transform.parent.GetComponent<SphereCasting>().interactableLayer.value, 2)) {
            selectableObjects.Add(collider.gameObject);
        }
    }

}
