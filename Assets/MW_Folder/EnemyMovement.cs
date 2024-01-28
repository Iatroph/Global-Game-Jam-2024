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
    private float[] EnemySpeed = {4.5f, 5, 6, 10};
    [SerializeField]
    private float
        timer,
        CatchUpRange,
        CatchUpBuffer,
        distanceToTarget, //How far the enemy has to be from the target to lose agro
        susDistance, // How far the enemy has to be from the susPoint to lose suspition
        susRad; // The radius of the circle that will have a random point for the enemy to search
    [SerializeField]
    private Vector3 susPoint;

    bool kindaHunting;

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
                if (Vector3.Distance(transform.position, agent.destination) <= distanceToTarget) //moves to random waypoint if clost to another waypoint
                {
                    agent.SetDestination(getWaypoint());
                    alertMe(transform.position);
                    timer = 0f;
                }
                break;
            case States.Hunting:
                if(Vector3.Distance(transform.position, agent.destination) <= distanceToTarget && timer >= 4f)
                {
                    actveState = States.Suspitious;
                    timer = 0;
                }
                else
                {
                    timer += Time.deltaTime;
                }
                break;
            case States.Catchup:
                if (Vector3.Distance(transform.position, agent.destination) <= susDistance && !kindaHunting)
                {
                    actveState = States.Wandering;
                    agent.SetDestination(getWaypoint());
                }

                    break;
            case States.Suspitious:
                if (Vector3.Distance(transform.position, agent.destination) <= susDistance && !kindaHunting)
                {
                    actveState = States.Wandering;
                    agent.SetDestination(getWaypoint());
                }
                break;
            case States.Scared:
                break;
        }

        if (farAwayFromPlayer() && !(actveState == States.Catchup))
        {
            actveState = States.Catchup;
            catchUp();
        }

        if(actveState == States.Catchup)
        {
            agent.speed = EnemySpeed[3];
        }
        else if (actveState == States.Hunting)
        {
            agent.speed = EnemySpeed[2];
        }
        else if (actveState == States.Hunting)
        {
            agent.speed = EnemySpeed[1];
        }
        else if (agent.speed != EnemySpeed[0])
        {
            agent.speed = EnemySpeed[0];
        }
    }

    private Vector3 getWaypoint()
    {
        return waypoints[Random.Range(0, waypoints.Count)].position; //Gets the position of a random waypoint
    }

    private Vector3 getSusPoint(Vector3 target)
    {
        //Debug.Log("tar" + target);
        susPoint = target + new Vector3(Random.Range(-susRad, susRad), 0, Random.Range(-susRad, susRad));
        //Debug.Log("point" + susPoint);
        return susPoint;
    }

    private void setSusPoint(Vector3 target)
    {
        susPoint = target;
    }

    public void alertMe(Vector3 target)
    {
        actveState = States.Suspitious;
        do
        {
            //Debug.Log("uuuhhh");
            Vector3 newTarget = getSusPoint(target);
            agent.SetDestination(newTarget);
            setSusPoint(newTarget);
        } while (!agent.CalculatePath(agent.destination, agent.path));
    }

    public void attackPlayer()
    {
        actveState = States.Hunting;
        agent.SetDestination(player.transform.position);
    }

    public void attackDecoy(Vector3 target)
    {
        actveState = States.Hunting;
        agent.SetDestination(target);
    }

    public bool farAwayFromPlayer()
    {
        return (Vector3.Distance(player.transform.position, transform.position) >= 30);
    }

    public void catchUp()
    {
        Vector3 target = player.transform.position;
        do
        {
             target += new Vector3(Random.Range(-CatchUpRange, CatchUpRange) * CatchUpBuffer, 0, Random.Range(-CatchUpRange, CatchUpRange) *  CatchUpBuffer);
            agent.SetDestination(target);
        } while (!agent.CalculatePath(agent.destination, agent.path));
    }
}
