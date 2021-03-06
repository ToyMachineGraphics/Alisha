﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CirclePuzzleBehavior : MonoBehaviour
{
    public AudioClip RotateSE;

    public int CircleSplits = 4;

    [HideInInspector]
    public int CurrentSplits;

    [HideInInspector]
    public bool IsActive = false;

    private bool _rotateFinished = true;

    // Use this for initialization
    private void Awake()
    {
        Random.InitState(System.DateTime.Now.Millisecond);
        CurrentSplits = Random.Range(1, CircleSplits);
        transform.localEulerAngles += Vector3.up * (360 / CircleSplits) * CurrentSplits;
    }

    // Update is called once per frame
    private void Update()
    {
    }

    public void OnMouseUpAsButton()
    {
        TriggerRotate();
    }

    public void TriggerRotate()
    {
        if (!IsActive)
        {
            return;
        }

        SEManager.Instance.PlaySEClip(RotateSE, SEChannels.PlayerTrigger, true, false, false);
        CurrentSplits++;
        CurrentSplits = CurrentSplits == CircleSplits ? 0 : CurrentSplits;

        transform.DOLocalRotate(Vector3.up * 90, 0.2f).SetRelative(true);
    }
}