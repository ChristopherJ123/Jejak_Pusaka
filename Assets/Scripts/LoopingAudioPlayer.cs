using UnityEngine;

[RequireComponent(typeof(AudioSource))] // Ensures an AudioSource component is on this GameObject
public class LoopingAudioPlayer : MonoBehaviour
{
    // The boolean field that controls the loop.
    // You can set this to true/false from other scripts or in the Inspector.
    public bool shouldBePlaying = false;

    private AudioSource _audioSource;

    void Awake()
    {
        // Get the AudioSource component
        _audioSource = GetComponent<AudioSource>();

        // Ensure the AudioSource is set to loop
        _audioSource.loop = true;
    }

    void Update()
    {
        // This is our state manager that runs every frame
        ManageAudioState();
    }

    private void ManageAudioState()
    {
        // CASE 1: The boolean is TRUE, but the audio is NOT playing. -> START playing.
        if (shouldBePlaying && !_audioSource.isPlaying)
        {
            _audioSource.Play();
        }
        // CASE 2: The boolean is FALSE, but the audio IS playing. -> STOP playing.
        else if (!shouldBePlaying && _audioSource.isPlaying)
        {
            _audioSource.Stop();
        }
    }

    /// <summary>
    /// This is the method you asked for. It assigns a clip and starts the loop.
    /// </summary>
    /// <param name="clipToLoop">The AudioClip to play on loop.</param>
    public void PlayAudioClipLoop(AudioClip clipToLoop)
    {
        // If a valid clip is provided, assign it to the AudioSource
        if (clipToLoop)
        {
            _audioSource.clip = clipToLoop;
        }
        else
        {
            return;
        }

        // Set the boolean to true, the Update() method will handle the rest
        shouldBePlaying = true;
    }

    /// <summary>
    /// A helper method to easily stop the looping audio from another script.
    /// </summary>
    public void StopLooping()
    {
        // Set the boolean to false, the Update() method will handle stopping the playback.
        shouldBePlaying = false;
    }
}