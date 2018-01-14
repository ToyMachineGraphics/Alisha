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
    public int FlySteps = 2;
    public int UnderSteps = 0;
    public GameObject[] LocalPlayerObjects;

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

    public static RobotBehavior Instance = null;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(gameObject);
    }

    // Use this for initialization
    private void Start()
    {
        _initPos = transform.position;
        _controller = GetComponent<CharacterController>();
        if (isLocalPlayer)
        {
            foreach (var _gameObjects in LocalPlayerObjects)
            {
                _gameObjects.SetActive(true);
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
                _controller.Move(transform.forward * Speed * Time.deltaTime);

                break;

            case MoveState.Back:
                _controller.Move(transform.forward * -Speed * Time.deltaTime);

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
    }

    public void DoForward(bool active)
    {
        if (active)
            Cmd_ChangeState(MoveState.Forward);
        else
            Cmd_ChangeState(MoveState.Idel);
    }

    public void DoBackward(bool active)
    {
        if (active)
            Cmd_ChangeState(MoveState.Back);
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
    }

    public void Landing()
    {
        if (_flying || _falling)
            return;
        if (PosDelta_Y >= FlySteps)
        {
            if (PosDelta_Y % FlyUnit == 0)
                _targetHeight = FlyUnit * ((PosDelta_Y / FlyUnit) - 1);
            else
                _targetHeight = FlyUnit * Mathf.FloorToInt(PosDelta_Y / FlyUnit);
        }
        else
            _targetHeight = 0;

        _falling = true;
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        _flying = false;
    }
}