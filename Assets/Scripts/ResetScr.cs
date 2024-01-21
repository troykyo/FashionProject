using System.Collections.Generic;
using UnityEngine;

public class ResetScr : MonoBehaviour
{
    private List<GameObject> Clothsnew = new List<GameObject>();
    public GameObject parent;
    private List<GameObject> initialClothsPrefabs = new List<GameObject>();
    private List<GameObject> clothsToDestroy = new List<GameObject>();
    private List<GameObject> pinsToDestroy = new List<GameObject>();
    private Vector3 parentPositionBeforeReset;

    void Start()
    {
        // Save the initial state of cloth objects as prefabs
        foreach (Transform child in parent.GetComponentsInChildren<Transform>())
        {
            if (child.CompareTag("cloth"))
            {
                GameObject clothObject = child.gameObject;
                // Create a deep copy of the prefab and add it to the list
                GameObject prefabCopy = Instantiate(clothObject);
                prefabCopy.SetActive(false);  // Make sure it's initially inactive
                initialClothsPrefabs.Add(prefabCopy);
            }
        }

        // Save the initial position of the parent
        parentPositionBeforeReset = parent.transform.position;
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyUp(KeyCode.R))
        {
            rest();
        }
    }

    public void rest()
    {
        foreach (Transform child in parent.GetComponentsInChildren<Transform>())
        {
            if (child.CompareTag("cloth"))
            {
                // Add all the objects with this tag to a list so we can destroy them later
                Clothsnew.Add(child.gameObject);
            }
            if (child.GetComponent<vertexSnapping>() != null)
            {
                // Add all the objects with this script destroy
                child.GetComponent<vertexSnapping>().ClearPin();
            }
        }

        foreach (GameObject cloth in Clothsnew)
        {
            // Set a flag to mark the object for destruction
            clothsToDestroy.Add(cloth);
        }

        // Clear the list of cut pieces
        Clothsnew.Clear();

        // Destroy the marked objects in the next frame
        StartCoroutine(DestroyMarkedObjects());

        // Instantiate new instances based on the original prefabs
        foreach (GameObject initialClothPrefab in initialClothsPrefabs)
        {
            GameObject newCloth = Instantiate(initialClothPrefab);
            // Calculate the new position based on the difference in positions between the parent before and after the reset
            Vector3 newPosition = newCloth.transform.position + (parent.transform.position - parentPositionBeforeReset);
            newCloth.transform.position = newPosition;
            newCloth.SetActive(true);  // Make sure the new instance is set to active
            newCloth.transform.SetParent(parent.transform); // Set the parent of the new object
        }

        // Update the parentPositionBeforeReset for the next reset
        //parentPositionBeforeReset = parent.transform.position;
    }

    System.Collections.IEnumerator DestroyMarkedObjects()
    {
        // Wait for the end of the frame before destroying objects
        yield return new WaitForEndOfFrame();

        // Destroy the marked objects
        foreach (GameObject cloth in clothsToDestroy)
        {
            Destroy(cloth);
        }

        // Clear the list of marked objects
        clothsToDestroy.Clear();
    }
}
