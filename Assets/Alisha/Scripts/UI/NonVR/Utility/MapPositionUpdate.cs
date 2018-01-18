using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapPositionUpdate : MonoBehaviour
{
    private Vector3 initPos;
    public float Scaler = 0.1f;

    private void Awake()
    {
        initPos = transform.position;
    }

    private void OnEnable()
    {
        transform.position = initPos + (RobotBehavior.Instance.RelativePosition * Scaler);
    }
}