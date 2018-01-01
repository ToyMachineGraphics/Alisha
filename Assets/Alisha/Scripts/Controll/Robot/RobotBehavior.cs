using System.Collections;
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
    public float Speed = 5;
    public float TurnSpeed = 45;
    public GameObject[] LocalPlayerObjects;

    private bool _flying = false;
    private bool _falling = false;
    private int _flyPhase = 0;
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

        if (_falling)
        {
            _controller.Move(Physics.gravity * Time.deltaTime);
            if (_controller.isGrounded)
            {
                _falling = false;
            }
        }
        switch (_state)
        {
            case MoveState.Idel:
                break;

            case MoveState.Forward:
                if (!_controller.isGrounded)
                    _controller.Move(transform.forward * Speed * Time.deltaTime);
                else
                    _controller.SimpleMove(transform.forward * Speed);

                break;

            case MoveState.Back:
                if (!_controller.isGrounded)
                    _controller.Move(transform.forward * -Speed * Time.deltaTime);
                else
                    _controller.SimpleMove(transform.forward * -Speed);

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

    public void FlyOrLanding()
    {
        if (_flying || _falling)
            return;

        if (_flyPhase < 2)
        {
            _flying = true;
            _flyPhase++;

            transform.DOLocalMoveY(2, 1)
                .SetEase(Ease.OutBack)
                .SetRelative()
                .OnComplete(() =>
                {
                    //To update IsGrounded
                    _controller.Move(transform.forward * Speed * Time.deltaTime);
                    _flying = false;
                });
        }
        else
        {
            _falling = true;
            _flyPhase = 0;
        }
    }
}