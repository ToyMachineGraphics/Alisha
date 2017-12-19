using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

public class DenryuIrairaBo : MonoBehaviour
{
    public const string DENRYU_IRAIRA_BO_KILLER = "DenryuIrairaBoKiller";
    public const string DENRYU_IRAIRA_BO_DUPLICATE = "DenryuIrairaBoDuplicate";

    [SerializeField]
    private List<DenryuIrairaBoAgent> _agentPool = new List<DenryuIrairaBoAgent>();
    private List<DenryuIrairaBoAgent> _activeAgent = new List<DenryuIrairaBoAgent>();

    public Transform Target;
    [SerializeField]
    private Transform _destination;

    [SerializeField]
    private NavMeshSurface[] _surfaces;
    private List<Vector3[]> _surfacesVertices = new List<Vector3[]>();

    private bool _rayHover;
    public Action RaycastAction;

    private void Start()
    {
        foreach (NavMeshSurface s in _surfaces)
        {
            s.BuildNavMesh();
        }

        _rayHover = false;
        DenryuIrairaBoMask = LayerMask.GetMask("DenryuIrairaBo");
        //DenryuIrairaBoAgent agent;
        //for (int i = 0; i < 10; i++)
        //{
        //    SpawnAgent(out agent);
        //}
    }

    private void Reset()
    {

    }

    private void Update ()
    {
        if (Target == null)
        {
            return;
        }

		foreach (DenryuIrairaBoAgent a in _activeAgent)
        {
            //a.Agent.SetDestination(Target.position);
        }

        if (_rayHover && RaycastAction != null)
        {
            RaycastAction();
        }
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Space))
        {
            DenryuIrairaBoAgent agent;
            if (SpawnAgent(out agent))
            {

            }
        }
#endif
    }

    #region Event Trigger
    public void OnPointerEnter(BaseEventData data)
    {
        _rayHover = true;
        PointerEventData pointerEventData = data as PointerEventData;
        Target.position = pointerEventData.pointerCurrentRaycast.worldPosition;
    }

    public void OnPointerExit(BaseEventData data)
    {
        _rayHover = false;
    }

    #endregion

    public bool SpawnAgent(out DenryuIrairaBoAgent agent)
    {
        foreach (DenryuIrairaBoAgent a in _agentPool)
        {
            if (!a.gameObject.activeInHierarchy)
            {
                _activeAgent.Add(a);
                agent = a;
                agent.DenryuIrairaBo = this;
                agent.gameObject.SetActive(true);
                return true;
            }
        }
        agent = null;
        return false;
    }

    public bool DespawnAgent(DenryuIrairaBoAgent agent)
    {
        if (_activeAgent.Contains(agent))
        {
            _activeAgent.Remove(agent);
            agent.gameObject.SetActive(false);
            return true;
        }
        return false;
    }

    private static RaycastHit[] _raycastHitBuffer = new RaycastHit[1];
    private static Ray _ray;
    private static Vector3[] _rayDirections = new Vector3[4];
    public static int DenryuIrairaBoMask;
    public bool FindRandomDestinationOnNavMesh(NavMeshAgent agent, ref Vector3 destination)
    {
        _rayDirections[0] = agent.transform.forward;
        _rayDirections[1] = -agent.transform.up;
        _rayDirections[2] = agent.transform.right;
        _rayDirections[3] = -agent.transform.right;

        Vector3 position = agent.transform.position + UnityEngine.Random.insideUnitSphere * 1f;
        _ray.origin = position;

        int idx = UnityEngine.Random.Range(0, _rayDirections.Length);
        for (int i = 0; i < _rayDirections.Length; i++)
        {
            int j = (idx + i) % _rayDirections.Length;
            _ray.direction = _rayDirections[j];
            int count = Physics.RaycastNonAlloc(_ray, _raycastHitBuffer, 4, DenryuIrairaBoMask);
            if (count > 0)
            {
                destination = _raycastHitBuffer[0].point;
                return true;
            }
        }
        return false;
    }
}
