using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class RobotActionManager : MonoBehaviour
{
    private void Start()
    {
    }

    public void StretchHand()
    {
        HandBehavior.Instance.Stretch();
    }
}