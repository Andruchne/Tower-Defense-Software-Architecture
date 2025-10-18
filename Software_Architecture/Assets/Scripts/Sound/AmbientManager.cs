using UnityEngine;

/// <summary>
/// Manages ambient level sounds
/// Additionally also handles the lose sound, indicating player defeat
/// </summary>

public class AmbientManager : MonoBehaviour
{
    [SerializeField] AudioClip windSound;
    [SerializeField] AudioClip waveStartSound;
    [SerializeField] AudioClip loseSound;

    private SoundPlayer _soundPlayer;

    private void Start()
    {
        _soundPlayer = GetComponent<SoundPlayer>();
        if (_soundPlayer == null) { _soundPlayer = gameObject.AddComponent<SoundPlayer>(); }

        EventBus<OnStopBreakTime>.OnEvent += PlayWaveStartSound;
        EventBus<OnLevelFinishedEvent>.OnEvent += PlayWindSound;
        EventBus<OnPlayerDefeatedEvent>.OnEvent += PlayLoseSound;

        PlayWindSound(new OnLevelFinishedEvent());
    }

    private void OnDestroy()
    {
        EventBus<OnStopBreakTime>.OnEvent -= PlayWaveStartSound;
        EventBus<OnLevelFinishedEvent>.OnEvent -= PlayWindSound;
        EventBus<OnPlayerDefeatedEvent>.OnEvent -= PlayLoseSound;
    }

    private void PlayWindSound(OnLevelFinishedEvent onLevelFinishedEvent)
    {
        if (windSound != null)
        {
            AudioClip[] sounds = new AudioClip[1] { windSound };
            _soundPlayer.ReplaceSound(sounds);
            _soundPlayer.autoVolumeTransition = false;
            _soundPlayer.Play();
        }
    }

    private void PlayWaveStartSound(OnStopBreakTime onStopBreakTime)
    {
        if (waveStartSound != null)
        {
            AudioClip[] sounds = new AudioClip[1] { waveStartSound };
            _soundPlayer.ReplaceSound(sounds);
            _soundPlayer.autoVolumeTransition = true;
            _soundPlayer.transitionTime = 1;
            _soundPlayer.PlayTransition();
        }
    }

    private void PlayLoseSound(OnPlayerDefeatedEvent onPlayerDefeatedEvent)
    {
        if (loseSound != null)
        {
            AudioClip[] sounds = new AudioClip[1] { loseSound };
            _soundPlayer.ReplaceSound(sounds);
            _soundPlayer.autoVolumeTransition = true;
            _soundPlayer.transitionTime = 0.5f;
            _soundPlayer.PlayTransition();
        }
    }
}
