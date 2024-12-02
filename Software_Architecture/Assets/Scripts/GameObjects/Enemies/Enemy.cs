using System;
using UnityEngine;

[RequireComponent(typeof(HealthComponent))]
public class Enemy : MonoBehaviour, ITargetable
{
    [SerializeField] ParticleSystem dustParticle;

    [SerializeField] float moveSpeed = 1;

    public event Action<ITargetable> OnTargetDestroyed;
    private float _defaultMoveSpeed;
    private Timer _moveSpeedAlteredTimer;

    private IMoveBehaviour _moveBehaviour;
    private HealthComponent _healthComp;

    private void Start()
    {
        _defaultMoveSpeed = moveSpeed;
        _moveSpeedAlteredTimer = gameObject.AddComponent<Timer>();
        _moveSpeedAlteredTimer.Initialize(0);
        _moveSpeedAlteredTimer.OnTimerFinished += ResetSpeed;

        _healthComp = GetComponent<HealthComponent>();
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

        EventBus<OnPlayerDefeatedEvent>.OnEvent += Vanish;
    }

    private void OnDestroy()
    {
        _healthComp.OnDeath -= Defeated;
        _moveSpeedAlteredTimer.OnTimerFinished -= ResetSpeed;
        EventBus<OnPlayerDefeatedEvent>.OnEvent -= Vanish;
    }

    private void StartMoving()
    {
        _moveBehaviour.Move(moveSpeed);
    }

    private void Vanish(OnPlayerDefeatedEvent onPlayerDefeated)
    {
        float dustDuration = 1.0f;
        InstantiateDustParticle(dustDuration);

        Destroy(gameObject);
    }

    public void Hit(float damage)
    {
        _healthComp.Health -= damage;
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
        _moveBehaviour.Move(moveSpeed);
    }

    public void Defeated()
    {
        OnTargetDestroyed?.Invoke(this);
        Destroy(gameObject);
    }

    public Vector3 GetNextPosition(float timeInSeconds)
    {
        Vector3 currentVelocity = _moveBehaviour.GetCurrentVelocity();
        float distance = currentVelocity.magnitude * timeInSeconds;

        Vector3 predictedPosition = transform.position + currentVelocity.normalized * distance;

        return predictedPosition;
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
}
