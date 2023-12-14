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

            if (key.BlendShapeName == "Angry" && value > 0.85f)
            {
                SetEnvironmentMode(1);
            } 
            else if (key.BlendShapeName == "Sorrow" && value > 0.95f)
            {
                SetEnvironmentMode(2);
            }
            else if (key.BlendShapeName == "Fun" && value > 0.60f ||
                     key.BlendShapeName == "Joy" && value > 0.95f)
            {
                SetEnvironmentMode(3);
            }
            else if (key.BlendShapeName == "Suprised" && value > 0.95f)
            {
                SetEnvironmentMode(4);
            }

        }
    }
    // Methode om de omgevingsmodus in te stellen en te wisselen
    private void SetEnvironmentMode(int newMode)
    {
        environmontScr.mode = newMode;
        environmontScr.Swap();
    }
}
