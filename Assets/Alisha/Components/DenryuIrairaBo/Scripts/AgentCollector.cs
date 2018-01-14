using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class AgentCollector : NetworkBehaviour
{
    public static DenryuIrairaBo DenryuIrairaBo;

    [SerializeField]
    private int _indexInCollection;
    [SerializeField]
    public int _agentCountRequired;
    private HashSet<DenryuIrairaBoAgent> _agentCollectionBase = new HashSet<DenryuIrairaBoAgent>();

    private void Start ()
    {

	}

    private void Update ()
    {

	}

    [Server]
    private void OnTriggerEnter(Collider other)
    {
        DenryuIrairaBoAgent agent = other.GetComponent<DenryuIrairaBoAgent>();
        _agentCollectionBase.Add(agent);
        if (_agentCollectionBase.Count >= _agentCountRequired)
        {
            _agentCollectionBase.Clear();
            GetComponent<Collider>().enabled = false;
            RpcAgentCollectionEvent();
        }
    }

    [ClientRpc]
    private void RpcAgentCollectionEvent()
    {
        GetComponent<Collider>().enabled = false;
        //DenryuIrairaBo.BroadcastAgentCollectionEvent = true;
    }

    [Server]
    private void OnTriggerExit(Collider other)
    {
        DenryuIrairaBoAgent agent = other.GetComponent<DenryuIrairaBoAgent>();
        _agentCollectionBase.Remove(agent);
    }
}
