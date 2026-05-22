using NavMeshPlus.Components;
using UnityEngine;

public class NavMeshSurfaceManagement : MonoBehaviour
{
    public static NavMeshSurfaceManagement Instance {  get; private set; }

    private NavMeshSurface navMeshSurface;

    private void Awake()
    {
        Instance = this;
        navMeshSurface = GetComponent<NavMeshSurface>();
        navMeshSurface.hideEditorLogs = true;
    }

    public void RebakeNavMeshSurface()
    {
        navMeshSurface.BuildNavMesh();
    }
}
