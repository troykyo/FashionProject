using UnityEngine;

public class vertexSnapTest : MonoBehaviour
{

    public GameObject lightsaber_blade;
    private Mesh saberMesh;

    void Start()
    {
        saberMesh = lightsaber_blade.GetComponent<MeshFilter>().mesh;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            Vector3 temp = GetClosestVertexTo(this.transform.position);
            Debug.Log("temp " + temp);

            this.transform.position = temp;
        }
    }

    public Vector3 GetClosestVertexTo(Vector3 point)
    {

        float minDistanceSqr = Mathf.Infinity;

        Vector3 nearestVertex = Vector3.zero;

        foreach (Vector3 vertex in saberMesh.vertices)
        {
            Vector3 diff = point - vertex;
            float distSqr = Vector3.Distance(point, vertex);

            if (distSqr < minDistanceSqr)
            {
                minDistanceSqr = distSqr;
                nearestVertex = vertex;
            }

        }

        print(nearestVertex);
        Debug.Log(minDistanceSqr);

        return lightsaber_blade.transform.TransformPoint(nearestVertex);
    }
}
