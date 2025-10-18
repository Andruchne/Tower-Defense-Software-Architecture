using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

/// <summary>
/// Extends AudioSource components
/// This component enables random clip playment, random pitch setting per play, and volume transitions
/// </summary>

public class SoundPlayer : MonoBehaviour
{
    [SerializeField] AudioClip[] sounds;

    public bool playOnAwake;

    public float volume = 1;
    public float pitch = 1;

    public bool autoVolumeTransition;
    public float transitionTime = 1.0f;

    [SerializeField] bool randomizePitch;
    [SerializeField] float randomPitchRange = 0.1f;

    private AudioSource _audioSource;
    private bool _transitionStarted;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        if (_audioSource == null) { _audioSource = gameObject.AddComponent<AudioSource>(); }

        _audioSource.volume = volume;
        _audioSource.pitch = pitch;

        AudioMixer mixer = Resources.Load<AudioMixer>("Sounds/AudioMixer");
        AudioMixerGroup[] mixGroups = mixer.FindMatchingGroups("Master");
        _audioSource.outputAudioMixerGroup = mixGroups[0];

        if (playOnAwake) { Play(); }
    }

    private void Update()
    {
        CheckTransition();
    }

    public void Play()
    {
        if (randomizePitch) { SetRandomPitch(); }

        _audioSource.volume = volume;

        // Play random sound
        AudioClip clip = sounds[Random.Range(0, sounds.Length)];
        _audioSource.clip = clip;
        _audioSource.Play();
    }

    // Added this to play multiple sounds at once with a single SoundPlayer
    // This does not work combined with transitions however, since the clip is only temp
    public void PlayOneShot()
    {
        if (randomizePitch) { SetRandomPitch(); }

        _audioSource.volume = volume;

        // Play random sound
        AudioClip clip = sounds[Random.Range(0, sounds.Length)];
        _audioSource.PlayOneShot(clip);
    }

    public void PlayTransition()
    {
        if (randomizePitch) { SetRandomPitch(); }

        AudioClip clip = sounds[Random.Range(0, sounds.Length)];
        _audioSource.clip = clip;
        _audioSource.Play();

        // Start volume transition
        StartCoroutine(FadeVolume(0, volume, transitionTime));
    }

    public void Stop()
    {
        _audioSource.Stop();
    }

    public void StopTransition()
    {
        StartCoroutine(FadeVolume(volume, 0, transitionTime));
    }

    public void ReplaceSound(AudioClip[] sounds)
    {
        this.sounds = sounds;
    }

    private void CheckTransition()
    {
        // Apply volume transition automatically, if audioClip is about to end
        if (!autoVolumeTransition || !_audioSource.isPlaying || (_audioSource.clip.length - _audioSource.time) > transitionTime) 
        {
            _transitionStarted = false;
            return; 
        }

        if (!_transitionStarted)
        {
            StopTransition();
            _transitionStarted = true;
        }
    }

    private void SetRandomPitch()
    {
        _audioSource.pitch = 1 + (Random.value * 2 - 1) * randomPitchRange;
    }

    private IEnumerator FadeVolume(float startVolume, float targetVolume, float duration)
    {
        float currentTime = 0;
        while (currentTime < duration)
        {
            float t = currentTime / duration;
            _audioSource.volume = Mathf.Lerp(startVolume, targetVolume, t);
            currentTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the volume reaches the target exactly
        _audioSource.volume = targetVolume;
        // Stop audioSource if no sound is being played
        if (targetVolume == 0) { _audioSource.Stop(); }
    }
}
