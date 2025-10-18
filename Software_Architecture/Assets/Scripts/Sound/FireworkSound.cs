using UnityEngine;

/// <summary>
/// Manages playing firework sfx fittingly
/// </summary>

public class FireworkSound : MonoBehaviour
{
    [SerializeField] AudioClip fireworkShot;
    [SerializeField] AudioClip fireworkExplode;

    private ParticleSystem _particleSystem;
    private SoundPlayer _soundPlayer;
    private int _currentParticleCount = 0;

    private void Start()
    {
        _particleSystem = GetComponent<ParticleSystem>();
        _soundPlayer = GetComponent<SoundPlayer>();
    }

    private void Update()
    {
        PlayFireworkSounds();
    }

    private void PlayFireworkSounds()
    {
        int pSystemCount = _particleSystem.particleCount;

        if (pSystemCount < _currentParticleCount) 
        {
            if (fireworkExplode != null)
            {
                AudioClip[] sounds = new AudioClip[1] { fireworkExplode };
                _soundPlayer.ReplaceSound(sounds);
                _soundPlayer.PlayOneShot();
            }
        }

        if (pSystemCount > _currentParticleCount)
        {
            if (fireworkShot != null)
            {
                AudioClip[] sounds = new AudioClip[1] { fireworkShot };
                _soundPlayer.ReplaceSound(sounds);
                _soundPlayer.PlayOneShot();
            }
        }

        _currentParticleCount = pSystemCount;
    }
}
