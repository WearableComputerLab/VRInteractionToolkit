using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class iSithController : MonoBehaviour {

    public iSithLaser laser1;
    public iSithLaser laser2;
    public GameObject interactionObject;

    void setCubeLocation()
    {
        // assuming 1 is pointing controller for test
        Vector3 d1 = laser1.transform.forward;
        Vector3 d2 = laser2.transform.forward;

        Vector3 p1 = laser1.transform.position;
        Vector3 p2 = laser2.transform.position;

        // as these two vectors will probably create skew lines (on different planes) have to calculate the points on the lines that are
        // closest to eachother and then getting the midpoint between them giving a fake 'intersection'
        // This is achieved by utilizing parts of the fromula to find the shortest distance between two skew lines
        Vector3 n1 = Vector3.Cross(d1, (Vector3.Cross(d2, d1)));
        Vector3 n2 = Vector3.Cross(d2, (Vector3.Cross(d1, d2)));

        // Figuring out point 1
        Vector3 localPoint1 = p1 + ((Vector3.Dot((p2 - p1), n2)) / (Vector3.Dot(d1, n2))) * d1;

        // Figuring out point 2
        Vector3 localPoint2 = p2 + ((Vector3.Dot((p1 - p2), n1)) / (Vector3.Dot(d2, n1))) * d2;

        Vector3 location = (localPoint1 + localPoint2) / 2f;

        /*
        Vector3 theVector = this.transform.forward;
        hitPoint = this.transform.position;
        float distance_formula_on_vector = Mathf.Sqrt(theVector.x * theVector.x + theVector.y * theVector.y + theVector.z * theVector.z);
        // Using formula to find a point which lies at distance on a 3D line from vector and direction
        hitPoint.x = hitPoint.x + (100 / (distance_formula_on_vector)) * theVector.x;
        hitPoint.y = hitPoint.y + (100 / (distance_formula_on_vector)) * theVector.y;
        hitPoint.z = hitPoint.z + (100 / (distance_formula_on_vector)) * theVector.z;
        */

        float distance_formula_on_vector = Mathf.Sqrt(d1.x * d1.x + d1.y * d1.y + d1.z * d1.z);

        float num = (distance_formula_on_vector * (localPoint1.x - p1.x)) / d1.x;

        if (num > 0)
        {
            interactionObject.transform.position = location;
        } else
        {
            interactionObject.transform.position = (p1 + p2) / 2f;
        }

        

    }


    public float AngleDir(Vector3 fwd, Vector3 targetDir, Vector3 up) // chang eto own code
    {
        Vector3 perp = Vector3.Cross(fwd, targetDir);
        float dir = Vector3.Dot(perp, up);

        if (dir > 0.0f)
        {
            return 1.0f;
        }
        else if (dir < 0.0f)
        {
            return -1.0f;
        }
        else
        {
            return 0.0f;
        }
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        setCubeLocation();
        //Vector3.Lerp(interactionObject.transform.position, getInteractionPoint(), 0.5f);
	}
}
