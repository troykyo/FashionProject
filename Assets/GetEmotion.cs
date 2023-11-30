using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetEmotion : MonoBehaviour
{
    [SerializeField] VRM.VRMBlendShapeProxy vRMBlendShapeProxy;
    [SerializeField] GameObject enviorment;
    environmontScr environmontScr;

    private void Start()
    {
        environmontScr = enviorment.GetComponent<environmontScr>();
    }
    // Update is called once per frame
    void Update()
    {
        foreach (VRM.BlendShapeClip key in vRMBlendShapeProxy.BlendShapeAvatar.Clips)
        {
            if (key.BlendShapeName == "Angry" && vRMBlendShapeProxy.m_merger.GetValue(key.Key) > 0.95f)
            {
                environmontScr.mode = false;
                environmontScr.swap();
            }
            if (key.BlendShapeName == "Fun" && vRMBlendShapeProxy.m_merger.GetValue(key.Key) > 0.95f)
            {
                environmontScr.mode = true;
                environmontScr.swap();
            }
            else if (key.BlendShapeName == "Joy" && vRMBlendShapeProxy.m_merger.GetValue(key.Key) > 0.95f)
            {
                environmontScr.mode = true;
                environmontScr.swap();
            }
        }
    }
}
