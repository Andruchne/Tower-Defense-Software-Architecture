using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(HealthComponent))]
public class Enemy : MonoBehaviour, ITargetable
{
    public event Action<ITargetable> OnTargetDestroyed;

    [SerializeField] float moveSpeed = 1;

    private IMoveBehaviour _moveBehaviour;
    private HealthComponent _healthComp;

    private void Start()
    {
        _healthComp = GetComponent<HealthComponent>();

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

    private void StartMoving()
    {
        _moveBehaviour.Move(moveSpeed);
    }

    public void Hit(float damage)
    {
        _healthComp.Health -= damage;
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
