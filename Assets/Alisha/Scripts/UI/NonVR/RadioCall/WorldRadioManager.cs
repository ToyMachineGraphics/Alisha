﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(RadioList))]
public class WorldRadioManager : MonoBehaviour
{
    public float MaxFrequence;
    public float MinFrequence;
    public Text FrequencceText;
    public Image TargetImage;
    public Sprite[] TargetSprite;
    public Text TargetTalk;
    public AudioSource RadioSound;
    public AudioClip TalkClip;
    public AudioClip[] RadioClips;

    private RadioList _radioList;
    private float _currentFrequence = 140;
    private List<string> imgNames = new List<string>();
    private List<string> audios = new List<string>();
    private List<string> talks = new List<string>();
    private AudioSource _talkSource;
    private bool Waiting = false;

    public float TalkDuration = 2f;
    private float _talkDuration;
    private int _sentenceAmount = 0;
    private int _sentenceID = 0;

    public float CurrentFrequence
    {
        get { return _currentFrequence; }
        set
        {
            _currentFrequence = Mathf.Clamp(value, MinFrequence, MaxFrequence);
            _currentFrequence = (float)System.Math.Round(_currentFrequence, 2);
            radioBar.Frequence = _currentFrequence;
            OnFrequenceChanged();
        }
    }

    public WorldRadioBar radioBar;

    //public static WorldRadioManager Instance = null;

    private void Awake()
    {
        //if (Instance == null)
        //    Instance = this;
        //else if (Instance != this)
        //    Destroy(gameObject);
    }

    private void Start()
    {
        _radioList = GetComponent<RadioList>();
        radioBar.CurrentMaxFreq = MaxFrequence;
        radioBar.CurrentMinFreq = MinFrequence;
        radioBar.Initialize();
        CurrentFrequence = radioBar.Frequence;
        _talkDuration = TalkDuration;
    }

    private void Update()
    {
        if (Waiting)
        {
            _talkDuration -= Time.deltaTime;
            if (_talkDuration < 0)
            {
                NextTalk();
            }
        }
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
        int isLocalPLayer = 1;
        if (RobotBehavior.Instance)
        {
            isLocalPLayer = RobotBehavior.Instance.isLocalPlayer ? 0 : 1;
        }
        if (!RadioData.instance.CheckChennal(CurrentFrequence, isLocalPLayer))
        {
            imgNames = new List<string>();
            audios = new List<string>();
            talks = new List<string>();
            imgNames.Add("雜訊");
            audios.Add("");
            talks.Add("");
        }
        else
        {
            audios = RadioData.instance.GetAduioName(CurrentFrequence, isLocalPLayer);
            imgNames = RadioData.instance.GetImageID(CurrentFrequence, isLocalPLayer);
            talks = RadioData.instance.GetTalk(CurrentFrequence, isLocalPLayer);
        }

        _sentenceAmount = talks.Count;
        RadioSound.Stop();
        RadioSound.clip = TalkClip;
        BgmManager.Instance.VolumeFadein(BgmChannels.Defult);
        _sentenceID = 0;
        NextTalk();
    }

    public void NextTalk()
    {
        Waiting = false;
        if (_sentenceID >= _sentenceAmount)
        {
            _talkDuration = TalkDuration;
            _sentenceID = 0;
            imgNames = new List<string>();
            audios = new List<string>();
            talks = new List<string>();
        }
        else
        {
            if (audios[_sentenceID] == "" && RadioSound.clip == TalkClip)
            {
                _talkSource = RadioSound;
                PlayTalkSound();
            }
            else
            {
                _talkSource = SEManager.Instance.GetSESource(SEChannels.GameEvent);
                PlayTalkSound();
                foreach (var item in RadioClips)
                {
                    if (item.name == audios[_sentenceID])
                    {
                        BgmManager.Instance.VolumeFadeout(item.length, BgmChannels.Defult);
                        RadioSound.clip = item;
                        RadioSound.Play();
                    }
                }
            }

            //set info
            foreach (var item in TargetSprite)
            {
                if (item.name == imgNames[_sentenceID])
                {
                    TargetImage.sprite = item;
                    //TargetImage.SetNativeSize();
                }
            }

            TargetTalk.DOKill();
            TargetTalk.text = "";
            TargetTalk.DOText(talks[_sentenceID], talks[_sentenceID].Length / 5)
                .SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    _talkDuration = TalkDuration;
                    Waiting = true;
                    StopTalkSound();
                });

            //talking control
            _sentenceID++;

            //RadioSound.clip = _radioList.GetClip(CurrentFrequence);
            //RadioSound.Play();
        }
    }

    private void PlayTalkSound()
    {
        _talkSource.clip = TalkClip;
        _talkSource.pitch = 1.5f;
        _talkSource.loop = true;
        _talkSource.Play();
    }

    private void StopTalkSound()
    {
        _talkSource.Stop();
        _talkSource.loop = false;
        _talkSource.pitch = 1f;
    }
}