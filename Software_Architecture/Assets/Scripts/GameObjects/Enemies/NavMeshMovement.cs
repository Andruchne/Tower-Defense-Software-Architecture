using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Make an object move, using NavMesh
/// </summary>

public class NavMeshMovement : MonoBehaviour, IMoveBehaviour
{
    [SerializeField] float rotateSpeed = 2;

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
        RotateToDirection();
    }

    private void RotateToDirection()
    {
        if (_agent.velocity.magnitude > 0.001f)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, 
                Quaternion.LookRotation(_agent.velocity), 
                Time.deltaTime * rotateSpeed);
        }
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

    public void MoveWithNoStop(float speed)
    {
        _agent.isStopped = false;
        _moveStarted = true;
        _destinationReached = false;

        _agent.speed = speed;
        _agent.acceleration = speed / 2;
        _agent.SetDestination(_destination);
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

    public Vector3 GetNextPosition(float timeInSeconds)
    {
        // Get the current path of the NavMeshAgent
        NavMeshPath path = _agent.path;

        // If the path is not ready or has no corners, return the current position
        if (path.corners.Length <= 1)
        {
            return transform.position;
        }

        // Find the closest corner on the path to the current position
        int closestCornerIndex = 0;
        float closestDistance = Vector3.Distance(transform.position, path.corners[0]);
        for (int i = 1; i < path.corners.Length; i++)
        {
            float distance = Vector3.Distance(transform.position, path.corners[i]);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestCornerIndex = i;
            }
        }

        // Calculate the distance the agent will travel in the prediction time
        float distanceToTravel = _agent.speed * timeInSeconds;

        // Find the corner that the agent is likely to reach within the prediction time
        int targetCornerIndex = closestCornerIndex;
        float accumulatedDistance = 0;
        while (targetCornerIndex < path.corners.Length - 1 && accumulatedDistance < distanceToTravel)
        {
            accumulatedDistance += Vector3.Distance(path.corners[targetCornerIndex], path.corners[targetCornerIndex + 1]);
            targetCornerIndex++;
        }

        // Calculate the predicted position along the current path segment
        Vector3 direction = (path.corners[targetCornerIndex] - path.corners[targetCornerIndex - 1]).normalized;
        float remainingDistance = distanceToTravel - (accumulatedDistance - Vector3.Distance(path.corners[targetCornerIndex - 1], path.corners[targetCornerIndex]));
        Vector3 predictedPosition = path.corners[targetCornerIndex - 1] + direction * remainingDistance;

        return predictedPosition;
    }
}
