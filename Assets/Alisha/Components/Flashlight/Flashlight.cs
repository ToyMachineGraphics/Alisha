using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Linq;

public class Flashlight : NetworkBehaviour
{
    public GameObject Model;

    [SerializeField]
    private Light _innerSpotlight;

    [SyncVar]
    public float LightLength = 1;

    [SyncVar]
    public Vector3 RayPoint;

    public SphereCollider InnerCollider;

    [SerializeField]
    private Light _outerSpotlight;

    public SphereCollider OuterCollider;

    public NetworkIdentity NetId;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        float innerRadius = LightLength * Mathf.Tan(_innerSpotlight.spotAngle * Mathf.Deg2Rad / 2.0f);
        InnerCollider.radius = innerRadius;
        InnerCollider.transform.position = _innerSpotlight.transform.position + _innerSpotlight.transform.forward * LightLength;

        float outerRadius = LightLength * Mathf.Tan(_outerSpotlight.spotAngle * Mathf.Deg2Rad / 2.0f);
        OuterCollider.radius = outerRadius;
        OuterCollider.transform.position = _outerSpotlight.transform.position + _outerSpotlight.transform.forward * LightLength;

        if (_triggerChanged)
        {
            //_triggerChanged = false;

            _diffSet = _outerLightTriggered.Except(_innerLightTriggered);
            foreach (DenryuIrairaBoAgent a in _outerLightTriggered)
            {
                if (a)
                {
                    CmdSetAgentAttracted(a.gameObject, true, RayPoint);
                }
            }
        }
    }

    [Command]
    public void CmdSetAgentAttracted(GameObject agent, bool attracted, Vector3 destination)
    {
        RpcSetAgentAttracted(agent, attracted, destination);
    }

    [ClientRpc]
    public void RpcSetAgentAttracted(GameObject agent, bool attracted, Vector3 destination)
    {
        DenryuIrairaBoAgent a = agent.GetComponent<DenryuIrairaBoAgent>();
        if (attracted)
        {
            a._destination = destination;
        }
        a.Attracted = attracted;
    }

    #region Trigger

    private HashSet<DenryuIrairaBoAgent> _outerLightTriggered = new HashSet<DenryuIrairaBoAgent>();
    private HashSet<DenryuIrairaBoAgent> _innerLightTriggered = new HashSet<DenryuIrairaBoAgent>();
    private IEnumerable<DenryuIrairaBoAgent> _diffSet;
    private bool _triggerChanged;

    public void OnOuterLightTriggerBugEnter(Collider other)
    {
        if (hasAuthority)
        {
            _triggerChanged = true;
            DenryuIrairaBoAgent agent = other.GetComponent<DenryuIrairaBoAgent>();
            _outerLightTriggered.Add(agent);
        }
    }

    public void OnOuterLightTriggerBugExit(Collider other)
    {
        if (hasAuthority)
        {
            _triggerChanged = true;
            DenryuIrairaBoAgent agent = other.GetComponent<DenryuIrairaBoAgent>();
            if (agent)
            {
                _outerLightTriggered.Remove(agent);
                CmdSetAgentAttracted(agent.gameObject, false, Vector3.zero);
            }
        }
    }

    public void OnInnerLightTriggerBugEnter(Collider other)
    {
        if (hasAuthority)
        {
            _triggerChanged = true;
            DenryuIrairaBoAgent agent = other.GetComponent<DenryuIrairaBoAgent>();
            _innerLightTriggered.Add(agent);
        }
    }

    public void OnInnerLightTriggerBugExit(Collider other)
    {
        if (hasAuthority)
        {
            _triggerChanged = true;
            DenryuIrairaBoAgent agent = other.GetComponent<DenryuIrairaBoAgent>();
            _innerLightTriggered.Remove(agent);
        }
    }

    #endregion Trigger

    [Command]
    public void CmdUseFlashlight()
    {
        RpcUseFlashlight();
    }

    [ClientRpc]
    public void RpcUseFlashlight()
    {
        gameObject.SetActive(true);
        Debug.Log("RpcUseFlashlight");
    }

    [Command]
    public void CmdUnuseFlashlight()
    {
        RpcUnuseFlashlight();
    }

    [ClientRpc]
    public void RpcUnuseFlashlight()
    {
        gameObject.SetActive(false);
        Debug.Log("RpcUnuseFlashlight");
    }
}