﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;
using DG.Tweening;

public enum MoveState
{
    Idel,
    Forward,
    Back,
    TurnRight,
    TurnLeft
}

public class RobotBehavior : NetworkBehaviour
{
    public bool SonarMode = false;
    public float Speed = 1;
    public float FlySpeed = 0.5f;
    public float FallDrag = 0.2f;
    public float TurnSpeed = 45;
    public float FlyUnit = 1.5f;
    public float MaxUpAngle = 70;
    public float MaxDownAngle = 40;
    public int FlySteps = 2;
    public int UnderSteps = 0;
    public GameObject[] LocalPlayerObjects;
    public AudioClip ForwardClip;
    public AudioClip FlyClip;
    public AudioClip LandingClip;
    public SyncFieldCommand SyncCmd;

    public Vector3 RelativePosition
    {
        get
        {
            return transform.position - _initPos;
        }
    }

    private bool _flying = false;
    private bool _floaing = false;
    private bool _falling = false;
    private float _targetHeight = 0;
    private Vector3 _initPos;

    public float PosDelta_Y
    {
        get { return transform.position.y - _initPos.y; }
    }

    private CharacterController _controller;

    [SyncVar]
    private MoveState _state = MoveState.Idel;

    public Flashlight FlashlightPrefab;
    public Flashlight FlashlightInstance;
    private Ray _ray;
    private RaycastHit[] _raycastHitBuffer = new RaycastHit[1];
    private int _denryuIrairaBoMask;
    private float _flashlightUpdateInterval = 0.125f;
    private float _flashlightUpdateTimer;

