using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallScr : MonoBehaviour
{
    public Vector3 returnPos;
    public GameObject boem;
    public bool hit;
    public float resettimer = 4;
    private float resetFrames;

    // Start is called before the first frame update
    void Start()
    {
        returnPos = this.transform.position;
        resetFrames = resettimer * 60;
    }

    // Update is called once per frame
    void Update()
    {
        if(this.transform.position.y <= 0)
        {
            Return();
        }
    }
    private void FixedUpdate()
    {
        if (hit)
        {
            resetFrames--;
            if (resetFrames < 60)
            {
                boem.SetActive(false);
            }
            if (resetFrames < 0)
            {
                Return();
            }
        }
    }

    public void Return()
    {
        this.transform.position = returnPos;
        //boem.SetActive(false);
        this.GetComponent<Rigidbody>().velocity = Vector3.zero;
        hit = false;
        resetFrames = resettimer * 60;
    }

    public void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Finish") == true)
        {
            //boem.SetActive(true);
            //GameObject.Find("ScoreBoard").GetComponent<ScoreBoardScr>().Score++;
            hit = true;
        }
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Respawn") == true)
        {
            hit = true;
        }
    }
}
