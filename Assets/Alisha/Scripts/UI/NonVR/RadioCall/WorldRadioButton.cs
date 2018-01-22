using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class WorldRadioButton : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
{
    public float AdjustUnit;
    public float PressDelay = 1f;
    private float _currentPressDelay;
    private bool _pressing = false;

    public WorldRadioManager radioManager;

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
                radioManager.CurrentFrequence += AdjustUnit;
            }
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        radioManager.CurrentFrequence += AdjustUnit;
        radioManager.Call();
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
