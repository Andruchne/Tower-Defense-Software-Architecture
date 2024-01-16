using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.AI;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour
{
    NavMeshAgent agent;
    Vector3 destination;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        destination = GameObject.FindWithTag("Destination").transform.position;
        agent.SetDestination(destination);
    }
}
