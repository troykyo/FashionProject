using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Management;
using UnityEngine.XR.Hands;
using UnityEngine.XR.Hands.Samples.VisualizerSample;

public class JointTracking : MonoBehaviour
{
    // Start is called before the first frame update
    XRHandSubsystem m_HandSubsystem;

    static readonly List<XRHandSubsystem> s_SubsystemsReuse = new List<XRHandSubsystem>();

    public GameObject XROrigin;

    public Vector3[] leftJointPositions;
    public Vector3[] rightJointPositions;
    public Vector3[] leftJointRotations;
    public Vector3[] rightJointRotations;

    public float fingerGunDistanceThreshold;

    public float grabRotationHoldThreshold;
    public float grabRotationHoldThresholdBase;
    public float grabRotationHoldThresholdExtended;

    public bool holdRotationConfirmed;
    //this bool to signal to other script to do rotating

    private bool confirmGesture;

    private bool isFingerGun = false;


    void Start()
    {
        var handSubsystems = new List<XRHandSubsystem>();
        SubsystemManager.GetSubsystems(handSubsystems);

        for (var i = 0; i < handSubsystems.Count; ++i)
        {
            var handSubsystem = handSubsystems[i];
            if (handSubsystem.running)
            {
                m_HandSubsystem = handSubsystem;
                break;
            }
        }

        if (m_HandSubsystem != null)
            m_HandSubsystem.updatedHands += OnUpdatedHands;
    }


    Pose ToWorldPose(XRHandJoint joint, Transform origin)
    {
        Pose xrOriginPose = new Pose(origin.position, origin.rotation);
        if (joint.TryGetPose(out Pose jointPose))
        {
            return jointPose.GetTransformedBy(xrOriginPose);
        }
        else
        {
            return Pose.identity;
        }
    }

    // Update is called once per frame
    void Update()
    {
        //HandVisualizer code below
        if (m_HandSubsystem != null && m_HandSubsystem.running)
            return;

        SubsystemManager.GetSubsystems(s_SubsystemsReuse);
        var foundRunningHandSubsystem = false;
        for (var i = 0; i < s_SubsystemsReuse.Count; ++i)
        {
            var handSubsystem = s_SubsystemsReuse[i];
            if (handSubsystem.running)
            {
                UnsubscribeHandSubsystem();
                m_HandSubsystem = handSubsystem;
                foundRunningHandSubsystem = true;
                break;
            }
        }

        if (!foundRunningHandSubsystem)
            return;

        SubscribeHandSubsystem();
        //HandVisualizer code above
        //Data on hand positions is gathered above, anything done with that data should be handled below
        //not quite how it works. just do it all in "OnUpdatedHands"

        //FingergunCheck(); //Note to self: Possibly split this so both hands can be checked individually if moving is desired during other actions for example

    }

    void UnsubscribeHandSubsystem()
    {
        if (m_HandSubsystem == null)
            return;

        m_HandSubsystem.updatedHands -= OnUpdatedHands;
    }

    void SubscribeHandSubsystem()
    {
        if (m_HandSubsystem == null)
            return;

        m_HandSubsystem.updatedHands += OnUpdatedHands;
    }

