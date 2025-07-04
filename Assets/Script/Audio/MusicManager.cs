using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance { get; private set; }

    [SerializeField] private AudioClip[] musicPlaylist;
    private AudioSource audioSource;
    private int currentTrackIndex = 0;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
//        DontDestroyOnLoad(gameObject);

        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        if (musicPlaylist.Length > 0)
        {
            audioSource.clip = musicPlaylist[currentTrackIndex];
            audioSource.Play();
        }
    }

    private void Update()
    {
        if (!audioSource.isPlaying && musicPlaylist.Length > 1)
        {
            PlayNextTrack();
        }
    }

    public void PlayNextTrack()
    {
        currentTrackIndex = (currentTrackIndex + 1) % musicPlaylist.Length;
        audioSource.clip = musicPlaylist[currentTrackIndex];
        audioSource.Play();
    }

    public void StopMusic()
    {
        audioSource.Stop();
    }

    public void SetVolume(float volume)
    {
        audioSource.volume = volume;
    }
}