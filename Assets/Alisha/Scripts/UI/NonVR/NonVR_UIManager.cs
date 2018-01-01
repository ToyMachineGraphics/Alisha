using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using DG.Tweening;

public class NonVR_UIManager : MonoBehaviour
{
    public Text GameTimeText;
    public RectTransform EnergyFill;

    public float EnergyPercentage
    {
        get
        {
            return
              1 - (-(EnergyFill.anchoredPosition.y - _initEnergyFillPos.y)) / EnergyFill.rect.height;
        }
    }

    private Vector2 _initEnergyFillPos;
    private float _timeAccumulate;

    public static NonVR_UIManager Instance = null;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    // Use this for initialization
    private void Start()
    {
        _initEnergyFillPos = EnergyFill.anchoredPosition;
    }

    // Update is called once per frame
    private void Update()
    {
        if (EnergyPercentage < 1)
            EnergyFill.anchoredPosition += Vector2.up * Time.deltaTime * 5;
        _timeAccumulate += Time.deltaTime;
        GameTimeText.text = (_timeAccumulate / 60).ToString("00") + ":" + (_timeAccumulate % 60).ToString("00");
    }

    public void CostEnergy(float costPercentage)
    {
        EnergyFill.DOAnchorPosY(-costPercentage * EnergyFill.rect.height, 0.5f)
            .SetRelative()
            .OnComplete(() =>
            {
                if (EnergyPercentage < 0)
                {
                    EnergyFill.anchoredPosition = new Vector2(EnergyFill.anchoredPosition.x, -EnergyFill.rect.height);
                }
            });
    }
}