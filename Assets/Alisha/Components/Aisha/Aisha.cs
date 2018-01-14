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

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
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

    private void Start ()
    {
    }

	private void Update ()
    {
        if (localPlayerAuthority && Controller && _flashlight)
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
                    if (UI.OnOpenFlag == VRMenuUI.OnOpen.Flashlight)
                    {
                        Debug.Log("Aisha update, OnFlashlightSelected");
                        _flashlight.CmdUseFlashlight();
                        UI.OnOpenFlag = VRMenuUI.OnOpen.None;
                    }
                }
            }
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
}
