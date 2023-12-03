using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Management;
using UnityEngine.XR.Hands;
using UnityEngine.XR.Hands.Samples.VisualizerSample;
using TMPro;

public class JointTracking : MonoBehaviour
{
    //set of actions and their gestures:
    /*
    - moving model (functional)| current gesture: flat right hand todo: enforce fingers away from hand (no fists)
    - turning model (funcional)| current gesture: flat left hand todo: enforce fingers away from hand (no fists)
    - grabbing "cloth" (whatever that might be)| current gesture: could be working with usual pinching grab (no cloth yet) |
    - sticking cloth to model | none | it's gonna be a hammer so: gesture should be fist (to grab hammer)
    - cutting cloth | | will be a scizzorsword so: flat straight sideways hand (chopping gesture)
    - sticking cloth together | |
    - coloring | |
    - tool safeguard | | possibily rocker sign?
    - translation safeguard CHECK
     */

    //todo list:
    /*
    - Above undifined gestures
    - position holding functionality (DONE)
    - determine gestures that need this functionality (translation safeguard CHECK)
    - visual confirmation that a gesture is being recognized (smol green sphere) (DONE)
    - 
    */

    XRHandSubsystem m_HandSubsystem;

    static readonly List<XRHandSubsystem> s_SubsystemsReuse = new List<XRHandSubsystem>();

    public GameObject XROrigin;
    public GameObject DebugCube1;
    public GameObject DebugCube2;
    public GameObject DebugCube3;

    public TextMeshProUGUI HandRotationText;

    //these hold all the different joints and their data. the array ID corresponds to those found in the XRHands documentation.
    public Vector3[] leftJointPositions;
    public Vector3[] rightJointPositions;
    public Vector3[] leftJointRotations;
    public Vector3[] rightJointRotations;

    //these thresholds are here to, for example, determine the rough orientation you hand needs to be in to recognize a gesture.
    //When active, theshold becomes bigger to make it easier to stay in the gesture.
    public float grabRotationHoldThreshold;
    public float grabRotationHoldThresholdBase;
    public float grabRotationHoldThresholdExtended;

    public float headPatThreshold;
    public float headPatThresholdBase;
    public float headPatThresholdExtended;

    public float straightFingerThreshold;

    public float fingerGunDistanceThreshold;

    //these bools to signal to another script to perform certain funtions.
    public bool headPatConfirmed;
    public bool holdRotationConfirmed;
    private bool translateConfirmed;

    private bool gestureconfirmed;

    public int translateTimer;
    public int translateTimerMax;

    private int gestureHoldTimer;
    //this bool to make the timer not go down while a gesture is still being recognized
    private bool holdup;

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

    //do not put anything related to joints into update, but rather into "OnUpdatedHands"
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

