using System.Collections.Generic;
using UnityEngine;

public class environmontScr : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject parent;
    [SerializeField] List<GameObject> parents = new List<GameObject>();
    [SerializeField] List<GameObject> evils = new List<GameObject>();
    [SerializeField] List<GameObject> happys = new List<GameObject>();

    [SerializeField] bool mode;

    void Start()
    {
        foreach (Transform child in parent.transform)
        {
            if (child.tag == "parent")
            {
                parents.Add(child.gameObject);
                foreach (Transform kid in child.transform)
                {
                    if (kid.tag == "evil")
                        evils.Add(kid.gameObject);
                }

                foreach (Transform kid in child.transform)
                {
                    if (kid.tag == "happy")
                        happys.Add(kid.gameObject);
                }
            }
        }

        foreach (Transform child in parent.transform)
        {
            if (child.tag == "parent")
            {
                parents.Add(child.gameObject);
                foreach (Transform middeChild in child.transform)
                {
                    if (middeChild.tag == "parent")
                    {
                        parents.Add(middeChild.gameObject);
                        foreach (Transform kid in middeChild.transform)
                        {
                            if (kid.tag == "evil")
                                evils.Add(kid.gameObject);
                        }

                        foreach (Transform kid in middeChild.transform)
                        {
                            if (kid.tag == "happy")
                                happys.Add(kid.gameObject);
                        }
                    }
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp("space"))
        {
            swap();
        }
    }

    private void swap()
    {
        mode = !mode;
        if (mode)
        {
            foreach (GameObject evil in evils)
            {
                evil.SetActive(false);
            }
            foreach (GameObject happy in happys)
            {
                happy.SetActive(true);
            }
        }
        else
        {
            foreach (GameObject evil in evils)
            {
                evil.SetActive(true);
            }
            foreach (GameObject happy in happys)
            {
                happy.SetActive(false);
            }
        }
    }
}
