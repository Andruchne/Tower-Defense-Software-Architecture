using System;
using UnityEngine;

/// <summary>
/// Enemy script, used for any type of enemy
/// </summary>

[RequireComponent(typeof(HealthComponent))]
public class Enemy : MonoBehaviour, ITargetable
{
    [SerializeField] GameObject goldGainedCanvas;
    [SerializeField] ParticleSystem dustParticle;

    [SerializeField] float power = 1;
    [SerializeField] float moveSpeed = 1;
    [SerializeField] int rewardAmount = 100;

    [SerializeField] AudioClip attackSound;
    [SerializeField] AudioClip hitSound;
    [SerializeField] AudioClip deathSound;

    public event Action<ITargetable> OnTargetDestroyed;
    private float _defaultMoveSpeed;
    private Timer _moveSpeedAlteredTimer;

    private IMoveBehaviour _moveBehaviour;
    private HealthComponent _healthComp;

    private Animator _anim;
    private bool _destinationReached;

    private SoundPlayer _soundPlayer;

    private void Start()
    {
        _soundPlayer = GetComponent<SoundPlayer>();
        if (_soundPlayer == null) { _soundPlayer = gameObject.AddComponent<SoundPlayer>(); }
        _anim = GetComponent<Animator>();

        if (_anim == null)
        {
            Debug.LogError("Enemy: No animator attached. Destroying enemy...");
            Destroy(gameObject);
            return;
        }

        _defaultMoveSpeed = moveSpeed;
        _moveSpeedAlteredTimer = gameObject.AddComponent<Timer>();
        _moveSpeedAlteredTimer.Initialize(3);
        _moveSpeedAlteredTimer.OnTimerFinished += ResetSpeed;

        _healthComp = GetComponent<HealthComponent>();

        if (_healthComp == null)
        {
            Debug.LogError("Enemy: No HealthComponent attached. Destroying enemy...");
            Destroy(gameObject);
            return;
        }

        _healthComp.OnDeath += Defeated;

        // Get move behaviour and do null check
        _moveBehaviour = GetComponent<IMoveBehaviour>();
        if (_moveBehaviour == null)
        {
            Debug.LogError("Enemy: No valid IMoveBehaviour script found. Destroying enemy...");
            Destroy(gameObject);
            return;
        }
        // To make sure that the move behaviour was able to get all necessary components
        Invoke("StartMoving", 0.1f);

        EventBus<OnPlayerDefeatedEvent>.OnEvent += TriggerVanish;
    }

    private void OnDestroy()
    {
        if (_healthComp != null) { _healthComp.OnDeath -= Defeated; }
        if (_moveSpeedAlteredTimer != null) { _moveSpeedAlteredTimer.OnTimerFinished -= ResetSpeed; }
        EventBus<OnPlayerDefeatedEvent>.OnEvent -= TriggerVanish;
    }

    private void Update()
    {
        DestinationReached();
    }

    private void DestinationReached()
    {
        if (!_destinationReached && _moveBehaviour.GetDestinationReached())
        {
            PlayAttackSound();

            OnTargetDestroyed?.Invoke(this);
            _healthComp.RemoveUI();

            _destinationReached = true;
            _anim.SetTrigger("Attack");
        }
    }

    private void StartMoving()
    {
        _moveBehaviour.Move(moveSpeed);
    }

    private void TriggerVanish(OnPlayerDefeatedEvent onPlayerDefeated)
    {
        OnTargetDestroyed?.Invoke(this);
        Vanish();
    }

    private void Vanish()
    {
        float dustDuration = 1.0f;
        InstantiateDustParticle(dustDuration);

        Destroy(gameObject);
    }

    public void Hit(float damage)
    {
        PlayHitSound();

        if (_healthComp.Health <= 0) { return; }

        if (!GameManager.Instance.GetOneShotTargets()) { _healthComp.Health -= damage; }
        else { _healthComp.Health = 0; }

        _anim.SetTrigger("Hit");
    }

    public void MultiplySpeed(float speedFactor, float duration)
    {
        moveSpeed = _defaultMoveSpeed * speedFactor;
        _moveBehaviour.Move(moveSpeed);

        _moveSpeedAlteredTimer.SetWaitTime(duration);
        _moveSpeedAlteredTimer.ResetTimer(true);
    }

    public void ResetSpeed()
    {
        moveSpeed = _defaultMoveSpeed;
        _moveBehaviour.MoveWithNoStop(moveSpeed);
    }

    public void Defeated()
    {
        PlayDeathSound();

        _healthComp.RemoveUI();
        OnTargetDestroyed?.Invoke(this);
        _anim.SetTrigger("Die");
        _moveBehaviour.Stop();
    }

    public Vector3 GetNextPosition(float timeInSeconds)
    {
        return _moveBehaviour.GetNextPosition(timeInSeconds);
    }

    private void InstantiateDustParticle(float duration)
    {
        if (dustParticle != null)
        {
            ParticleSystem[] particleSystems = dustParticle.GetComponentsInChildren<ParticleSystem>();

            // Set the duration for all particle systems
            foreach (ParticleSystem particleSys in particleSystems)
            {
                ParticleSystem.MainModule mainModule = particleSys.main;
                mainModule.duration = duration;
            }

            Vector3 particlePos = new Vector3(transform.position.x, transform.position.y + Useful.GetRenderedHeight(transform), transform.position.z);
            Instantiate(dustParticle, particlePos, Quaternion.Euler(-90.0f, 0.0f, 0.0f));
        }
        else { Debug.LogError("Tower Prefab: No dustParticle to instantiate"); }
    }

    private void PlayHitSound()
    {
        if (hitSound != null)
        {
            AudioClip[] sounds = new AudioClip[1] { hitSound };
            _soundPlayer.ReplaceSound(sounds);
            _soundPlayer.Play();
        }
    }

    private void PlayDeathSound()
    {
        if (deathSound != null)
        {
            AudioClip[] sounds = new AudioClip[1] { deathSound };
            _soundPlayer.ReplaceSound(sounds);
            _soundPlayer.Play();
        }
    }

    private void PlayAttackSound()
    {
        if (attackSound != null)
        {
            AudioClip[] sounds = new AudioClip[1] { attackSound };
            _soundPlayer.ReplaceSound(sounds);
            _soundPlayer.Play();
        }
    }

    // Triggered by animation event
    public void Death()
    {
        // Instantiate floating gold indicator
        GoldMover gold = Instantiate(goldGainedCanvas, transform.position, Quaternion.identity)
            .GetComponent<GoldMover>();
        
        gold.Initialize(transform.position, rewardAmount);

        EventBus<OnGetGoldEvent>.Publish(new OnGetGoldEvent(rewardAmount));

        Vanish();
    }

    public void DamagePlayer()
    {
        EventBus<OnDamagePlayerEvent>.Publish(new OnDamagePlayerEvent(power));
        Vanish();
    }

    // For unit test purposes
    public void DeactivateAttack()
    {
        _destinationReached = true;
    }

    public float GetCurrentSpeed()
    {
        return moveSpeed;
    }
}
