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
        NonVR_UIManager.Instance.CostEnergy(0.5f);
    }

    public void Fly()
    {
        RobotBehavior.Instance.FlyOrLanding();
        NonVR_UIManager.Instance.CostEnergy(0.5f);
    }
}