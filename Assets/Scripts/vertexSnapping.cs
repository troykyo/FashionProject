using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class vertexSnapping : MonoBehaviour
{

    private GameObject mannequin;

    private List<GameObject> clothObjects;

    public Material mat;
    private Rigidbody rb;

    private BoxCollider boxCol;

    public void Start()
    {
        clothObjects = new List<GameObject>();
        rb = this.GetComponent<Rigidbody>();
        mat = this.GetComponent<Material>();
        boxCol = this.GetComponent<BoxCollider>();
    }

    public void Update()
    {
        if (rb.velocity.magnitude > 0)
        {
            SetPin();
        }
    }


    public void SetPin()
    {
        if (mannequin != null)
        {
            this.transform.parent = mannequin.transform;

            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            //rb.isKinematic = true;
            rb.useGravity = false;

            boxCol.enabled = false;

            if (mat != null)
                mat.SetFloat("Alpha", 0.5f);

            foreach (GameObject obj in clothObjects)
            {
                obj.transform.parent = this.transform;

                if(obj.GetComponent<Rigidbody>() != null)
                {
                    Rigidbody objRb = obj.GetComponent<Rigidbody>();

                    objRb.useGravity = false;
                    objRb.velocity = Vector3.zero;
                    objRb.angularVelocity = Vector3.zero;
                    obj.GetComponent<BoxCollider>().enabled = false;
                }
            }
        }

    }

    public void ClearPin()
    {
        Debug.Log("Clear Pin");
        foreach (GameObject obj in clothObjects)
        {
            obj.transform.parent = null;
        }

        clothObjects.Clear();

        this.transform.parent = null;
        this.gameObject.GetComponent<Material>().SetFloat("Alpha", 1f);

        mannequin = null;

        rb.useGravity = true;

        boxCol.enabled = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        gameObject.GetComponent<XRGrabInteractable>().enabled = false;
        GameObject obj = collision.gameObject;

        if (obj.CompareTag("mannequin"))
        {
            mannequin = obj;

        }

        if (obj.CompareTag("cloth"))
        {
            clothObjects.Add(obj);
        }
    }

    /*private void OnCollisionExit(Collision collision)
    {
        GameObject obj = collision.gameObject;
        if (obj.CompareTag("cloth"))
        {
            if (clothObjects.Contains(obj))
            {
                clothObjects.Remove(obj);
            }
        }
    }*/
}
