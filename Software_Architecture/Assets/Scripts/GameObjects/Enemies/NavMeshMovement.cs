using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Make an object move, using NavMesh
/// </summary>

public class NavMeshMovement : MonoBehaviour, IMoveBehaviour
{
    private NavMeshAgent _agent;
    private Vector3 _destination;

    private bool _moveStarted;
    private bool _destinationReached;

    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _destination = GameObject.FindWithTag("Destination").transform.position;
    }

    private void Update()
    {
        CheckDestinationReached();
    }

    private void CheckDestinationReached()
    {
        if (!_moveStarted || _destinationReached) { return; }

        if (!_agent.pathPending)
        {
            if (_agent.remainingDistance <= _agent.stoppingDistance)
            {
                if (!_agent.hasPath || _agent.velocity.magnitude == 0f)
                {
                    _destinationReached = true;
                }
            }
        }
    }

    public void Move(float speed)
    {
        _agent.isStopped = false;
        _moveStarted = true;
        _destinationReached = false;

        _agent.speed = speed;
        _agent.acceleration = speed / 2;
        _agent.SetDestination(_destination);

        // Reset velocity, to account for changing speed whilst moving
        _agent.velocity = Vector3.zero;
    }

    public void Stop()
    {
        _agent.velocity = Vector3.zero;
        _moveStarted = false;
        _agent.isStopped = true;
    }

    public bool GetDestinationReached()
    {
        return _destinationReached;
    }

    public Vector3 GetCurrentVelocity()
    {
        return _agent.velocity;
    }
}
