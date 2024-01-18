using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakePlayer : MonoBehaviour
{
    [SerializeField]
    GameObject player;
    [SerializeField]
    GameObject reset;

    // Start is called before the first frame update
    void Start()
    {
        if (GameObject.Find("VRplayer 2 weeee(Clone)") != null)
        {
            return;
        }
        else
        {
            Instantiate(player);
            //Instantiate(reset);
        }
    }
}
