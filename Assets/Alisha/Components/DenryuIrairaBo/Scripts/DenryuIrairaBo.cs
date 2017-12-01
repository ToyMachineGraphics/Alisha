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
    private NavMeshSurface _surface;

    private bool _rayHover;
    public Action RaycastAction;

    private void Start()
    {
        _surface.BuildNavMesh();
        _rayHover = false;
        DenryuIrairaBoAgent agent;
        SpawnAgent(out agent);
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
            a.Agent.SetDestination(Target.position);
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
}
