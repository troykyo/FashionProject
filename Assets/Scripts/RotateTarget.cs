using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateTarget : MonoBehaviour
{
    // this script makes this a rotatable target for the player
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // every gameobject.find can be replaced with a prefilled variable. do that.
    void Update()
    {
        Vector3 currentPosition = gameObject.transform.position;
        Vector3 currentRotation = gameObject.transform.rotation.eulerAngles;

        JointTracking jointScript = GameObject.Find("XR Origin (XR Rig)").GetComponent<JointTracking>();

        if (jointScript.holdRotationConfirmed)
        {
            //Debug.Log("Rotation should be set!");
            Debug.Log("Y rotation of palm is: " + jointScript.leftJointRotations[2].y);
            currentRotation = new Vector3(0, jointScript.leftJointRotations[2].y, 0);
            //GameObject.Find("XR Origin (XR Rig)").GetComponent<JointTracking>().leftJointRotations
        }

        //swipe turn
        /*if (Vector3.Distance(jointScript.rightJointPositions[11], currentPosition) < 10)
        {
            //if within range, track position to turn object

        }*/

        //pinch scale
        if (true)
        {
            //wheeeeeeeeeeee
        }


        gameObject.transform.rotation = Quaternion.Euler(currentRotation.x, currentRotation.y, currentRotation.z);
    }
}
