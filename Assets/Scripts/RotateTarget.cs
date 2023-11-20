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

    // Update is called once per frame
    void Update()
    {
        if (GameObject.Find("XR Origin (XR Rig)").GetComponent<JointTracking>().holdRotationConfirmed)
        {
            Debug.Log("Rotation should be set!");
            Debug.Log("Y rotation of palm is: " + GameObject.Find("XR Origin (XR Rig)").GetComponent<JointTracking>().leftJointRotations[2].y);
            this.gameObject.transform.rotation = Quaternion.Euler(0, GameObject.Find("XR Origin (XR Rig)").GetComponent<JointTracking>().leftJointRotations[2].y, 0);
            //GameObject.Find("XR Origin (XR Rig)").GetComponent<JointTracking>().leftJointRotations
        }
    }
}
