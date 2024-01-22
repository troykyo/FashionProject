using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class vertexSnapping : MonoBehaviour
{

    private GameObject mannequin;

    private List<GameObject> clothObjects;

    public Material mat;
    public bool colliding;
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
        colliding = false;
    }

    public void Update()
    {
        if (rb.velocity.magnitude > 0)
        {
            SetPin();
            Debug.Log("rb.velocity.magnitude > 0");
        }
    }


    public void SetPin()
    {
        if (mannequin != null)
        {
            Debug.Log("set pin");
            colliding = true;
            //Vector3 thisLocation = transform.position;
            transform.SetParent(mannequin.transform);

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
                Debug.Log(obj);
                //obj.transform.parent = this.transform;
                obj.transform.SetParent(this.transform);
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
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        //Debug.Log("I am colliding with: " + collision.gameObject.name);
    }

    public void ClearPin()
    {
        Destroy(this.gameObject);
    }

    /*private void OnCollisionEnter(Collision collision)
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
            Debug.Log("Item 1 in list:" + clothObjects);
        }
    }*/

    private void OnTriggerEnter(Collider other)
    {
        GameObject obj = other.gameObject;

        if (obj.CompareTag("mannequin"))
        {
            mannequin = obj;
            Debug.Log(obj);
        }

        if (obj.CompareTag("cloth"))
        {
            clothObjects.Add(obj);
            Debug.Log("Item 1 in list:" + clothObjects);
        }
    }

    /*private void OnCollisionExit(Collision collision)
    {
        GameObject obj = collision.gameObject;
        if (obj.CompareTag("cloth"))
        {
            if (clothObjects.Contains(obj))
            {
                Debug.Log("reeeeeeemove");
                clothObjects.Remove(obj);
            }
        }
    }*/

    private void OnTriggerExit(Collider other)
    {
        GameObject obj = other.gameObject;
        if (obj.CompareTag("cloth"))
        {
            if (clothObjects.Contains(obj))
            {
                Debug.Log("reeeeeeemove");
                clothObjects.Remove(obj);
            }
        }
    }
}
