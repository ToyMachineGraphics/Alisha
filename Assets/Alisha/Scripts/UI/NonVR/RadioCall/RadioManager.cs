using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class RadioManager : MonoBehaviour
{
    public float MaxFrequence;
    public float MinFrequence;
    public Text FrequencceText;

    private float _currentFrequence = 140;

    public float CurrentFrequence
    {
        get { return _currentFrequence; }
        set
        {
            _currentFrequence = Mathf.Clamp(value, MinFrequence, MaxFrequence);
            OnFrequenceChanged();
        }
    }

    public static RadioManager Instance = null;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(gameObject);
    }

    private void OnFrequenceChanged()
    {
        UpdateFreqText();
    }

    private void UpdateFreqText()
    {
        FrequencceText.text = CurrentFrequence.ToString("000.00");
    }
}