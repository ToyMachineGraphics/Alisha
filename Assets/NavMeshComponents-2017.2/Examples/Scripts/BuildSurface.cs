using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshSurface))]
public class BuildSurface : MonoBehaviour
{
    private NavMeshSurface _surface;

	private void Awake ()
    {
        _surface = GetComponent<NavMeshSurface>();
        _surface.BuildNavMesh();
        GetComponent<Renderer>().enabled = false;
    }
}
