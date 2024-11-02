using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, ITargetable
{
    [SerializeField] float moveSpeed = 1;

    private IMoveBehaviour moveBehaviour;

    private void Start()
    {
        // Get move behaviour and do null check
        moveBehaviour = GetComponent<IMoveBehaviour>();
        if (moveBehaviour == null)
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
        moveBehaviour.Move(moveSpeed);
    }

    public void Hit()
    {

    }

    public void Destroyed()
    {

    }

    public Vector3 GetNextPosition(float timeInSeconds)
    {
        Vector3 currentVelocity = moveBehaviour.GetCurrentVelocity();
        float distance = currentVelocity.magnitude * timeInSeconds;

        Vector3 predictedPosition = transform.position + currentVelocity.normalized * distance;

        return predictedPosition;
    }
}
