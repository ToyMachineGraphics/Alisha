using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class RadioList : MonoBehaviour
{
    public float[] TargetFrequence;
    public string[] TalkingContent;
    public Sprite[] TargetSprite;
    public AudioClip[] TargetClip;
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

        return "沒有訊號。";
    }

    public AudioClip GetClip(float frequence)
    {
        for (int i = 0; i < TargetFrequence.Length; i++)
        {
            if (frequence == TargetFrequence[i])
            {
                BgmManager.Instance.VolumeFadeout(TargetClip[i].length, BgmChannels.Defult);
                return TargetClip[i];
            }
        }

        BgmManager.Instance.VolumeFadein(BgmChannels.Defult);
        return null;
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