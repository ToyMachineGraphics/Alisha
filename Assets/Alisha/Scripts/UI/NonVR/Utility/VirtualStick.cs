using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class VirtualStick : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    private Vector3 _initPos;
    private Vector3 dir;
    private bool startRotating;

    public Vector3 TouchValue
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
            RobotBehavior.Instance.UpdateEularRotation(new Vector3(-TouchValue.y, TouchValue.x));
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _initPos = transform.position;
        startRotating = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        dir = (Input.mousePosition - _initPos);
        if (dir.magnitude > Radius)
            dir = dir.normalized * Radius;
        transform.position = _initPos + dir;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        startRotating = false;
        transform.DOMove(_initPos, 0.2f);
        dir = Vector3.zero;
    }
}