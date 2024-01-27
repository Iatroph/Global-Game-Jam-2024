using UnityEngine.AI;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField]
    private NavMeshAgent agent;

    [SerializeField]
    private GameObject player;
    private void FixedUpdate()
    {
        agent.SetDestination(player.transform.position);
    }
}
