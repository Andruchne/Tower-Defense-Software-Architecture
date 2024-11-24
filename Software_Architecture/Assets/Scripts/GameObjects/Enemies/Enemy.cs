using System;
using UnityEngine;

[RequireComponent(typeof(HealthComponent))]
public class Enemy : MonoBehaviour, ITargetable
{
    public event Action<ITargetable> OnTargetDestroyed;

    [SerializeField] float moveSpeed = 1;
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
    }

    private void OnDestroy()
    {
        _healthComp.OnDeath -= Defeated;
        _moveSpeedAlteredTimer.OnTimerFinished -= ResetSpeed;
    }

    private void StartMoving()
    {
        _moveBehaviour.Move(moveSpeed);
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
}
