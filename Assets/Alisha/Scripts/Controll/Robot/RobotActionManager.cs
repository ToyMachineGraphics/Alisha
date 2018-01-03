using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class RobotActionManager : MonoBehaviour
{
    private bool _xrayMode = false;

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
        RobotBehavior.Instance.Fly();
        NonVR_UIManager.Instance.CostEnergy(0.5f);
    }

    public void Landing()
    {
        RobotBehavior.Instance.Landing();
        NonVR_UIManager.Instance.CostEnergy(0.5f);
    }

    public void Xray()
    {
        _xrayMode = !_xrayMode;
        foreach (BoxXray target in FindObjectsOfType<BoxXray>())
        {
            target.SwitchXrayMode(_xrayMode);
        }
        NonVR_UIManager.Instance.CostEnergy(0.5f);
    }

    public void Sonar()
    {
        if (RobotBehavior.Instance.SonarMode)
            return;

        RobotBehavior.Instance.SonarMode = true;
        NonVR_UIManager.Instance.CostEnergy(0.5f);
    }
}