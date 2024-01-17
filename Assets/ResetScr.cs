using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResetScr : MonoBehaviour
{
    [SerializeField] private GameObject VrPlayer;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyUp(KeyCode.R))
        {
            Debug.Log("restet");
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

            VrPlayer.transform.position = new Vector3(0, 0.074f, -1);
        }
    }
}
