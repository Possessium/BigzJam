using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BJ_NavmeshGenerator : MonoBehaviour
{   

    private List<GameObject> navMeshElements = new List<GameObject>();
    public void SetNavMeshElements(List<GameObject> values)
    {
        //navMeshElements.Clear();
        navMeshElements.AddRange(values);
    }

    public void ResetList()
    {
        NavMesh.RemoveAllNavMeshData();
        navMeshElements.Clear();
    }

    private void Awake()
    {
        if (gameObject == null) { new GameObject("NavMeshRoot"); }
    }

    public void BuildNavMesh()
    {
        int agentTypeCount = NavMesh.GetSettingsCount();
        if (agentTypeCount < 1) { return; }
        for (int i = 0; i < navMeshElements.Count; ++i) 
        {
            if(navMeshElements[i])
                navMeshElements[i].transform.SetParent(gameObject.transform, true); 
        }
        for (int i = 0; i < agentTypeCount; ++i)
        {
            NavMeshBuildSettings settings = NavMesh.GetSettingsByIndex(i);
            NavMeshSurface navMeshSurface = gameObject.AddComponent<NavMeshSurface>();
            navMeshSurface.agentTypeID = settings.agentTypeID;

            NavMeshBuildSettings actualSettings = navMeshSurface.GetBuildSettings();
            navMeshSurface.useGeometry = NavMeshCollectGeometry.PhysicsColliders; // or you can use RenderMeshes

            navMeshSurface.BuildNavMesh();
        }

    }

}