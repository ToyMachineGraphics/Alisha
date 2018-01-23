using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class VirtualStick : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    private Vector2 _initPos;
    private Vector2 dir;
    private bool startRotating;
    private int fingerID;

    public Vector2 TouchValue
    {
        get
        {
            return dir.normalized;
        }
    }

    public float Radius = 50;

    // Use this for initialization
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
        if (startRotating)
            RobotBehavior.Instance.UpdateEularRotation(new Vector2(-TouchValue.y, TouchValue.x));
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _initPos = transform.position;
        startRotating = true;
        fingerID = Input.touchCount - 1;
    }

    public void OnDrag(PointerEventData eventData)
    {
#if UNITY_EDITOR

        dir = (new Vector2(Input.mousePosition.x, Input.mousePosition.y) - _initPos);
#else
        if (fingerID < 0)
            return;
        dir = (Input.GetTouch(fingerID).position - _initPos);
#endif

        if (dir.magnitude > Radius)
            dir = dir.normalized * Radius;
        transform.position = _initPos + dir;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        startRotating = false;
        transform.DOMove(_initPos, 0.2f);
        dir = Vector2.zero;
    }
}