using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObserverCamController : MonoBehaviour
{

    Camera mainCamera;
    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.position = mainCamera.transform.position;
        this.transform.rotation = mainCamera.transform.rotation;
    }
}
