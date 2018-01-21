using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class RobotActionManager : MonoBehaviour
{
    public AudioClip StretchHandClip;
    public AudioClip XrayClip;
    private bool _xrayMode = false;

    private void Start()
    {
    }

    public void StretchHand()
    {
        HandBehavior.Instance.Stretch();
        //NonVR_UIManager.Instance.CostEnergy(0.5f);
        SEManager.Instance.PlaySEClip(StretchHandClip, SEChannels.PlayerTrigger, true, false, false);
    }

    public void Fly()
    {
        NonVR_UIManager.Instance.CostEnergy(0.2f);
        RobotBehavior.Instance.Fly();
    }

    public void Landing()
    {
        RobotBehavior.Instance.Landing();
        NonVR_UIManager.Instance.CostEnergy(0.2f);
    }

    public void Xray()
    {
        _xray();
        NonVR_UIManager.Instance.CostEnergy(0.4f);
        SEManager.Instance.PlaySEClip(XrayClip, SEChannels.PlayerTrigger, true, false, false);
        Invoke("_xray", 2f);
    }

    private void _xray()
    {
        _xrayMode = !_xrayMode;
        foreach (BoxXray target in FindObjectsOfType<BoxXray>())
        {
            target.SwitchXrayMode(_xrayMode);
        }
    }

    public void Sonar()
    {
        if (RobotBehavior.Instance.SonarMode)
            return;

        RobotBehavior.Instance.SonarMode = true;
        NonVR_UIManager.Instance.CostEnergy(0.5f);
    }
}