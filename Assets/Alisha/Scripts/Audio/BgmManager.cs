using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System;

public enum BgmChannels
{
    Defult,
    Second,
}

public class BgmManager : MonoBehaviour
{
    /// <summary>
    /// The initial bgms plays on Manager Start.
    /// </summary>
    public AudioClip InitialBgms;

    [SerializeField]
    private AudioSource[] BgmSources = new AudioSource[2];

    private bool[] _fadeState = new bool[2] { false, false };
    private float _fadingTime;
    private float _originalVolme;

    private Action _onFadeoutFinished;

    public static BgmManager Instance = null;

    // SoundManager won't be destroy
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
        {
            Debug.Log("there is second BgmManager instance");
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        for (int i = 0; i < BgmSources.Length; i++)
        {
            BgmSources[i] = gameObject.AddComponent<AudioSource>();
            BgmSources[i].playOnAwake = false;
            BgmSources[i].loop = true;
            if (i == 1)
            {
                BgmSources[i].volume = 0.2f;
            }
        }
        if (InitialBgms)
            PlayBgm(InitialBgms, BgmChannels.Defult);
    }

    private void Update()
    {
        for (int i = 0; i < BgmSources.Length; i++)
        {
            if (_fadeState[i])
            {
                _fadingTime -= Time.deltaTime;
                BgmSources[i].volume -= Time.deltaTime;
                if (_fadingTime < 0)
                {
                    BgmSources[i].volume = _originalVolme;
                    _fadeState[i] = false;
                    if (_onFadeoutFinished != null)
                    {
                        _onFadeoutFinished();
                        _onFadeoutFinished = null;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Gets the bgm source.
    /// </summary>
    /// <returns>The bgm source.</returns>
    /// <param name="SoundChannel">Sound channel.</param>
    public AudioSource GetBgmSource(BgmChannels SoundChannel)
    {
        int _index = (int)SoundChannel;
        return BgmSources[_index];
    }

    /// <summary>
    /// Fadeout volume to 0 in FadeTime of Target SoundChannel, then stop the clip, return original Volumn
    /// </summary>
    /// <param name="FadeTime"></param>
	/// <param name="SoundChannel">Target Channel</param>
	public void VolumeFadeout(float FadeTime, BgmChannels SoundChannel)
    {
        int _index = (int)SoundChannel;
        if (_fadeState[_index])
            return;
        _fadingTime = FadeTime;
        _originalVolme = BgmSources[_index].volume;
        _fadeState[_index] = true;
        _onFadeoutFinished = new Action(() => { PlayBgm(BgmSources[_index].clip, SoundChannel); });
    }

    public void VolumeFadein(BgmChannels SoundChannel)
    {
        int _index = (int)SoundChannel;
        if (!_fadeState[_index])
            return;

        BgmSources[_index].volume = _originalVolme;
        _fadeState[_index] = false;
        if (_onFadeoutFinished != null)
        {
            _onFadeoutFinished();
        }
    }

    /// <summary>
    /// Play BGM Music
    /// </summary>
	/// <param name="clip">The bgm clip</param>
	/// <param name="SoundChannel">Target Channel</param>
	public void PlayBgm(AudioClip clip, BgmChannels SoundChannel)
    {
        int _index = (int)SoundChannel;

        if (BgmSources[_index].clip != null)
        {
            if (BgmSources[_index].clip.name != clip.name || !BgmSources[_index].isPlaying)
            {
                BgmSources[_index].clip = clip;
                BgmSources[_index].Play();
            }
        }
        else
        {
            BgmSources[_index].clip = clip;
            BgmSources[_index].Play();
        }
    }
}