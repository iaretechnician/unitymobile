using UnityEngine;

public class GameController : Singleton<GameController> {

    [Tooltip("Is the player ship taking mouse/keyboard input to controls?")]
    public bool IsShipInputDisabled;
    private AudioSource _audioSource;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    public void PlaySound(AudioClip clip)
    {
        _audioSource.PlayOneShot(clip);
    }

}
