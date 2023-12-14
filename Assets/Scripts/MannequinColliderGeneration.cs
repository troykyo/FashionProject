using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MannequinColliderGeneration : MonoBehaviour
{

    public SkinnedMeshRenderer skinnedMeshRenderer;
    public MeshCollider collider;
    public void Start()
    {
        UpdateCollider();
    }

    // Update is called once per frame
    public void UpdateCollider()
    {
        Mesh colliderMesh = new Mesh();
        skinnedMeshRenderer.BakeMesh(colliderMesh);
        collider.sharedMesh = null;
        collider.sharedMesh = colliderMesh;
    }
}
