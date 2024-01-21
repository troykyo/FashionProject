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
    private MeshCollider meshCol;

    public void Start()
    {
        clothObjects = new List<GameObject>();
        rb = this.GetComponent<Rigidbody>();
        mat = this.GetComponent<Material>();
        
        if (this.GetComponent<MeshCollider>() != null)
        {
            meshCol = this.GetComponent<MeshCollider>();
        }
        if (this.GetComponent<BoxCollider>() != null)
        {
            boxCol = this.GetComponent<BoxCollider>();
        }
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
            //Vector3 thisLocation = transform.position;
            transform.SetParent(mannequin.transform, true);

            //Debug.Log("i am a child of: " + transform.parent.name);

            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
            //rb.useGravity = false;
            if (boxCol != null)
            {
                boxCol.enabled = false;
            }

            if (meshCol != null)
            {
                meshCol.enabled = false;
            }

            /*if (mat != null)
            {
                mat.SetFloat("Alpha", 0.5f);
            }*/

            foreach (GameObject obj in clothObjects)
            {
                //obj.transform.parent = this.transform;
                obj.transform.SetParent(this.transform, true);
                //this.transform.SetParent(obj.transform.parent);
                //obj.transform.localScale = new Vector3(1,1,1);
                //obj.GetComponent<Renderer>().material.color = Color.green;

                if (obj.GetComponent<Rigidbody>() != null)
                {
                    Rigidbody objRb = obj.GetComponent<Rigidbody>();

                    objRb.useGravity = false;
                    objRb.velocity = Vector3.zero;
                    objRb.angularVelocity = Vector3.zero;
                    if (obj.GetComponent<BoxCollider>() != null)
                    {
                        obj.GetComponent<BoxCollider>().enabled = false;
                    }
                    if (obj.GetComponent<MeshCollider>() != null)
                    {
                        obj.GetComponent<MeshCollider>().enabled = false;
                    }
                }
            }
            //this.GetComponent<MeshRenderer>().enabled = false;
            //this.GetComponent<vertexSnapping>().enabled = false;
        }

    }

    private void OnCollisionStay(Collision collision)
    {
        Debug.Log("I am colliding with: " + collision.gameObject.name);
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

        if (boxCol != null)
        {
            boxCol.enabled = true;
        }
        if (meshCol != null)
        {
            meshCol.enabled = true;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        //gameObject.GetComponent<XRGrabInteractable>().enabled = false;
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
