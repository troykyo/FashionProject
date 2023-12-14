using System.Collections.Generic;
using UnityEngine;

public class vertexSnapping : MonoBehaviour
{

    private GameObject mannequin;

    private List<GameObject> clothObjects;

    private Material mat;
    private Rigidbody rb;

    private BoxCollider boxCol;

    public void Start()
    {
        clothObjects = new List<GameObject>();
        rb = GetComponent<Rigidbody>();
        mat = this.GetComponent<Material>();
        boxCol = GetComponent<BoxCollider>();
    }


    public void SetPin()
    {
        if (mannequin != null)
        {
            this.transform.parent = mannequin.transform;
            Debug.Log("mannequin set as parent");

            mat.SetFloat("Alpha", 0f);
        }

        foreach (GameObject obj in clothObjects)
        {
            obj.transform.parent = this.transform;
        }

        if (mannequin != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;

            boxCol.enabled = false;
        }
    }

    /* public void ClearPin()
     {
         foreach (GameObject obj in clothObjects)
         {
             obj.transform.parent = null;
         }

         clothObjects.Clear();

         this.transform.parent = null;
         this.gameObject.GetComponent<Material>().SetFloat("Alpha", 1f);

         mannequin = null;
         mannequinMesh = null;
     }*/

    private void OnCollisionEnter(Collision collision)
    {
        GameObject obj = collision.gameObject;

        if (obj.CompareTag("mannequin"))
        {
            mannequin = obj;
            Debug.Log("mannequin found");
        }

        if (obj.CompareTag("cloth") != null)
        {
            clothObjects.Add(obj);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        GameObject obj = collision.gameObject;
        if (obj.CompareTag("cloth"))
        {
            if (clothObjects.Contains(obj))
            {
                clothObjects.Remove(obj);
            }
        }
    }
}
