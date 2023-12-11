using System.Collections.Generic;
using UnityEngine;

public class environmontScr : MonoBehaviour
{
    // Referenties naar game-objecten en materialen
    public GameObject parent;
    [SerializeField] private List<GameObject> parents = new List<GameObject>();
    [SerializeField] private List<GameObject> evils = new List<GameObject>();
    [SerializeField] private List<GameObject> happys = new List<GameObject>();

    public bool mode;

    public GameObject directionalLightObject;
    public GameObject bloons;
    public GameObject fog;
    private Light directionalLight;

    public Material happySkyBox;
    public Material evilSkyBox;
    [SerializeField] private Material skybox;

    [SerializeField] private int currentItem;

    // Timers voor soepele overgangen
    public float timeRemaining = 1;
    public float timeNext = 0.15f;
    public float stateOfSky = 1;

    void Start()
    {
        directionalLight = directionalLightObject.GetComponent<Light>();

        // Loop door alle 'parent'-objecten en hun kinderen
        foreach (Transform child in parent.GetComponentsInChildren<Transform>())
        {
            if (child.tag == "parent")
            {
                parents.Add(child.gameObject);

                // Loop door alle kinderen van het 'parent'-object
                foreach (Transform kid in child)
                {
                    if (kid.tag == "evil")
                        evils.Add(kid.gameObject);
                    else if (kid.tag == "happy")
                        happys.Add(kid.gameObject);
                }
            }
        }
        currentItem = evils.Count;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey("space"))
        {
            Swap();
        }
        if (Input.GetKeyUp("space"))
        {
            mode = !mode;
        }
        Mathf.Clamp(skybox.GetFloat("Weight_"), 0, 1);
    }

    public void Swap()
    {
        // Controleer of de 'mode' is ingeschakeld
        if (mode)
        {
            // Verminder de resterende tijd als deze groter is dan 0
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
            }
            // Als er nog items in de lijst zijn en currentItem binnen de grenzen is
            else if ((currentItem + 1) < evils.Count && currentItem >= 0)
            {
                // Zet de huidige 'evil' uit en de bijbehorende 'happy' aan
                evils[currentItem].SetActive(false);
                happys[currentItem].SetActive(true);
                currentItem++;
                // Reset de timer naar de ingestelde tijd voor de volgende wissel
                timeRemaining = timeNext;
            }

            // Als het huidige item de helft van de 'evils' overschrijdt, pas effecten aan
            if (currentItem > (evils.Count * 0.5))
            {
                bloons.GetComponent<ParticleSystem>().enableEmission = true;
                fog.GetComponent<ParticleSystem>().enableEmission = false;
            }

            // Pas de intensiteit van het directionele licht aan met een vloeiende overgang
            directionalLight.intensity = Mathf.Lerp(directionalLight.intensity, 1f, 0.5f * Time.deltaTime);
            
            if (stateOfSky > 0)
                stateOfSky -= 0.005f;
            if (stateOfSky < 0)
                stateOfSky = 0;
        }
        else
        {
            // Verminder de resterende tijd als deze groter is dan 0
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
            }
            // Als currentItem binnen de grenzen is
            else if (currentItem <= evils.Count && currentItem > 0)
            {
                // Verminder currentItem en zet de bijbehorende 'evil' aan en 'happy' uit
                currentItem--;
                evils[currentItem].SetActive(true);
                happys[currentItem].SetActive(false);
                // Reset de timer naar de ingestelde tijd voor de volgende wissel
                timeRemaining = timeNext;
            }

            // Als het huidige item minder is dan de helft van de 'evils', pas effecten aan
            if (currentItem < (evils.Count * 0.5))
            {
                bloons.GetComponent<ParticleSystem>().enableEmission = false;
                fog.GetComponent<ParticleSystem>().enableEmission = true;
            }

            // Pas de intensiteit van het directionele licht aan met een vloeiende overgang
            directionalLight.intensity = Mathf.Lerp(directionalLight.intensity, 0.1f, 0.5f * Time.deltaTime);

            if (stateOfSky < 0.5)
                stateOfSky += 0.005f;
        }
        skybox.SetFloat("Weight_", stateOfSky);

    }
}
