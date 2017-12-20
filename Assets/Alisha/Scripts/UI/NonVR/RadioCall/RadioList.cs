using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class RadioList : MonoBehaviour
{
    public float[] TargetFrequence;
    public string[] TalkingContent;
    public Sprite[] TargetSprite;
    public Sprite ConnectFailedSprite;

    // Use this for initialization
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
    }

    public string GetTalk(float frequence)
    {
        for (int i = 0; i < TargetFrequence.Length; i++)
        {
            if (frequence == TargetFrequence[i])
                return TalkingContent[i];
        }

        return "no singal";
    }

    public Sprite GetSprite(float frequence)
    {
        for (int i = 0; i < TargetFrequence.Length; i++)
        {
            if (frequence == TargetFrequence[i])
                return TargetSprite[i];
        }

        return ConnectFailedSprite;
    }
}