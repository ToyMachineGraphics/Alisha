using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class AgentCollector : NetworkBehaviour
{
    public static DenryuIrairaBo DenryuIrairaBo;

    [SerializeField]
    private int _indexInCollection;

    public int PuzzleID = 0;

    [SerializeField]
    public int _agentCountRequired;

    private HashSet<DenryuIrairaBoAgent> _agentCollectionBase = new HashSet<DenryuIrairaBoAgent>();
    private float _timer;
    private bool _ready;

    private void Start()
    {
    }

    private void Update()
    {
    }

    [Server]
    private void OnTriggerEnter(Collider other)
    {
        DenryuIrairaBoAgent agent = other.GetComponent<DenryuIrairaBoAgent>();
        if (agent)
        {
            bool clear = true;
            if (Time.time - _timer < 0.5f || _timer == 0)
            {
                _agentCollectionBase.Add(agent);
                clear = false;
            }
            if (_agentCollectionBase.Count >= _agentCountRequired)
            {
                foreach (DenryuIrairaBoAgent a in _agentCollectionBase)
                {
                    RpcAgentCollectionEvent(a.gameObject);
                }
                _agentCollectionBase.Clear();
                GetComponent<Collider>().enabled = false;

                if (RobotBehavior.Instance)
                {
                    RobotBehavior.Instance.SyncCmd.Cmd_SetPuzzleUnlock(PuzzleID);
                }
                else if (Aisha.Instance)
                {
                    Aisha.Instance.SyncCmd.Cmd_SetPuzzleUnlock(PuzzleID);
                }
            }
            if (clear)
            {
                _agentCollectionBase.Clear();
                _timer = 0;
            }
        }
    }

    [ClientRpc]
    private void RpcAgentCollectionEvent(GameObject agent)
    {
        Destroy(agent.gameObject);
        GetComponent<DOTweenAnimation>().DOPlayForward();
        //DenryuIrairaBo.BroadcastAgentCollectionEvent = true;
    }

    [Server]
    private void OnTriggerExit(Collider other)
    {
        DenryuIrairaBoAgent agent = other.GetComponent<DenryuIrairaBoAgent>();
        _agentCollectionBase.Remove(agent);
    }
}