using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using DG.Tweening;

public class VirticalFollow : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    public RobotActionManager ActionManager;
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

    public float Range = 50;

    // Use this for initialization
    private void Start()
    {
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _initPos = transform.position;
        startRotating = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        dir = (Input.mousePosition - _initPos);
        if (dir.magnitude > Range)
            dir = dir.normalized * Range;
        transform.position = new Vector3(transform.position.x, (_initPos + dir).y);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (TouchValue.y > 0.5f)
        {
            ActionManager.Fly();
        }
        else if (TouchValue.y < -0.5f)
        {
            ActionManager.Landing();
        }
        startRotating = false;
        transform.DOMove(_initPos, 0.2f);
        dir = Vector3.zero;
    }
}