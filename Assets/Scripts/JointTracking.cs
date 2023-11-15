using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Management;
using UnityEngine.XR.Hands;

public class JointTracking : MonoBehaviour
{
    // Start is called before the first frame update
    XRHandSubsystem m_HandSubsystem;

    static readonly List<XRHandSubsystem> s_SubsystemsReuse = new List<XRHandSubsystem>();

    public GameObject XROrigin;

    //public GameObject handObject;
    /*private XRHand rightHand;
    private XRHand leftHand;
    private XRHandJoint indexTip;
    private XRHandJoint thumbTip;*/
    //use these instead. makes the code more readable

    Vector3[] leftJointPositions;
    Vector3[] rightJointPositions;
    Vector3[] leftJointRotations;
    Vector3[] rightJointRotations;

    public float fingerGunDistanceThreshold;
    

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

        /*Debug.Log("trying to find hands...");
        rightHand = m_HandSubsystem.rightHand;
        Debug.Log("right hand should now be: " + rightHand);

            Debug.Log("trying to get joints...");
            indexTip = rightHand.GetJoint(XRHandJointID.IndexTip);
            thumbTip = rightHand.GetJoint(XRHandJointID.ThumbTip);
            Debug.Log("Index joint should be: " + indexTip + ". Thumb joint should be: " + thumbTip);

        Debug.Log("locking joint positions...");
        indexTipPosition = ToWorldPose(indexTip, XROrigin.transform).position;
        thumbTipPosition = ToWorldPose(thumbTip, XROrigin.transform).position;
        Debug.Log("index and thumb should be at: " + indexTipPosition + ", and " + thumbTipPosition);*/

        //ChatGPT code
        
        //Debug.Log();

        //for loop for the list of joints (missing hand refrence(What is "hand" in this context?!?))
        /*for(var i = XRHandJointID.BeginMarker.ToIndex();
        i < XRHandJointID.EndMarker.ToIndex();
        i++)
{
            var trackingData = hand.GetJoint(XRHandJointIDUtility.FromIndex(i));

            if (trackingData.TryGetPose(out Pose pose))
            {
                // displayTransform is some GameObject's Transform component
                displayTransform.localPosition = pose.position;
                displayTransform.localRotation = pose.rotation;
            }
        }*/
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
        //for clarity for both myself and others using this:
        //"OnUpdatedHands" runs whenever "SubscribeHandSubsystem" runs, which on it's turn is ran every Update.

        //for reference, these are the IDs assigned to each joint:
        /*Invalid = 0,
        BeginMarker = 1,
        Wrist = 1,
        Palm = 2,
        ThumbMetacarpal = 3,
        ThumbProximal = 4,
        ThumbDistal = 5,
        ThumbTip = 6,
        IndexMetacarpal = 7,
        IndexProximal = 8,
        IndexIntermediate = 9,
        IndexDistal = 10,
        IndexTip = 11,
        MiddleMetacarpal = 12,
        MiddleProximal = 13,
        MiddleIntermediate = 14,
        MiddleDistal = 15,
        MiddleTip = 16,
        RingMetacarpal = 17,
        RingProximal = 18,
        RingIntermediate = 19,
        RingDistal = 20,
        RingTip = 21,
        LittleMetacarpal = 22,
        LittleProximal = 23,
        LittleIntermediate = 24,
        LittleDistal = 25,
        LittleTip = 26,
        EndMarker = 27
        */

        if (updateType == XRHandSubsystem.UpdateType.Dynamic)
            return;

        leftJointPositions = GetAllJointPositions("left");
        rightJointPositions = GetAllJointPositions("right");
        leftJointRotations = GetAllJointRotations("left");
        rightJointRotations = GetAllJointRotations("right");

        Vector3[] GetAllJointPositions(string handedness) //Returns the Vectors for every joint on the requested hand. (left or right)
        {
            Vector3[] jointVectorData = new Vector3[XRHandJointID.EndMarker.ToIndex()+1]; //amount of joints is 27. filling this in like this, in case that ever changes

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
            Vector3[] jointRotationData = new Vector3[XRHandJointID.EndMarker.ToIndex()+1]; //amount of joints is 27. filling this in like this, in case that ever changes

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
        } //returns the postion of the joint in a vector3

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

        //bool leftHandTracked = subsystem.leftHand.isTracked;
        //bool rightHandTracked = subsystem.rightHand.isTracked;

        //Debug.Log("LeftThumbtip is at: " + leftJointPositions[6]);

        // Calculate the distance between the thumb and index tips.

        //Debug.Log(leftJointPositions[6]);

        float LeftDistance = Vector3.Distance(leftJointPositions[6], leftJointPositions[11]);
        float RightDistance = Vector3.Distance(rightJointPositions[6], rightJointPositions[11]);

        //float leftWristDistance = Vector3.Distance(leftMiddleTipPosition, leftWristPosition);
        //float rightWristDistance = Vector3.Distance(leftMiddleTipPosition, leftWristPosition);

        /*Debug.Log("The index vector3 is: " + leftIndexTipPosition);
        Debug.Log("The thumb vector3 is: " + leftThumbTipPosition);
        Debug.Log("The distance between the thumb and the index is: " + LeftDistance);*/

        // Check if the distance is below the threshold for a finger gun gesture.
        if ((LeftDistance > fingerGunDistanceThreshold) /*&& (leftWristDistance < fingerGunDistanceThreshold)*/)
        {
            // Finger gun gesture recognized.
            isFingerGun = true;
            Debug.Log("Finger Gun Gesture Recognized!");

            //gameObject.transform.position += new Vector3(GameObject.Find("Main Camera").transform.forward.x, 0, GameObject.Find("Main Camera").transform.forward.z)*0.05f;
            gameObject.transform.position += new Vector3(GameObject.Find("Left Hand").transform.forward.x, 0, GameObject.Find("Left Hand").transform.forward.z).normalized * 0.05f;
            /*if (!isFingerGun)
            {
                
            }*/
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
            Debug.Log("Finger Gun Gesture Recognized!");

            gameObject.transform.position += new Vector3(GameObject.Find("Main Camera").transform.forward.x, 0, GameObject.Find("Main Camera").transform.forward.z) * 0.05f;
            //Turn this bit into a seperate function. to avoid more copy/pasting
            /*if (!isFingerGun)
            {
                
            }*/
        }
        else
        {
            // Finger gun gesture not recognized.
            //isFingerGun = false;
        }

    }
}
