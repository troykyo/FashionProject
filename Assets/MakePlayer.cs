using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakePlayer : MonoBehaviour
{
    [SerializeField]
    GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(GameObject.Find("VRplayer 2 weeee(Clone)"));
        if (GameObject.Find("VRplayer 2 weeee(Clone)") != null)
        {
            Debug.Log("stop it");
            return;
        }
        else
        {
            Instantiate(player);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
