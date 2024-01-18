using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResetScr : MonoBehaviour
{
    // Update is called once per frame
    private void Awake()
    {
       // DontDestroyOnLoad(gameObject);
    }
    void Update()
    {
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyUp(KeyCode.R))
        {
            Debug.Log("restet");
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
