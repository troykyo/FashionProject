using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MannquinTranslationController : MonoBehaviour
{
    public Vector2 input;


    public Transform mannequin;

    [SerializeField]
    private float mannequinRotationSpeed, mannequinMovementSpeed;
    [SerializeField]
    private Vector2 mannequinZLimits;

    public void Update()
    {
        Rotate((int) input.x);
        Move((int) input.y);
    }

    public void Rotate(int direction)
    {
        mannequin.Rotate(Vector3.up * mannequinRotationSpeed * direction * Time.deltaTime);
    }

    public void Move(int direction)
    {
        mannequin.position = new Vector3(mannequin.position.x, mannequin.position.y,Mathf.Clamp(mannequin.position.z + direction * mannequinMovementSpeed * Time.deltaTime, mannequinZLimits.x, mannequinZLimits.y));
    }
}
