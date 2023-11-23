using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddClothComponent : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            Debug.Log("the A button is being pressed");

            gameObject.AddComponent<Cloth>();

            Debug.Log("this thing should be a type of cloth now?");
        }
        
    }
}
