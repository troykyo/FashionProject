using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRM;

public class GetEmotion : MonoBehaviour
{
    [SerializeField] VRMBlendShapeProxy vRMBlendShapeProxy;
    [SerializeField] GameObject enviorment;
    environmontScr environmontScr;

    private void Start()
    {
        environmontScr = enviorment.GetComponent<environmontScr>();
    }
    // Update is called once per frame
    void Update()
    {
        foreach (BlendShapeClip key in vRMBlendShapeProxy.BlendShapeAvatar.Clips)
        {
            if (key.BlendShapeName == "Angry" && vRMBlendShapeProxy.m_merger.GetValue(key.Key) > 0.85f)
            {
                environmontScr.mode = false;
                environmontScr.swap();
            }
            else if (key.BlendShapeName == "Sorrow" && vRMBlendShapeProxy.m_merger.GetValue(key.Key) > 0.95f)
            {
                environmontScr.mode = false;
                environmontScr.swap();
            }
            if (key.BlendShapeName == "Fun" && vRMBlendShapeProxy.m_merger.GetValue(key.Key) > 0.60f)
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
