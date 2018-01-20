using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using DG.Tweening;

public enum SEChannels
{
    PlayerTrigger,
    GameEvent,
}

public class SEManager : MonoBehaviour
{
    public AudioSource[] SESource = new AudioSource[2];

    private static SEManager _instance = null;

    public static SEManager Instance
    {
        get
        {
            if (!FindObjectOfType<SEManager>())
                _instance = ((GameObject)Instantiate(Resources.Load("SEManager"))).GetComponent<SEManager>();
            return _instance;
        }
    }

    private float LowPitchRange = .95f;
    private float HighPitchRange = 1.05f;

    private void Awake()
    {
        if (_instance == null)
            _instance = this;
        else if (_instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
        initial();
    }

    private void initial()
    {
        for (int i = 0; i < SESource.Length; i++)
        {
            SESource[i] = gameObject.AddComponent<AudioSource>();
            SESource[i].playOnAwake = false;
        }
    }

    /// <summary>
    /// Gets the SE source.
    /// </summary>
    /// <returns>The SE source.</returns>
    /// <param name="SoundChannel">Sound channel.</param>
    public AudioSource GetSESource(SEChannels SoundChannel)
    {
        int _index = (int)SoundChannel;
        return SESource[_index];
    }

    /// <summary>
    /// Play SE clips randomly
    /// </summary>
    /// <param name="clips">Random clips.</param>
    /// <param name="Channel">Channel.</param>
    public void PlaySEClip(AudioClip[] clips, SEChannels channel, bool interruptable, bool randPitch, bool loop)
    {
        int randomIndex = Random.Range(0, clips.Length);
        PlaySEClip(clips[randomIndex], channel, interruptable, randPitch, loop);
    }

    /// <summary>
    /// Play the SE clip.
    /// </summary>
    /// <param name="clip">Clip.</param>
    /// <param name="Channel">Channel.</param>
    public void PlaySEClip(AudioClip clip, SEChannels channel, bool interruptable, bool randPitch, bool loop)
    {
        int _index = (int)channel;

        if (!interruptable && SESource[_index].isPlaying)
        {
            return;
        }

        if (randPitch)
        {
            float randomPitch = Random.Range(LowPitchRange, HighPitchRange);
            SESource[_index].pitch = randomPitch;
        }

        SESource[_index].time = 0;
        SESource[_index].clip = clip;
        SESource[_index].Stop();
        SESource[_index].Play();
        SESource[_index].loop = loop;
    }
}