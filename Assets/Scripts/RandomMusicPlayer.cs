using UnityEngine;

public class RandomMusicPlayer : MonoBehaviour
{
    public AudioClip[] musicClips;   // Assign in Inspector
    public float volume = 0.5f;
    private AudioSource _audioSource;

    void Start()
    {
        _audioSource = gameObject.AddComponent<AudioSource>();
        _audioSource.loop = false; // we don’t want a single track looping
        _audioSource.volume = volume;
        PlayRandomTrack();
    }

    void Update()
    {
        // If music finished, play another
        if (!_audioSource.isPlaying)
        {
            PlayRandomTrack();
        }
    }

    private int _lastIndex = -1;

    void PlayRandomTrack()
    {
        if (musicClips.Length == 0) return;

        int randomIndex;
        do
        {
            randomIndex = Random.Range(0, musicClips.Length);
        } while (randomIndex == _lastIndex && musicClips.Length > 1);

        _lastIndex = randomIndex;

        _audioSource.clip = musicClips[randomIndex];
        _audioSource.Play();
    }
}
