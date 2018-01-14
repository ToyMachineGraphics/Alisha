using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.Networking;

public class DenryuIrairaBo : NetworkBehaviour
{
    public const string DENRYU_IRAIRA_BO_KILLER = "DenryuIrairaBoKiller";
    public const string DENRYU_IRAIRA_BO_DUPLICATE = "DenryuIrairaBoDuplicate";

    [SerializeField]
    private Transform[] _agentSpawnPoints;

    [SerializeField]
    private DenryuIrairaBoAgent _agentPrefab;
    private Queue<Vector3> _spawnPositions = new Queue<Vector3>();
    private float _spawnTimer = 0;
    private float _spawnInterval = 0;
    //public int m_ObjectPoolSize = 50;

    //public NetworkHash128 assetId { get; set; }

    //public delegate GameObject SpawnDelegate(Vector3 position, NetworkHash128 assetId);
    //public delegate void UnSpawnDelegate(GameObject spawned);

    [SerializeField]
    private List<DenryuIrairaBoAgent> _agentPool = new List<DenryuIrairaBoAgent>();
    private List<DenryuIrairaBoAgent> _activeAgent = new List<DenryuIrairaBoAgent>();

    public Transform Target;
    [SerializeField]
    private Transform _destination;

    [SerializeField]
    private NavMeshSurface[] _surfaces;

    private bool _rayHover;
    public Action RaycastAction;

    public AgentCollector[] AgentCollectors;

    private void Awake()
    {
        //assetId = _agentPrefab.GetComponent<NetworkIdentity>().assetId;
        //_agentPool = new List<DenryuIrairaBoAgent>(m_ObjectPoolSize);
        //for (int i = 0; i < m_ObjectPoolSize; ++i)
        //{
        //    DenryuIrairaBoAgent a = Instantiate(_agentPrefab, transform);
        //    _agentPool.Add(a);
        //}
        DenryuIrairaBoAgent.AgentCount = 0;

        foreach (NavMeshSurface s in _surfaces)
        {
            s.BuildNavMesh();
            s.GetComponent<Renderer>().enabled = false;
        }
        _rayHover = false;
        DenryuIrairaBoMask = LayerMask.GetMask("DenryuIrairaBo");
        MazeAreaMask = (1 << NavMesh.GetAreaFromName("Maze"));

        AgentCollector.DenryuIrairaBo = this;
    }

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(1);
        //DenryuIrairaBoAgent agent;
        //for (int i = 0; i < 10; i++)
        //{
        //    SpawnAgent(out agent);
        //    yield return new WaitForSeconds(UnityEngine.Random.Range(0.5f, 3));
        //}
    }

    private void Reset()
    {

    }

    [Server]
    private void Update ()
    {
        _spawnTimer += Time.deltaTime;
        if (_spawnTimer > _spawnInterval && _spawnPositions.Count > 0)
        {
            _spawnTimer = 0;
            _spawnInterval = UnityEngine.Random.Range(0.016f, 0.03125f);
            Vector3 position = _spawnPositions.Dequeue();
            var agent = Instantiate(_agentPrefab, position, Quaternion.identity, transform);
            agent.gameObject.SetActive(true);
            NetworkServer.Spawn(agent.gameObject);
        }
        //if (!isServer)
        //{
        //    return;
        //}

        //if (Target == null)
        //{
        //    return;
        //}

        //foreach (DenryuIrairaBoAgent a in _activeAgent)
        //      {
        //          a.Agent.SetDestination(Target.position);
        //      }

        //        if (_rayHover && RaycastAction != null)
        //        {
        //            RaycastAction();
        //        }
        //#if UNITY_EDITOR
        //        if (Input.GetKeyDown(KeyCode.Space))
        //        {
        //            DenryuIrairaBoAgent agent;
        //            if (SpawnAgent(out agent))
        //            {

        //            }
        //        }
        //#endif
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

    [Server]
    public void SpawnAgentDefault()
    {
        int index = UnityEngine.Random.Range(0, _agentSpawnPoints.Length);
        var agent = Instantiate(_agentPrefab, _agentSpawnPoints[index].position, Quaternion.identity);
        //agent.transform.position = _agentPrefab.transform.position;
        DenryuIrairaBoAgent.DenryuIrairaBo = this;
        agent.gameObject.SetActive(true);
        NetworkServer.Spawn(agent.gameObject);
    }

    [Server]
    public void SpawnAgent(Vector3 position)
    {
        _spawnPositions.Enqueue(position);
    }

    //[Command]
    //public void CmdSpawnAgent(Vector3 position)
    //{
    //    var agent = GetFromPool(position);
    //    if (agent != null)
    //    {
    //        NetworkServer.Spawn(agent, assetId);
    //    }
    //}

    //[Command]
    //public void CmdDespawnAgent(GameObject go)
    //{
    //    UnSpawnObject(go);
    //    NetworkServer.UnSpawn(go);
    //}

    //public GameObject SpawnAgent(Vector3 position, NetworkHash128 assetId)
    //{
    //    return GetFromPool(position);
    //}

    //public void UnSpawnObject(GameObject spawned)
    //{
    //    Debug.Log("Re-pooling object " + spawned.name);
    //    spawned.SetActive(false);
    //}

    //public GameObject GetFromPool(Vector3 position)
    //{
    //    foreach (var obj in _agentPool)
    //    {
    //        if (!obj.gameObject.activeInHierarchy)
    //        {
    //            Debug.Log("Activating object " + obj.name + " at " + position);
    //            //obj.transform.position = position;
    //            obj.transform.SetParent(transform, true);
    //            obj.DenryuIrairaBo = this;
    //            obj.gameObject.SetActive(true);
    //            return obj.gameObject;
    //        }
    //    }
    //    Debug.LogError("Could not grab object from pool, nothing available");
    //    return null;
    //}

    public bool SpawnAgent(out DenryuIrairaBoAgent agent)
    {
        foreach (DenryuIrairaBoAgent a in _agentPool)
        {
            if (!a.gameObject.activeInHierarchy)
            {
                _activeAgent.Add(a);
                agent = a;
                DenryuIrairaBoAgent.DenryuIrairaBo = this;
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
    public static int MazeAreaMask;
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