    public static RobotBehavior Instance = null;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(gameObject);
    }

    public override void OnStartLocalPlayer()
    {
        StartCoroutine(SpawnBugs());
    }

    private IEnumerator SpawnBugs()
    {
        while (true)
        {
            if (!DenryuIrairaBoAgent.DenryuIrairaBo || DenryuIrairaBoAgent.AgentCount >= 50)
            {
                yield return null;
            }
            else if (DenryuIrairaBoAgent.AgentCount < 50)
            {
                yield return new WaitForSeconds(Random.Range(0.25f, 1.25f));
                DenryuIrairaBoAgent.DenryuIrairaBo.SpawnAgent();
            }
        }
    }

    // Use this for initialization
    private IEnumerator Start()
    {
        _initPos = transform.position;
        _controller = GetComponent<CharacterController>();
        if (isLocalPlayer)
        {
            foreach (var _gameObjects in LocalPlayerObjects)
            {
                _gameObjects.SetActive(true);
            }
            SyncCmd.OnStageClear += GameClear;
        }
        //CmdSpawnFlashlight ();

        _denryuIrairaBoMask = LayerMask.GetMask("DenryuIrairaBo");
        while (isLocalPlayer)
        {
            if (!DenryuIrairaBoAgent.DenryuIrairaBo || DenryuIrairaBoAgent.AgentCount >= 50)
            {
                yield return null;
            }
            else if (DenryuIrairaBoAgent.AgentCount < 50)
            {
                yield return new WaitForSeconds(Random.Range(0.25f, 1.25f));
                DenryuIrairaBoAgent.DenryuIrairaBo.SpawnAgent();
            }
        }
    }

    // Update is called once per frame
    private void Update()
    {
        if (!isLocalPlayer)
            return;

        if (_flying)
        {
            if (PosDelta_Y < _targetHeight)
            {
                float smooth = Mathf.Clamp(((_targetHeight - PosDelta_Y) / FlyUnit), 0.1f, 1);
                _controller.Move(-Physics.gravity * FlySpeed * Time.deltaTime * smooth);
            }
            else
            {
                transform.position = new Vector3(transform.position.x, _initPos.y + _targetHeight, transform.position.z);
                _flying = false;
            }
        }
        if (_falling)
        {
            if (PosDelta_Y > _targetHeight)
                _controller.Move(Physics.gravity * FallDrag * Time.deltaTime);
            else
            {
                transform.position = new Vector3(transform.position.x, _initPos.y + _targetHeight, transform.position.z);
                _falling = false;
            }
        }
        switch (_state)
        {
            case MoveState.Idel:
                break;

            case MoveState.Forward:
                _controller.Move(new Vector3(transform.forward.x, 0, transform.forward.z) * Speed * Time.deltaTime);

                break;

            case MoveState.Back:
                _controller.Move(new Vector3(transform.forward.x, 0, transform.forward.z) * -Speed * Time.deltaTime);

                break;

            case MoveState.TurnRight:
                transform.localEulerAngles += Vector3.up * Time.deltaTime * TurnSpeed;

                break;

            case MoveState.TurnLeft:
                transform.localEulerAngles += Vector3.down * Time.deltaTime * TurnSpeed;

                break;

            default:
                break;
        }

        if (_controller.isGrounded)
        {
            _floaing = _falling = false;
        }
        else
        {
            if (!_floaing)
                _controller.Move(Physics.gravity * FallDrag * Time.deltaTime);
        }

        if (FlashlightInstance && FlashlightInstance.gameObject.activeInHierarchy)
        {
            FlashlightInstance.transform.position = transform.position;
            FlashlightInstance.transform.rotation = transform.rotation;
            _flashlightUpdateTimer += Time.deltaTime;
            if (_flashlightUpdateTimer > _flashlightUpdateInterval)
            {
                _flashlightUpdateTimer = 0;
                _ray.origin = FlashlightInstance.transform.position;
                _ray.direction = FlashlightInstance.transform.forward;
                if (FlashlightInstance.hasAuthority && Physics.RaycastNonAlloc(_ray, _raycastHitBuffer, 64, _denryuIrairaBoMask) > 0)
                {
                    RaycastHit hit = _raycastHitBuffer[0];
                    CmdSetFlashlightParam(hit.point, transform.position);
                }
            }
        }
    }

    [Command]
    private void CmdSetFlashlightParam(Vector3 point, Vector3 cameraPosition)
    {
        FlashlightInstance.RayPoint = point;
        FlashlightInstance.LightLength = Vector3.Distance(point, cameraPosition);
    }

    public void UpdateEularRotation(Vector3 delta)
    {
        transform.localEulerAngles += Time.deltaTime * TurnSpeed * delta.x * Vector3.right;
        transform.localEulerAngles += Time.deltaTime * TurnSpeed * delta.y * Vector3.up;

        if (180 < transform.localEulerAngles.x && transform.localEulerAngles.x < 360 - MaxUpAngle)
            transform.localEulerAngles = new Vector3(360 - MaxUpAngle, transform.localEulerAngles.y);
        else if (transform.localEulerAngles.x > MaxDownAngle && transform.localEulerAngles.x < 180)
            transform.localEulerAngles = new Vector3(MaxDownAngle, transform.localEulerAngles.y);
    }

    public void DoForward(bool active)
    {
        if (active)
        {
            Cmd_ChangeState(MoveState.Forward);
            SEManager.Instance.PlaySEClip(ForwardClip, SEChannels.PlayerTrigger, true, false, false);
        }
        else
            Cmd_ChangeState(MoveState.Idel);
    }

    public void DoBackward(bool active)
    {
        if (active)
        {
            SEManager.Instance.PlaySEClip(ForwardClip, SEChannels.PlayerTrigger, true, false, false);
            Cmd_ChangeState(MoveState.Back);
        }
        else
            Cmd_ChangeState(MoveState.Idel);
    }

    public void TurnRight(bool active)
    {
        if (active)
            Cmd_ChangeState(MoveState.TurnRight);
        else
            Cmd_ChangeState(MoveState.Idel);
    }

    public void TurnLeft(bool active)
    {
        if (active)
            Cmd_ChangeState(MoveState.TurnLeft);
        else
            Cmd_ChangeState(MoveState.Idel);
    }

    private void ChangeState(MoveState state)
    {
        if (isServer)
            _state = state;
        else
            Cmd_ChangeState(state);
    }

    [Command]
    private void Cmd_ChangeState(MoveState state)
    {
        _state = state;
    }

    public void Fly()
    {
        if (_flying || _falling)
            return;

        if (PosDelta_Y % FlyUnit == 0)
            _targetHeight = FlyUnit * ((PosDelta_Y / FlyUnit) + 1);
        else
            _targetHeight = FlyUnit * Mathf.CeilToInt(PosDelta_Y / FlyUnit);

        _targetHeight = Mathf.Clamp(_targetHeight, -(UnderSteps * FlyUnit), FlySteps * FlyUnit);
        _floaing = _flying = true;
        SEManager.Instance.PlaySEClip(FlyClip, SEChannels.PlayerTrigger, true, false, false);
    }

    public void Landing()
    {
        if (_flying || _falling)
            return;

        if (PosDelta_Y % FlyUnit == 0)
            _targetHeight = FlyUnit * ((PosDelta_Y / FlyUnit) - 1);
        else
            _targetHeight = FlyUnit * Mathf.FloorToInt(PosDelta_Y / FlyUnit);

        _falling = true;
        SEManager.Instance.PlaySEClip(LandingClip, SEChannels.PlayerTrigger, true, false, false);
    }

    private void GameClear()
    {
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

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        _flying = false;
    }

    [Command]
    private void CmdSpawnFlashlight()
    {
        FlashlightInstance = Instantiate(FlashlightPrefab);
        NetworkServer.SpawnWithClientAuthority(FlashlightInstance.gameObject, gameObject);
        RpcSpawnFlashlight(FlashlightInstance.gameObject);
    }

    [ClientRpc]
    private void RpcSpawnFlashlight(GameObject flashlight)
    {
        FlashlightInstance = flashlight.GetComponent<Flashlight>();
    }
}