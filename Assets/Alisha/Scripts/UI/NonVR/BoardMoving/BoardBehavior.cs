using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using System;

[RequireComponent(typeof(Board))]
public class BoardBehavior : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    private Board _boardInfo;
    private float _delta_x;
    private float _delta_y;
    private Vector2 _initPosition;

    private void Start()
    {
        _boardInfo = GetComponent<Board>();
    }

    public void OnDrag(PointerEventData eventData)
    {
#if UNITY_EDITOR
        _delta_x = _initPosition.x - Input.mousePosition.x;
        _delta_y = _initPosition.y - Input.mousePosition.y;
#elif UNITY_ANDROID
        _delta_x = _initPosition.x - Input.GetTouch(0).position.x;
        _delta_y = _initPosition.y - Input.GetTouch(0).position.y;
#endif
        if (_delta_x > 100 || _delta_x < 100)
        {
            MoveHorizontal(_delta_x);
        }

        if (_delta_y > 100 || _delta_y < 100)
        {
            MoveVirtical(_delta_y);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
#if UNITY_EDITOR
        _initPosition = Input.mousePosition;
#elif UNITY_ANDROID
        _initPosition = Input.GetTouch(0).position;
#endif
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _initPosition = Input.mousePosition;
        _delta_x = _delta_y = 0;
    }

    private void MoveHorizontal(float delta)
    {
    }

    private void MoveVirtical(float delta)
    {
    }
}