    void OnUpdatedHands(XRHandSubsystem subsystem,
        XRHandSubsystem.UpdateSuccessFlags updateSuccessFlags,
        XRHandSubsystem.UpdateType updateType)
    {

        if (updateType == XRHandSubsystem.UpdateType.Dynamic)
            return;

        leftJointPositions = GetAllJointPositions("left");
        rightJointPositions = GetAllJointPositions("right");
        leftJointRotations = GetAllJointRotations("left");
        rightJointRotations = GetAllJointRotations("right");

        Vector3[] GetAllJointPositions(string handedness) //Returns the Vectors for every joint on the requested hand. (left or right)
        {
            Vector3[] jointVectorData = new Vector3[XRHandJointID.EndMarker.ToIndex()+1];

            for (var i = XRHandJointID.BeginMarker.ToIndex();
                 i < XRHandJointID.EndMarker.ToIndex();
                 i++)
            {
                jointVectorData[i+1] = GetJointPosition(handedness, XRHandJointIDUtility.FromIndex(i));
                //Debug.Log("Here's that data: " + jointVectorData[i]);
            }

            //Debug.Log(jointVectorData[6]);
            return jointVectorData;

        }

        Vector3[] GetAllJointRotations(string handedness)
        {
            Vector3[] jointRotationData = new Vector3[XRHandJointID.EndMarker.ToIndex()+1];

            for (var i = XRHandJointID.BeginMarker.ToIndex();
                 i < XRHandJointID.EndMarker.ToIndex();
                 i++)
            {
                jointRotationData[i+1] = GetJointRotation(handedness, XRHandJointIDUtility.FromIndex(i));
            }

            return jointRotationData;
        }

        Vector3 GetJointPosition(string handedness, XRHandJointID jointID)
        {
            //Debug.Log("Currently trying to get the position from joint number " + jointID.ToIndex() + " from the " + handedness + " hand.");
            if (handedness == "left")
            {
                return ToWorldPose(subsystem.leftHand.GetJoint(jointID), XROrigin.transform).position;
            }
            else if (handedness == "right")
            {
                return ToWorldPose(subsystem.rightHand.GetJoint(jointID), XROrigin.transform).position;
            }
            else
            {
                Debug.LogError("Handedness incorrectly identified! Joint can't be found if your hand is neither left nor right!");
                return new Vector3();
            }
        } //returns the postion of the joint as a vector3

        Vector3 GetJointRotation(string handedness, XRHandJointID jointID)
        {
            if (handedness == "left")
            {
                return ToWorldPose(subsystem.leftHand.GetJoint(jointID), XROrigin.transform).rotation.eulerAngles;
            }
            else if (handedness == "right")
            {
                return ToWorldPose(subsystem.rightHand.GetJoint(jointID), XROrigin.transform).rotation.eulerAngles;
            }
            else
            {
                Debug.LogError("Handedness incorrectly identified! Joint can't be found if your hand is neither left nor right!");
                return new Quaternion().eulerAngles;
            }
        } //returns the rotation of the joint, as a quaternion converted to an euler angle

        Debug.Log("Confirmation is: " + confirmGesture);
        //confirmGesture = false;
        //ChangePoseCheck();
        if (confirmGesture)
        {
            //FingergunCheck();
            
        }
        GrabRotateCheck();


        Debug.Log("Palm rotation is: " + leftJointRotations[2]);
    }

    //note: in rotation for the hands, Z at 0 is the palm pointing down.

    //for reference, these are the IDs assigned to each joint:
    /*Invalid = 0
    BeginMarker = 1
    Wrist = 1
    Palm = 2
    ThumbMetacarpal = 3
    ThumbProximal = 4
    ThumbDistal = 5
    ThumbTip = 6
    IndexMetacarpal = 7
    IndexProximal = 8
    IndexIntermediate = 9
    IndexDistal = 10
    IndexTip = 11
    MiddleMetacarpal = 12
    MiddleProximal = 13
    MiddleIntermediate = 14
    MiddleDistal = 15
    MiddleTip = 16
    RingMetacarpal = 17
    RingProximal = 18
    RingIntermediate = 19
    RingDistal = 20
    RingTip = 21
    LittleMetacarpal = 22
    LittleProximal = 23
    LittleIntermediate = 24
    LittleDistal = 25
    LittleTip = 26
    EndMarker = 27
    */

    //NOTE TO SELF: PUT IN JOINTS USED INTO THE METHOD AS A PARAMETER, TO MORE EASILY WORK WITH THEM IN THE METHOD ITSELF.

    //this checks if a hand is making the designated pose to change the function you want to use
    void ChangePoseCheck()
    {
        //palm rotation should be about (270, 0, 180) for face facing
        if ((rightJointRotations[2].x <= 280) || (rightJointRotations[2].x >= 260))
        {
            if ((rightJointRotations[2].y <= 10) || (rightJointRotations[2].y >= 350))
            {
                if ((rightJointRotations[2].z <= 190) || (rightJointRotations[2].z >= 170))
                {
                    Debug.Log("Palm is facing face!");
                    confirmGesture = true;

                }
            }
        }
    }

