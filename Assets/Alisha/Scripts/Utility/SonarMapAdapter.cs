using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class SonarMapAdapter : MonoBehaviour
{
    public Mask MapMask;
    public RectTransform Rect;

    private void OnEnable()
    {
        if (RobotBehavior.Instance.SonarMode)
        {
            Rect.DOSizeDelta(Vector2.one * 2000, 2)
                .OnComplete(() => { MapMask.enabled = false; });
        }
    }
}