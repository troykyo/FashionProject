using System.Collections.Generic;
using UnityEngine;

public class environmontScr : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject parent;
    [SerializeField] List<GameObject> parents = new List<GameObject>();
    [SerializeField] List<GameObject> evils = new List<GameObject>();
    [SerializeField] List<GameObject> happys = new List<GameObject>();

    public bool mode;

    public GameObject directionalLightObject;
    private Light directionalLight;

    public Material happySkyBox;
    public Material evilSkyBox;
    [SerializeField] Material skybox;


    [SerializeField] int currentItem;

    public float timeRemaining = 1;
    public float timeNext = 0.15f;

    void Start()
    {
        directionalLight = directionalLightObject.GetComponent<Light>();
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
        currentItem = evils.Count;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp("space"))
        {
            mode = !mode;
            swap();
        }
        Mathf.Clamp(skybox.GetFloat("Weight_"), 0, 1);
    }

    public void swap()
    {
        if (mode)
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
            }
            else if ((currentItem + 1) < evils.Count && currentItem >= 0)
            {
                evils[currentItem].SetActive(false);
                happys[currentItem].SetActive(true);
                currentItem++;
                timeRemaining = timeNext;
            }
            if (currentItem > (evils.Count * 0.5))
            {
                directionalLight.intensity = 1f;
            }
            skybox.SetFloat("Weight_", Mathf.Lerp(skybox.GetFloat("Weight_"), 1, 0.2f * Time.deltaTime));
        }
        else
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
            }
            else if (currentItem <= evils.Count && currentItem > 0)
            {
                currentItem--;
                evils[currentItem].SetActive(true);
                happys[currentItem].SetActive(false);
                timeRemaining = timeNext;
            }
            if (currentItem < (evils.Count * 0.5))
            {
                directionalLight.intensity = 0.6f;
            }
            skybox.SetFloat("Weight_", Mathf.Lerp(skybox.GetFloat("Weight_"), 0, 1f * Time.deltaTime));
        }
        Debug.Log(currentItem);
        //RenderSettings.skybox = newSkybox;
    }
}
