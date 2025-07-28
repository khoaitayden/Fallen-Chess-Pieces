using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }
    [Header("Audio Mixer")]
    [SerializeField] private AudioMixer mainMixer;
    [Header("Audio Clips")]
    [SerializeField] private AudioClip[] moveSounds;
    [SerializeField] private AudioClip captureSound;
    [SerializeField] private AudioClip checkSound;
    [SerializeField] private AudioClip castleSound;
    [SerializeField] private AudioClip promotionSound;
    [SerializeField] private AudioClip gameWinSound;
    [SerializeField] private AudioClip gameLoseSound;
    [SerializeField] private AudioClip gameDrawSound;
    private AudioSource audioSource;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

        audioSource = GetComponent<AudioSource>();
    }

    public void PlayCaptureSound() => PlaySound(captureSound);
    public void PlayCheckSound() => PlaySound(checkSound, 0.7f);
    public void PlayCastleSound() => PlaySound(castleSound);
    public void PlayPromotionSound() => PlaySound(promotionSound);

    public void PlayGameEndSound(GameState finalState)
    {
        switch (finalState)
        {
            case GameState.Checkmate:
            case GameState.Timeout:
                PlaySound(gameWinSound);
                break;
            case GameState.Stalemate:
            case GameState.Draw:
                PlaySound(gameDrawSound);
                break;
        }
    }
    public void PlayMoveSound()
    {
        if (moveSounds != null && moveSounds.Length > 0)
        {
            AudioClip randomMoveClip = moveSounds[Random.Range(0, moveSounds.Length)];
            PlaySound(randomMoveClip);
        }
    }
    private void PlaySound(AudioClip clip, float volumeScale = 1.0f)
    {
        if (clip != null)
        {
            audioSource.PlayOneShot(clip, volumeScale);
        }
    }
    public void SetMasterVolume(float level)
    {
        mainMixer.SetFloat("MasterVolume", Mathf.Log10(level) * 20);
    }

    public void SetSFXVolume(float level)
    {
        mainMixer.SetFloat("SFXVolume", Mathf.Log10(level) * 20);
    }

    public void SetMusicVolume(float level)
    {
        mainMixer.SetFloat("MusicVolume", Mathf.Log10(level) * 20);
    }
}