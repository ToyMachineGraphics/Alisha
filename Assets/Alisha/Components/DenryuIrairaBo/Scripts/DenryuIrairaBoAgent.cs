using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class DenryuIrairaBoAgent : MonoBehaviour
{
    private NavMeshAgent _agent;
    public NavMeshAgent Agent
    {
        get { return _agent; }
    }
    public DenryuIrairaBo DenryuIrairaBo;

    private Vector3 _lastPosition;
    private Vector3 _motion;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
    }

    private void OnEnable()
    {
        _lastPosition = transform.position;
    }

    private void Start ()
    {
        
    }

    private void Update ()
    {
        _motion = transform.position - _lastPosition;
        _lastPosition = transform.position;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(DenryuIrairaBo.DENRYU_IRAIRA_BO_KILLER))
        {
            Debug.Log("Kill");
            DenryuIrairaBo.DespawnAgent(this);
        }
        else if (other.CompareTag(DenryuIrairaBo.DENRYU_IRAIRA_BO_DUPLICATE))
        {
            Debug.Log("Duplicate");
            DenryuIrairaBoAgent agent;
            if (DenryuIrairaBo.SpawnAgent(out agent))
            {
                agent.transform.position = transform.position;
            }
        }
    }
}
