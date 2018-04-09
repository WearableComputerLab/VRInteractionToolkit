using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateTriangle : MonoBehaviour {

    Mesh mesh;
    MeshRenderer meshRenderer;
    Vector3[] vertices;
    int[] triangles;

    public Material material;
    //public Material blendMaterial;
    private GameObject triangle = null;
    public GameObject newTriangle;

    // Use this for initialization
    void Start () {
        newTriangle = new GameObject();
        newTriangle.AddComponent<MeshFilter>();
        meshRenderer = newTriangle.AddComponent<MeshRenderer>();

        meshRenderer.material = material;

        mesh = new Mesh();
        newTriangle.GetComponent<MeshFilter>().mesh = mesh;
        newTriangle.AddComponent<MeshCollider>().sharedMesh = mesh;
        vertices = new[] {
            new Vector3(0,0,0),
            new Vector3(0,1,0),
            new Vector3(1,0,0),
        };
        mesh.vertices = vertices;
        triangles = new[] { 0, 1, 2 };
        mesh.triangles = triangles;

        triangle = Instantiate(newTriangle, new Vector3(0f, 0f, 0f), Quaternion.identity) as GameObject;
        triangle.transform.localEulerAngles = new Vector3(0f, 0f, 45f);
        //triangle.transform.name = "TriangleNorth";
        triangle.transform.name = "TriangleQuadObject";
        triangle.transform.SetParent(this.transform, false);
        //triangle.AddComponent<Renderer>().material = blendMaterial;
        //triangle.GetComponent<Renderer>().material.color = Color.clear;

        triangle = Instantiate(newTriangle, new Vector3(0f, 0f, 0f), Quaternion.identity) as GameObject;
        triangle.transform.localEulerAngles  = new Vector3(0f, 0f, 135f);
        //triangle.transform.name = "TriangleWest";
        triangle.transform.name = "TriangleQuadObject";
        triangle.transform.SetParent(this.transform, false);
        //triangle.AddComponent<Renderer>().material = blendMaterial;
        //triangle.GetComponent<Renderer>().material.color = Color.clear;

        triangle = Instantiate(newTriangle, new Vector3(0f, 0f, 0f), Quaternion.identity) as GameObject;
        triangle.transform.localEulerAngles = new Vector3(0f, 0f, 225f);
        triangle.transform.name = "TriangleQuadObject";
        //triangle.transform.name = "TriangleSouth";
        triangle.transform.SetParent(this.transform, false);
        //triangle.AddComponent<Renderer>().material = blendMaterial;
        //triangle.GetComponent<Renderer>().material.color = Color.clear;

        triangle = Instantiate(newTriangle, new Vector3(0f, 0f, 0f), Quaternion.identity) as GameObject;
        triangle.transform.localEulerAngles = new Vector3(0f, 0f, 315f);
        triangle.transform.name = "TriangleQuadObject";
        //triangle.transform.name = "TriangleEast";
        triangle.transform.SetParent(this.transform, false);
        //triangle.AddComponent<Renderer>().material = blendMaterial;
        //triangle.GetComponent<Renderer>().material.color = Color.clear;
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
