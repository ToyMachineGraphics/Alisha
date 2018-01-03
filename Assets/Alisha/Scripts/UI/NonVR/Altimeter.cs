using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Altimeter : MonoBehaviour
{
    private float _maxHeight;
    private float _initX;

    // Use this for initialization
    private void Start()
    {
        _initX = ((RectTransform)transform).anchoredPosition.x;
        _maxHeight = ((RectTransform)transform.parent).rect.height;
    }

    // Update is called once per frame
    private void Update()
    {
        ((RectTransform)transform).anchoredPosition = new Vector2(_initX, RobotBehavior.Instance.PosDelta_Y / RobotBehavior.Instance.FlyUnit * _maxHeight / RobotBehavior.Instance.FlySteps);
    }
}