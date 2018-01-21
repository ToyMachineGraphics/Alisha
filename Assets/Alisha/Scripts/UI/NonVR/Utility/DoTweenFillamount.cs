using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using DG.Tweening;

[RequireComponent(typeof(Image))]
public class DoTweenFillamount : MonoBehaviour
{
    public bool PlayOnEnable;
    public Ease ease = Ease.OutQuad;
    public float To;
    public float duration;
    public float delay;
    public LoopType loopType = LoopType.Restart;
    public int loops = 1;
    public bool Relative;
    public UnityEvent OnCompelete;
    private Image img;

    private void OnEnable()
    {
        img = GetComponent<Image>();
        if (PlayOnEnable)
            DoFill();
    }

    // Use this for initialization
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
    }

    public void DoFill()
    {
        img.DOFillAmount(To, duration)
            .SetLoops(loops, loopType)
            .SetDelay(delay)
            .SetEase(ease)
            .SetRelative(Relative)
            .OnComplete(() => { OnCompelete.Invoke(); });
    }

    public void DoReverse(float duration)
    {
        img.DOFillAmount(0, duration)
            .SetLoops(loops, loopType)
            .SetDelay(delay)
            .SetEase(ease)
            .SetRelative(Relative)
            .OnComplete(() => { OnCompelete.Invoke(); });
    }
}