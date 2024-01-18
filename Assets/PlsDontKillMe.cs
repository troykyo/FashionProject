using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlsDontKillMe : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }
}
