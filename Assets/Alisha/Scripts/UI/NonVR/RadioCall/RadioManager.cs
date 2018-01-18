using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(RadioList))]
public class RadioManager : MonoBehaviour
{
    public float MaxFrequence;
    public float MinFrequence;
    public Text FrequencceText;
    public Image TargetImage;
    public Text TargetTalk;
    public AudioSource RadioSound;

    private RadioList _radioList;
    private float _currentFrequence = 140;

    public float CurrentFrequence
    {
        get { return _currentFrequence; }
        set
        {
            _currentFrequence = Mathf.Clamp(value, MinFrequence, MaxFrequence);
            _currentFrequence = (float)System.Math.Round(_currentFrequence, 2);
            RadioBar.Instance.Frequence = _currentFrequence;
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

    private void Start()
    {
        _radioList = GetComponent<RadioList>();
    }

    private void OnEnable()
    {
        GameObject.FindGameObjectWithTag("RadioCam").GetComponent<Camera>().enabled = true;
    }

    private void OnFrequenceChanged()
    {
        UpdateFreqText();
    }

    private void UpdateFreqText()
    {
        FrequencceText.text = CurrentFrequence.ToString("000.00");
    }

    public void Call()
    {
        RadioSound.Stop();
        TargetImage.sprite = _radioList.GetSprite(CurrentFrequence);
        TargetTalk.DOKill();
        TargetTalk.text = "";
        TargetTalk.DOText(_radioList.GetTalk(CurrentFrequence), 1)
            .SetEase(Ease.Linear);
        RadioSound.clip = _radioList.GetClip(CurrentFrequence);
        RadioSound.Play();
    }

    private void OnDisable()
    {
        GameObject.FindGameObjectWithTag("RadioCam").GetComponent<Camera>().enabled = false;
    }
}