    //This checks if the hands are currently in a "fingergun" position, which would then move the player
    void FingergunCheck()
    {
        // Calculate the distance between the thumb and index tips.

        float LeftDistance = Vector3.Distance(leftJointPositions[6], leftJointPositions[11]);
        float RightDistance = Vector3.Distance(rightJointPositions[6], rightJointPositions[11]);

        //float leftWristDistance = Vector3.Distance(leftMiddleTipPosition, leftWristPosition);
        //float rightWristDistance = Vector3.Distance(leftMiddleTipPosition, leftWristPosition);

        // Check if the distance is below the threshold for a finger gun gesture.
        if ((LeftDistance > fingerGunDistanceThreshold) /*&& (leftWristDistance < fingerGunDistanceThreshold)*/)
        {
            // Finger gun gesture recognized.
            isFingerGun = true;
            //Debug.Log("Finger Gun Gesture Recognized!");
            MovePlayer(false, true);

        }
        else
        {
            // Finger gun gesture not recognized.
            //isFingerGun = false;
        }

        if ((RightDistance > fingerGunDistanceThreshold) /*&& (rightWristDistance < fingerGunDistanceThreshold)*/)
        {
            // Finger gun gesture recognized.
            isFingerGun = true;
            //Debug.Log("Finger Gun Gesture Recognized!");
            MovePlayer(false, false);

        }
        else
        {
            // Finger gun gesture not recognized.
            //isFingerGun = false;
        }
    }

    void GrabRotateCheck()
    {
        if (((leftJointRotations[2].z <= 0 + grabRotationHoldThreshold) || (leftJointRotations[2].z >= 360 - grabRotationHoldThreshold)) && ((leftJointRotations[2].x <= 0 + grabRotationHoldThreshold) || (leftJointRotations[2].x >= 360 - grabRotationHoldThreshold)))
        {
            //Debug.Log("Hand points down enough!");
            //if your palm is facing generally down, it's good enough

            holdRotationConfirmed = true;

            grabRotationHoldThreshold = grabRotationHoldThresholdExtended;

            //activate arrow model
            GameObject.Find("Pointer").gameObject.GetComponent<MeshRenderer>().enabled = true;
            GameObject.Find("hand visualizer").GetComponent<HandVisualizer>().drawMeshes = false;
            GameObject.Find("Pointer").transform.position = leftJointPositions[2];
            GameObject.Find("Pointer").transform.rotation = Quaternion.Euler(-90,0,leftJointRotations[2].y);

        }
        else
        {
            grabRotationHoldThreshold = grabRotationHoldThresholdBase;
            GameObject.Find("Pointer").gameObject.GetComponent<MeshRenderer>().enabled = false;
            GameObject.Find("hand visualizer").GetComponent<HandVisualizer>().drawMeshes = true;
            holdRotationConfirmed = false;
        }
        //In this function we will be trying to check for a "jar opening" gesture, which will allow a user to rotate an object.

    }

    void MovePlayer(bool cameraMove, bool leftHanded) //note: movement pointing only works with left currently. fix this.
    {
        if (cameraMove)
        {
            //Move player based on looking direction
            gameObject.transform.position += new Vector3(GameObject.Find("Main Camera").transform.forward.x, 0, GameObject.Find("Main Camera").transform.forward.z) * 0.05f;
        }
        else
        {
            if (leftHanded)
            {
                gameObject.transform.position += new Vector3(GameObject.Find("Left Hand").transform.forward.x, 0, GameObject.Find("Left Hand").transform.forward.z).normalized * 0.05f;
            }
            else
            {
                gameObject.transform.position += new Vector3(GameObject.Find("Left Hand").transform.forward.x, 0, GameObject.Find("Left Hand").transform.forward.z).normalized * 0.05f;
            }
        }
    }

}
