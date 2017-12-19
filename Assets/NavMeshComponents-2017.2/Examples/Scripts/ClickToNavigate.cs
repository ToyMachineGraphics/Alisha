using System.Collections;
using UnityEngine;
using UnityEngine.AI;

// Use physics raycast hit from mouse click to set agent destination
[RequireComponent(typeof(NavMeshAgent))]
public class ClickToNavigate : MonoBehaviour
{
    private NavMeshAgent _agent;
    private RaycastHit _rayHit;

    [SerializeField]
    private string _areaName;
    private int _areaMask;
    [SerializeField]
    private float _detectLength;
    //private NavMeshHit _navHit;
    [SerializeField]
    private float _transitionSpeed;

    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _areaMask = (1 << NavMesh.GetAreaFromName(_areaName));
    }

    private int stage = 0;
    private float detectLengthScale = 0;
    [SerializeField]
    private float _lerpProcess = 0;
    Vector3 targetPosition = Vector3.zero;
    Vector3 sourcePosition = Vector3.zero;
    private void Update()
    {
        if (Input.GetMouseButton(0) && !Input.GetKey(KeyCode.LeftShift))
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray.origin, ray.direction, out _rayHit))
            {
                _agent.destination = _rayHit.point;

                stage = 0;
                detectLengthScale = 0;
                _agent.updatePosition = true;
            }
        }

        if (_agent.pathStatus == NavMeshPathStatus.PathPartial && _agent.remainingDistance < _agent.stoppingDistance/*_agent.velocity.sqrMagnitude < _stoppingVelocityMagnitude * _stoppingVelocityMagnitude*/)
        {
            Vector3 movingDir = (_rayHit.point - transform.position).normalized;
            sourcePosition = _agent.transform.position;
            Vector3 samplePosition = transform.position + movingDir * _detectLength * (++detectLengthScale);
            
            if (stage == 0 && _agent.Warp(samplePosition))
            {
                targetPosition = samplePosition;
                _agent.updatePosition = false;
                _agent.transform.position = sourcePosition;
                _lerpProcess = 0;

                stage = 1;
            }
        }
        if (stage == 1)
        {
            //if (_lerpProcess < 1)
            //{
            //    _lerpProcess += _transitionSpeed;
            //    _lerpProcess = Mathf.Clamp01(_lerpProcess);
            //    _agent.transform.position = Vector3.Lerp(sourcePosition, targetPosition, _lerpProcess);
            //}
            //else
            {
                stage = 2;
            }
        }
        if (stage == 2)
        {
            _agent.updatePosition = true;
            _agent.destination = _rayHit.point;
            stage = 0;
        }
    }
}
