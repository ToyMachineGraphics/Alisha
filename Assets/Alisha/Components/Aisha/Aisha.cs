using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Aisha : NetworkBehaviour
{
    public VRController Controller;
    public VRMenuUI UI;
    private NetworkIdentity _networkId;

    [SerializeField]
    private Flashlight _flashlightPrefab;

    [SerializeField]
    private Flashlight _flashlight;

    private Light _spotlight;
    public static Aisha Instance = null;

    private Ray _ray;
    private RaycastHit[] _raycastHitBuffer = new RaycastHit[1];
    private int _denryuIrairaBoMask;
    private float _flashlightUpdateInterval = 0.125f;
    private float _flashlightUpdateTimer;

    public Camera Camera;

    public SyncFieldCommand SyncCmd;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }

        _networkId = GetComponent<NetworkIdentity>();
        _denryuIrairaBoMask = LayerMask.GetMask("DenryuIrairaBo");
    }

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();

        Controller = VRController.Instance;
        CmdSpawnFlashlight();
        _flashlightUpdateTimer = 0;
        Debug.Log("Aisha OnStartLocalPlayer");
    }

    private void Start()
    {
        if (true || isLocalPlayer)
        {
            //SyncFieldCommand.Instance.OnStageClear -= GameClear;
            SyncCmd.OnStageClear += GameClear;
            WorldText.Instance.text.text = "SyncFieldCommand.Instance.OnStageClear += GameClear";
        }
    }

    private void Update()
    {
        if (localPlayerAuthority && Controller)
        {
            if (_flashlight)
            {
                if (_flashlight.gameObject.activeInHierarchy)
                {
                    _flashlight.transform.position = Controller.ControllerModel.transform.position;
                    _flashlight.transform.rotation = Controller.ControllerModel.transform.rotation;
                    _flashlightUpdateTimer += Time.deltaTime;
                    if (_flashlightUpdateTimer > _flashlightUpdateInterval)
                    {
                        _flashlightUpdateTimer = 0;
                        _ray.origin = _flashlight.transform.position;
                        _ray.direction = _flashlight.transform.forward;
                        if (_flashlight.hasAuthority && Physics.RaycastNonAlloc(_ray, _raycastHitBuffer, 8, _denryuIrairaBoMask) > 0)
                        {
                            RaycastHit hit = _raycastHitBuffer[0];
                            CmdSetFlashlightParam(hit.point, Controller.MainCamera.transform.position);
                        }
                    }
                }

                if (UI)
                {
                    if (UI.OnVRMenuUIEnable)
                    {
                        UI.OnVRMenuUIEnable = false;
                        Debug.Log("Aisha update, OnVRMenuUIEnable");
                        _flashlight.CmdUnuseFlashlight();
                    }
                    if (UI.OnOpenFlag != VRMenuUI.OnOpen.None)
                    {
                        if (UI.OnOpenFlag == VRMenuUI.OnOpen.Hierachy1Flashlight)
                        {
                            Debug.Log("Aisha update, OnFlashlightSelected");
                            _flashlight.CmdUseFlashlight();
                            UI.OnOpenFlag = VRMenuUI.OnOpen.None;
                        }
                    }
                }
            }

            if (Camera == null)
            {
                Camera = Controller.MainCamera;
            }
            Vector3 forward = Controller.MainCamera.transform.forward;
            Vector3 forwardProjected = forward - Vector3.Dot(forward, Vector3.up) * Vector3.up;
            transform.position = Controller.MainCamera.transform.position - forwardProjected * 0.5f + Vector3.down * Controller.PlayerOffset.y;
            transform.LookAt(transform.position + forwardProjected, Vector3.up);
        }
    }

    [Command]
    private void CmdSpawnFlashlight()
    {
        _flashlight = Instantiate(_flashlightPrefab);
        NetworkServer.SpawnWithClientAuthority(_flashlight.gameObject, gameObject);
        RpcSpawnFlashlight(_flashlight.gameObject);
    }

    [ClientRpc]
    private void RpcSpawnFlashlight(GameObject flashlight)
    {
        _flashlight = flashlight.GetComponent<Flashlight>();
        Controller.Flashlight = _flashlight;
        UI = Controller.VRMenuUI.GetComponent<VRMenuUI>();
        _flashlight.gameObject.SetActive(false);
    }

    [Command]
    private void CmdSetFlashlightParam(Vector3 point, Vector3 cameraPosition)
    {
        _flashlight.RayPoint = point;
        _flashlight.LightLength = Vector3.Distance(point, cameraPosition);
    }

    public void GameClear()
    {
        WorldText.Instance.text.text = "GameClear";
        Debug.Log("IN");
        foreach (var item in GameObject.FindGameObjectsWithTag("FinishGroup"))
        {
            Debug.Log(item);
            for (int i = 0; i < item.transform.childCount; i++)
            {
                item.transform.GetChild(i).gameObject.SetActive(true);
            }
            item.gameObject.SetActive(true);
        }
        foreach (var item in GameObject.FindGameObjectsWithTag("FinishTween"))
        {
            Debug.Log(item);
            item.GetComponent<DOTweenAnimation>().DOPlay();
        }
    }
}