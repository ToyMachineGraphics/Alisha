using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotBehavior : MonoBehaviour
{
    public float Speed = 5;
    public float TurnSpeed = 45;
    private CharacterController _controller;
    private bool _doForward;
    private bool _doBackward;
    private bool _turnRight;
    private bool _turnLeft;

    // Use this for initialization
    private void Start()
    {
        _controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (_doForward)
        {
            _controller.SimpleMove(transform.forward * Speed);
        }
        if (_doBackward)
        {
            _controller.SimpleMove(transform.forward * -Speed);
        }
        if (_turnRight)
        {
            transform.localEulerAngles += Vector3.up * Time.deltaTime * TurnSpeed;
        }
        if (_turnLeft)
        {
            transform.localEulerAngles += Vector3.down * Time.deltaTime * TurnSpeed;
        }
    }

    public void DoForward(bool active)
    {
        _doForward = active;
    }

    public void DoBackward(bool active)
    {
        _doBackward = active;
    }

    public void TurnRight(bool active)
    {
        _turnRight = active;
    }

    public void TurnLeft(bool active)
    {
        _turnLeft = active;
    }
}