﻿using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Networking;

[RequireComponent(typeof(NavMeshAgent))]
public class DenryuIrairaBoAgent : NetworkBehaviour
{
    private NavMeshAgent _agent;
    public NavMeshAgent Agent
    {
        get { return _agent; }
    }
    public static DenryuIrairaBo DenryuIrairaBo;

    private Vector3 _lastPosition;
    private Vector3 _motion;

    #region Navigation
    private int stage = 0;
    //[SyncVar]
    private Vector3 _destination;

    [SerializeField]
    private float _detectLength;

    private bool _attracted;
    public bool Attracted
    {
        set
        {
            _attracted = value;
            if (!value)
            {
                _setDestinationAction = FindNextDestination;
            }
            else
            {
                _setDestinationAction = GetDestinationFromController;
            }
        }
    }
    private Action _setDestinationAction;
    #endregion

    [SerializeField]
    private GameObject _targetPrefab;
    public Transform Target;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        Target = GameObject.CreatePrimitive(PrimitiveType.Sphere).transform;
        Target.transform.SetParent(transform.parent);
        Target.transform.localScale = Vector3.one * 0.03125f;
        if (DenryuIrairaBo == null)
        {
            DenryuIrairaBo = FindObjectOfType<DenryuIrairaBo>();
        }
    }

    private void OnEnable()
    {
        _lastPosition = transform.position;
    }

    //public override void OnStartServer()
    //{
    //    base.OnStartServer();
    //    _agent.enabled = true;
    //}

    private void Start ()
    {
        _setDestinationAction = FindNextDestination;
    }

    private Coroutine _waitForMoveRoutine;
    private void FindNextDestination()
    {
        if (isServer)
        {
            stage = -1;
            if (DenryuIrairaBo.FindRandomDestinationOnNavMesh(_agent, ref _destination))
            {
                stage = 1;
                _waitForMoveRoutine = StartCoroutine(WaitForMove());
            }
        }
    }

    private IEnumerator WaitForMove()
    {
        float random = UnityEngine.Random.Range(0f, 1f);
        yield return new WaitForSeconds(random * random * 4);
        RpcSetDestination(_destination);
        //Target.position = _destination;
        //_agent.destination = _destination;
        //stage = 0;
        _waitForMoveRoutine = null;
    }

    [Server]
    public void CompleteWaitForMove()
    {
        if (_waitForMoveRoutine != null)
        {
            StopCoroutine(_waitForMoveRoutine);
            _waitForMoveRoutine = null;
        }
        RpcSetDestination(_destination);
    }

    [ClientRpc]
    private void RpcSetDestination(Vector3 destination)
    {
        _destination = destination;
        Target.position = _destination;
        _agent.destination = _destination;
        stage = 0;
    }

    private void GetDestinationFromController()
    {
        stage = -1;
        _agent.destination = DenryuIrairaBo.Target.position;

        stage = 0;
    }

    private void Update ()
    {
        _motion = transform.position - _lastPosition;
        _lastPosition = transform.position;
        if (stage == -1)
        {
            _setDestinationAction();
        }
        else if (stage == 0)
        {
            if (_agent.pathStatus == NavMeshPathStatus.PathPartial)
            {
                if (_agent.remainingDistance < _agent.stoppingDistance)
                {
                    SurfaceTransition();
                }
            }
            else if (_agent.pathStatus == NavMeshPathStatus.PathComplete)
            {
                if (_agent.remainingDistance < _agent.stoppingDistance)
                {
                    stage = -1;
                }
            }
        }
        // stage equals to 1, no action
    }

    #region Surface Transition
    private RaycastHit[] _raycastHitBuffer = new RaycastHit[1];
    private Ray[] _detectRay = new Ray[3];

    private void SurfaceTransition()
    {
        _detectRay[0].origin = transform.position;
        _detectRay[0].direction = transform.forward;
        _detectRay[1].origin = transform.position + transform.up * _detectLength * 2;
        _detectRay[1].direction = transform.forward;
        _detectRay[2].origin = transform.position + transform.forward * _detectLength * 2;
        _detectRay[2].direction = -transform.up;
        bool found = false;
        Vector3 samplePosition = Vector3.zero;
        for (int i = 0; i < _detectRay.Length; i++)
        {
            if (Physics.RaycastNonAlloc(_detectRay[i], _raycastHitBuffer, 1, DenryuIrairaBo.DenryuIrairaBoMask) > 0)
            {
                samplePosition = _raycastHitBuffer[0].point;
                UnityEngine.Object owner = _agent.navMeshOwner;
                found = _agent.Warp(samplePosition);
                if (found && _agent.navMeshOwner != owner)
                {
                    _agent.destination = _destination;
                    break;
                }
            }
        }
    }
    #endregion

    [ServerCallback]
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(DenryuIrairaBo.DENRYU_IRAIRA_BO_KILLER))
        {
            Debug.Log("Kill");
            //DenryuIrairaBo.DespawnAgent(this);
            Destroy(gameObject);
        }
        else if (other.CompareTag(DenryuIrairaBo.DENRYU_IRAIRA_BO_DUPLICATE))
        {
            Debug.Log("Duplicate");
            //DenryuIrairaBoAgent agent;
            //if (DenryuIrairaBo.SpawnAgent(out agent))
            //{
            //    agent.transform.position = transform.position;
            //}
            DenryuIrairaBo.SpawnAgent(transform.position);
        }
    }

    private void OnDestroy()
    {
        if (Target)
        {
            Destroy(Target.gameObject);
        }
    }
}