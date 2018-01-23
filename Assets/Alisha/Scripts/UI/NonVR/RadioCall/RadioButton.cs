using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using System;

public class RadioButton : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
{
    public float AdjustUnit;
    public float PressDelay = 1f;
    private float _currentPressDelay;
    private bool _pressing = false;

    // Use this for initialization
    private void Start()
    {
        _currentPressDelay = PressDelay;
    }

    // Update is called once per frame
    private void Update()
    {
        if (_pressing)
        {
            if (_currentPressDelay > 0)
            {
                _currentPressDelay -= Time.deltaTime;
            }
            else
            {
                RadioBar.Instance.Frequence += AdjustUnit;
                RadioManager.Instance.CurrentFrequence = RadioBar.Instance.Frequence;
            }
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        RadioBar.Instance.Frequence += AdjustUnit;
        RadioManager.Instance.CurrentFrequence = RadioBar.Instance.Frequence;
        RadioManager.Instance.Call();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _pressing = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _pressing = false;
        _currentPressDelay = PressDelay;
    }
}