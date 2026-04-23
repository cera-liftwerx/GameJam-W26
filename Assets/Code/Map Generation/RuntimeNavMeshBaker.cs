using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class RuntimeNavMeshBaker : MonoBehaviour
{
    private NavMeshData navMeshData;
    private NavMeshDataInstance navMeshInstance;

    public void BakeNavMesh()
    {
        List<NavMeshBuildSource> sources = new List<NavMeshBuildSource>();

        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("MapGeometry"))
        {
            MeshFilter mf = obj.GetComponent<MeshFilter>();
            if (mf == null || mf.sharedMesh == null) return;

            NavMeshBuildSource source = new NavMeshBuildSource();
            source.shape     = NavMeshBuildSourceShape.Mesh;
            source.component = mf; 
            source.transform = obj.transform.localToWorldMatrix;
            source.area      = 0;

            sources.Add(source);
        }

        // Set bounds to cover your whole map
        Bounds bounds = new Bounds(Vector3.zero, new Vector3(200f, 50f, 200f));
        NavMeshBuildSettings settings = NavMesh.GetSettingsByID(0);

        // Remove old NavMesh
        NavMesh.RemoveNavMeshData(navMeshInstance);

        // Build and register new NavMesh
        navMeshData = NavMeshBuilder.BuildNavMeshData(settings, sources, bounds, Vector3.zero, Quaternion.identity);
        navMeshInstance = NavMesh.AddNavMeshData(navMeshData);

        Debug.Log("NavMesh baked!");
    }

    void OnDestroy()
    {
        NavMesh.RemoveNavMeshData(navMeshInstance);
    }
}