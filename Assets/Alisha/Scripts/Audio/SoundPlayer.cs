using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using DG.Tweening;

public class SoundPlayer : MonoBehaviour, IPointerClickHandler
{
    public bool IndependAudioSource = false;
    public bool PlayOnEnable = false;
    public bool PlayOnClick = false;
    public bool RandomPitch = false;
    public bool Loop = false;
    public float LowPitchRange = .95f;
    public float HighPitchRange = 1.05f;
    public AudioClip[] RandomSEs;

    private AudioSource FxSoundSource;

    private void OnEnable()
    {
        if (IndependAudioSource)
        {
            FxSoundSource = gameObject.AddComponent<AudioSource>();
            FxSoundSource.playOnAwake = false;
        }

        if (PlayOnEnable && RandomSEs.Length != 0)
        {
            PlayFxSound(RandomSEs);
        }
    }

    #region IPointerClickHandler implementation

    public void OnPointerClick(PointerEventData eventData)
    {
        if (PlayOnClick)
        {
            PlayFxSound(RandomSEs);
        }
    }

    #endregion IPointerClickHandler implementation

    private void OnMouseUpAsButton()
    {
        if (
#if UNITY_EDITOR
            !EventSystem.current.IsPointerOverGameObject()
#elif UNITY_IOS || UNITY_ANDROID
			! EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId)
#endif
        )
        {
            if (PlayOnClick)
            {
                PlayFxSound(RandomSEs);
            }
        }
    }

    private void PlayFxSound(AudioClip[] clips)
    {
        int randomIndex = Random.Range(0, clips.Length);

        PlayFxSound(clips[randomIndex]);
    }

    private void PlayFxSound(AudioClip clip)
    {
        if (!IndependAudioSource)
            SEManager.Instance.PlaySEClip(clip, (PlayOnClick ? SEChannels.PlayerTrigger : SEChannels.GameEvent), PlayOnClick, RandomPitch, false);
        else
        {
            if (RandomPitch)
            {
                float randomPitch = Random.Range(LowPitchRange, HighPitchRange);
                FxSoundSource.pitch = randomPitch;
            }

            FxSoundSource.time = 0;
            FxSoundSource.clip = clip;
            FxSoundSource.Stop();
            FxSoundSource.Play();
            FxSoundSource.loop = Loop;
        }
    }
}