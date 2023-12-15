using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateTarget : MonoBehaviour
{
    // this script makes this a rotatable target for the player

    public bool isRotatable;
    public bool isMovable;
    void Start()
    {

    }



    // every gameobject.find can be replaced with a prefilled variable. do that.
    void Update()
    {
        Vector3 currentPosition = gameObject.transform.position;
        Vector3 currentRotation = gameObject.transform.rotation.eulerAngles;

        JointTracking jointScript = GameObject.Find("XR Origin (XR Rig)").GetComponent<JointTracking>();

        if (jointScript.holdRotationConfirmed && isRotatable)
        {
            //Debug.Log("Rotation should be set!");
            //Debug.Log("Y rotation of palm is: " + jointScript.leftJointRotations[2].y);
            currentRotation = new Vector3(0, jointScript.leftJointRotations[2].y + 180, 0);
            //GameObject.Find("XR Origin (XR Rig)").GetComponent<JointTracking>().leftJointRotations
        }

        if (jointScript.headPatConfirmed && isMovable)
        {
            //Debug.Log("It's gonna be adorable if this works!");
            currentPosition.x = jointScript.rightJointPositions[2].x;
            currentPosition.z = jointScript.rightJointPositions[2].z;
            currentPosition.y = gameObject.transform.position.y; //we want to keep the target at the same elevation as they were

        }

        //swipe turn
        /*if (Vector3.Distance(jointScript.rightJointPositions[11], currentPosition) < 10)
        {
            //if within range, track position to turn object

        }*/

        //pinch scale
        /*if (true)
        {
            //wheeeeeeeeeeee
        }*/


        gameObject.transform.rotation = Quaternion.Euler(currentRotation.x, currentRotation.y, currentRotation.z);
        gameObject.transform.position = currentPosition;
    }
}
