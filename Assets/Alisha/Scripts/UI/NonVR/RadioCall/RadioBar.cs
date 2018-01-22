using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class RadioBar : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    public Transform Bar;
    public AudioClip AdjustClip;
    public float MaxFreq;
    public float MinFreq;
    public float UnitFreq;
    private float deltaFreq;

    public float Frequence
    {
        get
        {
            frequence = MinFreq + (Mathf.CeilToInt(-Bar.localPosition.x / lenth * deltaFreq / UnitFreq) * UnitFreq);
            return frequence;
        }
        set
        {
            frequence = value;
            Bar.localPosition = new Vector3(-(frequence - MinFreq) * lenth / deltaFreq, Bar.localPosition.y);
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
        initLocalPos = Bar.localPosition;
        lenth = ((RectTransform)Bar).sizeDelta.x;
        Frequence = (MaxFreq + MinFreq) / 2;
        RadioManager.Instance.CurrentFrequence = Frequence;
    }

    // Update is called once per frame
    private void Update()
    {
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        lastTouchPos = Input.mousePosition;
        SEManager.Instance.GetSESource(SEChannels.PlayerTrigger).volume = 0.5f;
    }

    public void OnDrag(PointerEventData eventData)
    {
        float lastFreq = Frequence;
        Vector3 delta = Input.mousePosition - lastTouchPos;
        Bar.localPosition += Vector3.right * delta.x;
        if (Frequence > MaxFreq || Frequence < MinFreq)
            Bar.localPosition -= Vector3.right * delta.x;
        if ((lastFreq - Frequence) != 0)
        {
            SEManager.Instance.PlaySEClip(AdjustClip, SEChannels.PlayerTrigger, false, false, false);
            RadioManager.Instance.CurrentFrequence = Frequence;
            RadioManager.Instance.Call();
        }
        lastTouchPos = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        SEManager.Instance.GetSESource(SEChannels.PlayerTrigger).volume = 1f;
        Frequence = frequence;
        RadioManager.Instance.Call();
    }
}