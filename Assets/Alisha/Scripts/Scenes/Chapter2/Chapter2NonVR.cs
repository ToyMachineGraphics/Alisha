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

    public ALishaNetworkMain NetworkMain;
    public ALishaNetworkManager ALishaNetworkManager;
    public Camera NetworkUICamera;

    private void Awake()
    {
        //_networkUI.worldCamera = _camera;
        _networkUI.gameObject.SetActive(true);
        //_camera.gameObject.SetActive(true);
    }

    private IEnumerator Start()
    {
        ALishaNetworkManager.OnClientConnectAction = () =>
        {
            NetworkMain.gameObject.SetActive(false);
            NetworkUICamera.gameObject.SetActive(false);
            _networkUI.gameObject.SetActive(false);
        };

        while (!_denryuIrairaBo.gameObject.activeInHierarchy || !RobotBehavior.Instance || !NetworkSyncField.Instance)
        {
            yield return null;
        }

        _circlePuzzleManager = FindObjectOfType<CirclePuzzleManager>();
        _networkSyncField = NetworkSyncField.Instance;
        NetworkIdentity net = _circlePuzzleManager.GetComponent<NetworkIdentity>();
        _circlePuzzleManager.GetComponent<NetworkIdentity>().AssignClientAuthority(RobotBehavior.Instance.connectionToClient);
        //_networkSyncField.GetComponent<NetworkIdentity>().AssignClientAuthority(RobotBehavior.Instance.connectionToClient);

        //while (true)
        //{
        //    if (DenryuIrairaBoAgent.AgentCount < 50 && _denryuIrairaBo.isServer)
        //    {
        //        _denryuIrairaBo.SpawnAgentDefault();
        //        yield return new WaitForSeconds(UnityEngine.Random.Range(0.5f, 3));
        //    }
        //    else
        //    {
        //        yield return null;
        //    }
        //}
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //if (_denryuIrairaBo.isServer)
            //{
            //    _denryuIrairaBo.SpawnAgentDefault();
            //}
        }
    }
}