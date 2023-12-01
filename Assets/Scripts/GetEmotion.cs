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
            float value = vRMBlendShapeProxy.m_merger.GetValue(key.Key);

            if (key.BlendShapeName == "Angry" && value > 0.85f ||
                key.BlendShapeName == "Sorrow" && value > 0.95f)
            {
                SetEnvironmentMode(false);
            }
            else if (key.BlendShapeName == "Fun" && value > 0.60f ||
                     key.BlendShapeName == "Joy" && value > 0.95f)
            {
                SetEnvironmentMode(true);
            }
        }
    }
    // Methode om de omgevingsmodus in te stellen en te wisselen
    private void SetEnvironmentMode(bool newMode)
    {
        environmontScr.mode = newMode;
        environmontScr.Swap();
    }
}
