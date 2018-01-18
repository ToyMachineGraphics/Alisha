using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class RadioBar : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    public float MaxFreq;
    public float MinFreq;
    public float UnitFreq;
    private float deltaFreq;

    public float Frequence
    {
        get
        {
            frequence = MinFreq + (Mathf.CeilToInt(-transform.localPosition.x / lenth * deltaFreq / UnitFreq) * UnitFreq);
            return frequence;
        }
        set
        {
            frequence = value;
            transform.localPosition = new Vector3(-(frequence - MinFreq) * lenth / deltaFreq, transform.localPosition.y);
        }
    }

    private float frequence;

    private Vector3 initLocalPos;
    private Vector3 lastTouchPos;
    private float lenth;

    public static RadioBar Instance = null;

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
        deltaFreq = MaxFreq - MinFreq;
        initLocalPos = transform.localPosition;
        lenth = ((RectTransform)transform).sizeDelta.x;
        Frequence = (MaxFreq + MinFreq) / 2;
    }

    // Update is called once per frame
    private void Update()
    {
        Debug.Log(Frequence);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        lastTouchPos = Input.mousePosition;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector3 delta = Input.mousePosition - lastTouchPos;
        transform.localPosition += Vector3.right * delta.x;
        if (Frequence > MaxFreq || Frequence < MinFreq)
            transform.localPosition -= Vector3.right * delta.x;
        RadioManager.Instance.CurrentFrequence = Frequence;
        RadioManager.Instance.Call();
        lastTouchPos = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Frequence = frequence;
        RadioManager.Instance.Call();
    }
}