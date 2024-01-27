using UnityEngine.AI;
using UnityEngine;
using System.Collections.Generic;
using System;
using Random = UnityEngine.Random;

public class EnemyMovement : MonoBehaviour
{
    public enum States
    {
        Hunting,
        Wandering,
        Suspitious,
        Catchup,
        Scared
    }

    [SerializeField]
    private NavMeshAgent agent;
    [SerializeField]
    private GameObject player;
    [SerializeField]
    private States actveState;
    [SerializeField]
    private List<Transform> waypoints;
    [SerializeField]
    private float distanceToTarget;

    private void Start()
    {
        actveState = States.Wandering;
        distanceToTarget = 5f;
        agent.SetDestination(getWaypoint());

    }


    private void FixedUpdate()
    {
        switch (actveState)
        {
            case States.Wandering:
                if (Vector3.Distance(transform.position, agent.destination) <= distanceToTarget)
                {
                    agent.SetDestination(getWaypoint());
                }
                break;
            case States.Hunting:
                agent.SetDestination(player.transform.position);
                break;
            case States.Catchup:
                break;
            case States.Suspitious:
                agent.SetDestination(player.transform.position);
                break;
            case States.Scared:
                break;
        }
    }

    private Vector3 getWaypoint()
    {
        return waypoints[Random.Range(0, waypoints.Count)].position; //Gets the position of a random waypoint
    }

    public void allertMe()
    {
        actveState = States.Suspitious;
    }

    public void attackPlayer()
    {
        actveState = States.Hunting;
    }

}