    //Also triggers every update (if hands are being tracked). put any joint stuff in here.
    //base of all this taken from HandVisualizer script
    void OnUpdatedHands(XRHandSubsystem subsystem,
        XRHandSubsystem.UpdateSuccessFlags updateSuccessFlags,
        XRHandSubsystem.UpdateType updateType)
    {
        DebugCube1.SetActive(false);
        DebugCube2.SetActive(false);
        DebugCube3.SetActive(false);
        if (updateType == XRHandSubsystem.UpdateType.Dynamic)
            return;

        leftJointPositions = GetAllJointPositions("left");
        rightJointPositions = GetAllJointPositions("right");
        leftJointRotations = GetAllJointRotations("left");
        rightJointRotations = GetAllJointRotations("right");

        Vector3[] GetAllJointPositions(string handedness) //Returns the Vectors for every joint on the requested hand. (left or right)
        {
            Vector3[] jointVectorData = new Vector3[XRHandJointID.EndMarker.ToIndex() + 1];

            for (var i = XRHandJointID.BeginMarker.ToIndex();
                 i < XRHandJointID.EndMarker.ToIndex();
                 i++)
            {
                jointVectorData[i + 1] = GetJointPosition(handedness, XRHandJointIDUtility.FromIndex(i));
                //Debug.Log("Here's that data: " + jointVectorData[i]);
            }

            //Debug.Log(jointVectorData[6]);
            return jointVectorData;

        }

        Vector3[] GetAllJointRotations(string handedness)
        {
            Vector3[] jointRotationData = new Vector3[XRHandJointID.EndMarker.ToIndex() + 1];

            for (var i = XRHandJointID.BeginMarker.ToIndex();
                 i < XRHandJointID.EndMarker.ToIndex();
                 i++)
            {
                jointRotationData[i + 1] = GetJointRotation(handedness, XRHandJointIDUtility.FromIndex(i));
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

        //Debug.Log("Confirmation is: " + confirmGesture);
        //confirmGesture = false;
        //ChangePoseCheck();

        /*if (Input.GetKey(KeyCode.A))
        {
            Debug.Log("true");
            confirmGesture = true;
        }

        if (Input.GetKey(KeyCode.B))
        {
            Debug.Log("false");
            confirmGesture = false;
        }*/

        //Debug.Log("Right palm rotation: " + rightJointRotations[2]);

        TranslatePoseCheck();

        if (translateTimer <= 0)
        {
            translateConfirmed = false;
            //DebugCube.SetActive(false);
        }
        else
        {
            //DebugCube.SetActive(true);
            translateTimer--;
        }

        //Debug.Log(translateConfirmed);
        //Debug.Log(gestureHoldTimer);

        if (translateConfirmed)
        {
            if (headPatConfirmed || holdRotationConfirmed)
            {
                translateTimer = translateTimerMax;
            }
            GrabRotateCheck();
            ModelMoveCheck();
        }

        if (holdRotationConfirmed || translateConfirmed || headPatConfirmed)
        {
            gestureconfirmed = true;
        }
        else
        {
            gestureconfirmed = false;
        }

        if (!gestureconfirmed && gestureHoldTimer>0 && !holdup)
        {
            gestureHoldTimer--;
        }
        holdup = false;
        SetDebugText();
        //Debug.Log("Palm rotation is: " + leftJointRotations[2]);
    }

    void SetDebugText()
    {
        //only one a at time pls
        //HandRotationText.text = "Right palm rotation in quaternions: " + Quaternion.Euler(rightJointRotations[2]).ToString();
        HandRotationText.text = "Right palm rotation: " + rightJointRotations[2].ToString();
    }

    //Notes for gesture recognition:
    /*
     * palm down: X&Z at 0
     * fingers point up: X at 270. increase to tilt fingers away from face, decrease to tilt towards. Y blank, Z at either 360/0 when fingers tilted away from face, or 180 when fingers tilted towards face. gimbal lock prevention is being a bitch with this one
     * Distance from tips to palm is about 0.13f when straight. (slightly more)
     */

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

    //From here, methods for checking various orientations. if statements.
    //NOTE TO SELF: PUT IN JOINTS USED INTO THE METHOD AS A PARAMETER, TO MORE EASILY WORK WITH THEM IN THE METHOD ITSELF.



    //this checks if a hand is making the designated pose to allow moving around
    void TranslatePoseCheck()
    {
        //be more lenient with higher X values. most people will have their hand palm aimed slightly downwards.
        //don't forget: if fingers are pointing more towards face, Z won't work.

        float indexDistance = Vector3.Distance(rightJointPositions[11], rightJointPositions[2]);
        float middleDistance = Vector3.Distance(rightJointPositions[16], rightJointPositions[2]);
        float ringDistance = Vector3.Distance(rightJointPositions[21], rightJointPositions[2]);

        /*if (indexDistance > straightFingerThreshold)
        {
            DebugCube1.SetActive(true);
        }
        if (middleDistance > straightFingerThreshold)
        {
            DebugCube2.SetActive(true);
        }
        if (ringDistance > straightFingerThreshold)
        {
            DebugCube3.SetActive(true);
        }*/


        if (((rightJointRotations[2].x <= 300) && (rightJointRotations[2].x >= 260)) && ((rightJointRotations[2].z <= 10) || (rightJointRotations[2].z >= 350)))
        {
            if ((indexDistance > straightFingerThreshold) && (middleDistance > straightFingerThreshold) && (ringDistance > straightFingerThreshold))
            {
                if (gestureHoldTimer > 60)
                {
                    translateTimer = translateTimerMax;
                    translateConfirmed = true;
                }
                else
                {
                    //Debug.Log("timer++ yoo");
                    holdup = true;
                    gestureHoldTimer++;
                    return;
                }
            }
        }
    }

    
    //Note: this was used as a first test to see if gesture recognition worked. it did. we don't need moving anymore, but the fingergun gesture could still be used for something else.
    void FingergunCheck() //This checks if the hands are currently in a "fingergun" position, which would then move the player
    {
        //Fingergun recognition currently only exists as "Are the index and thumb far enough away from each other?"

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
        float indexDistance = Vector3.Distance(leftJointPositions[11], leftJointPositions[2]);
        float middleDistance = Vector3.Distance(leftJointPositions[16], leftJointPositions[2]);
        float ringDistance = Vector3.Distance(leftJointPositions[21], leftJointPositions[2]);

        if (
               ((leftJointRotations[2].z <= 0 + grabRotationHoldThreshold) || (leftJointRotations[2].z >= 360 - grabRotationHoldThreshold))
            && ((leftJointRotations[2].x <= 0 + grabRotationHoldThreshold) || (leftJointRotations[2].x >= 360 - grabRotationHoldThreshold))
            && (indexDistance > straightFingerThreshold)
            && (middleDistance > straightFingerThreshold)
            && (ringDistance > straightFingerThreshold))
        {
            //Debug.Log("Hand points down enough!");
            //if your palm is facing generally down, it's good enough

            holdRotationConfirmed = true;

            grabRotationHoldThreshold = grabRotationHoldThresholdExtended;

            //activate arrow model
            GameObject.Find("Pointer").gameObject.GetComponent<MeshRenderer>().enabled = true;
            GameObject.Find("hand visualizer").GetComponent<HandVisualizer>().drawMeshes = false;
            GameObject.Find("Pointer").transform.position = new Vector3(leftJointPositions[2].x, leftJointPositions[2].y, leftJointPositions[2].z + 1);
            GameObject.Find("Pointer").transform.rotation = Quaternion.Euler(-90, 0, leftJointRotations[2].y + 180);

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

    void ModelMoveCheck()
    {
        float indexDistance = Vector3.Distance(rightJointPositions[11], rightJointPositions[2]);
        float middleDistance = Vector3.Distance(rightJointPositions[16], rightJointPositions[2]);
        float ringDistance = Vector3.Distance(rightJointPositions[21], rightJointPositions[2]);

        if (
               ((rightJointRotations[2].z <= 0 + headPatThreshold) || (rightJointRotations[2].z >= 360 - headPatThreshold)) 
            && ((rightJointRotations[2].x <= 0 + headPatThreshold) || (rightJointRotations[2].x >= 360 - headPatThreshold))
            && (indexDistance > straightFingerThreshold)
            && (middleDistance > straightFingerThreshold)
            && (ringDistance > straightFingerThreshold))
        {
            //Debug.Log("Hand points down enough!");
            //if your palm is facing generally down, it's good enough

            headPatConfirmed = true;
            headPatThreshold = headPatThresholdExtended;

        }
        else
        {
            //Debug.Log("bruh");
            headPatThreshold = headPatThresholdBase;
            headPatConfirmed = false;
            //Debug.Log("Moment" + holdRotationConfirmed);
        }

    }

    //Note: currently goes unused as we don't need to move the user at any time. Only used once for testing gesture recognition.
    void MovePlayer(bool cameraMove, bool leftHanded) //Note: movement pointing only works with left currently. fix this.
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
