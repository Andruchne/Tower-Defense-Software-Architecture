using System;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshMovement : MonoBehaviour, IMoveBehaviour
{
    private NavMeshAgent _agent;
    private Vector3 _destination;

    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _destination = GameObject.FindWithTag("Destination").transform.position;
    }

    public void Move(float speed)
    {
        _agent.speed = speed;
        _agent.acceleration = speed / 2;
        _agent.SetDestination(_destination);
    }

    public Vector3 GetCurrentVelocity()
    {
        return _agent.velocity;
    }
}
