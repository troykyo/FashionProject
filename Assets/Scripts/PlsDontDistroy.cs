using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlsDontDistroy : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }
}
