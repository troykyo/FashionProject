using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowCollisionIndicator : MonoBehaviour
{
    [SerializeField]
    private Material collisionIndicator, defaultMat;

    private Renderer objRenderer;

    [SerializeField]
    private Vector2 inputRegistration;

    public MannquinTranslationController mannequinController;

    private void Start()
    {
        objRenderer = this.GetComponent<Renderer>();
    }

    private void OnTriggerEnter(Collider other)
    {
        //check for hand
        if(other.tag == "Hand")
        {
            mannequinController.input = inputRegistration;
            objRenderer.material = collisionIndicator;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.tag == "Hand")
        {
            mannequinController.input = Vector2.zero;
            objRenderer.material = defaultMat;
        }
    }
}
