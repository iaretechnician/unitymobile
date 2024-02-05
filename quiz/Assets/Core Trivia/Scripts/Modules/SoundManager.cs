using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoBehaviour
{
    internal static SoundManager instance;

    internal bool SoundEnabled;

    private AudioSource MainAudioSource;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }

        SetupAudiosource();
    }

    private void SetupAudiosource()
    {
        MainAudioSource = GetComponent<AudioSource>();
    }

    internal bool isPlaying()
    {
        return MainAudioSource.isPlaying;
    }

    internal AudioClip CurrentClip()
    {
        return MainAudioSource.clip;
    }

    internal void PlaySound(AudioClip clip, bool stop = true)
    {
        if (!SoundEnabled)
            return;

        if (MainAudioSource.isPlaying && MainAudioSource.clip == clip)
            return;

        if (stop)
            StopAudioPlayer();

        MainAudioSource.DOKill();

        MainAudioSource.volume = 1;

        MainAudioSource.clip = clip;

        MainAudioSource.Play();
    }

    internal void StopAudioPlayer()
    {
        if (MainAudioSource.isPlaying)
            MainAudioSource.DOFade(0, 1).OnComplete(() => MainAudioSource.Stop());
    }
}
