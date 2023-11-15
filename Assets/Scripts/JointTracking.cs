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

    private Vector3 leftIndexTipPosition;
    private Vector3 leftThumbTipPosition;
    private Vector3 rightIndexTipPosition;
    private Vector3 rightThumbTipPosition;

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

        if (updateType == XRHandSubsystem.UpdateType.Dynamic)
            return;


        Vector3[] getAllJoints(string handedness) //Returns the Vectors for every joint on the requested hand. (left or right)
        {
            Vector3[] jointData = new Vector3[XRHandJointID.EndMarker.ToIndex()]; //amount of joints is 27. filling this in like this, in case that ever changes

            for (var i = XRHandJointID.BeginMarker.ToIndex();
                 i < XRHandJointID.EndMarker.ToIndex();
                 i++)
            {
                jointData[i] = GetJointVector(handedness, XRHandJointIDUtility.FromIndex(i));
            }

            return jointData;

        }

        Vector3 GetJointVector(string handedness, XRHandJointID jointID)
        {
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
        }

        //bool leftHandTracked = subsystem.leftHand.isTracked;
        //bool rightHandTracked = subsystem.rightHand.isTracked;

        leftIndexTipPosition = GetJointVector("left", XRHandJointID.IndexTip);
        leftThumbTipPosition = GetJointVector("left", XRHandJointID.ThumbTip);

        rightIndexTipPosition = ToWorldPose(subsystem.rightHand.GetJoint(XRHandJointID.IndexTip), XROrigin.transform).position;
        rightThumbTipPosition = ToWorldPose(subsystem.rightHand.GetJoint(XRHandJointID.ThumbTip), XROrigin.transform).position;

        Vector3 leftMiddleTipPosition = ToWorldPose(subsystem.leftHand.GetJoint(XRHandJointID.MiddleTip), XROrigin.transform).position;
        Vector3 rightMiddleTipPosition = ToWorldPose(subsystem.rightHand.GetJoint(XRHandJointID.MiddleTip), XROrigin.transform).position;

        Vector3 leftWristPosition = ToWorldPose(subsystem.leftHand.GetJoint(XRHandJointID.Wrist), XROrigin.transform).position;
        Vector3 rightWristPosition = ToWorldPose(subsystem.rightHand.GetJoint(XRHandJointID.Wrist), XROrigin.transform).position;

        // Calculate the distance between the thumb and index tips.
        float LeftDistance = Vector3.Distance(leftThumbTipPosition, leftIndexTipPosition);
        float RightDistance = Vector3.Distance(rightThumbTipPosition, rightIndexTipPosition);

        float leftWristDistance = Vector3.Distance(leftMiddleTipPosition, leftWristPosition);
        float rightWristDistance = Vector3.Distance(leftMiddleTipPosition, leftWristPosition);

        /*Debug.Log("The index vector3 is: " + leftIndexTipPosition);
        Debug.Log("The thumb vector3 is: " + leftThumbTipPosition);
        Debug.Log("The distance between the thumb and the index is: " + LeftDistance);*/

        // Check if the distance is below the threshold for a finger gun gesture.
        if ((LeftDistance > fingerGunDistanceThreshold) && (leftWristDistance < fingerGunDistanceThreshold))
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
        if ((RightDistance > fingerGunDistanceThreshold) && (rightWristDistance < fingerGunDistanceThreshold))
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
