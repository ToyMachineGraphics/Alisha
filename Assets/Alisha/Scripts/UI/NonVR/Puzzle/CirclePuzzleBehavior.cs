using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class CirclePuzzleBehavior : MonoBehaviour, IPointerClickHandler
{
    public int CircleSplits = 4;

    [HideInInspector]
    public int CurrentSplits;

    [HideInInspector]
    public bool IsActive = false;

    private bool _rotateFinished = true;

    // Use this for initialization
    private void Start()
    {
        Random.InitState(System.DateTime.Now.Millisecond);
        CurrentSplits = Random.Range(1, CircleSplits);
        transform.localEulerAngles = Vector3.forward * (360 / CircleSplits) * CurrentSplits;
    }

    // Update is called once per frame
    private void Update()
    {
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!IsActive)
        {
            return;
        }
        CurrentSplits++;
        CurrentSplits = CurrentSplits == CircleSplits ? 0 : CurrentSplits;

        transform.DOLocalRotate(Vector3.forward * (360 / CircleSplits) * CurrentSplits, 0.2f);
    }
}