using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Chapter2NonVR : MonoBehaviour
{
    [SerializeField]
    private Canvas _networkUI;
    [SerializeField]
    private Camera _camera;

    [SerializeField]
    private DenryuIrairaBo _denryuIrairaBo;

    private CirclePuzzleManager _circlePuzzleManager;
    private NetworkSyncField _networkSyncField;

    private void Awake()
    {
        _networkUI.worldCamera = _camera;
        _networkUI.gameObject.SetActive(true);
        _camera.gameObject.SetActive(true);
    }

    private IEnumerator Start()
    {
        while (!_denryuIrairaBo.gameObject.activeInHierarchy || !RobotBehavior.Instance)
        {
            yield return null;
        }

        _circlePuzzleManager = FindObjectOfType<CirclePuzzleManager>();
        _networkSyncField = FindObjectOfType<NetworkSyncField>();
        _circlePuzzleManager.GetComponent<NetworkIdentity>().AssignClientAuthority(RobotBehavior.Instance.connectionToClient);
        _networkSyncField.GetComponent<NetworkIdentity>().AssignClientAuthority(RobotBehavior.Instance.connectionToClient);

        while (true)
        {
            if (DenryuIrairaBoAgent.AgentCount < 1 && _denryuIrairaBo.isServer)
            {
                _denryuIrairaBo.SpawnAgentDefault();
                yield return new WaitForSeconds(UnityEngine.Random.Range(0.5f, 3));
            }
            else
            {
                yield return null;
            }
        }
    }

    private void Update ()
    {
		if (Input.GetKeyDown(KeyCode.Space))
        {
            if (_denryuIrairaBo.isServer)
            {
                _denryuIrairaBo.SpawnAgentDefault();
            }
        }
	}
}
