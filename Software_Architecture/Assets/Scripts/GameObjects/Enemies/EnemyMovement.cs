using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour
{
    private NavMeshAgent _agent;
    private Vector3 _destination;

    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _destination = GameObject.FindWithTag("Destination").transform.position;
        _agent.SetDestination(_destination);
    }
}
