using UnityEngine;
using UnityEngine.AI;

public class GhostAI : MonoBehaviour
{
    public enum GhostState { Patrol, Chase, Return }
    public GhostState currentState = GhostState.Patrol;

    public Transform[] patrolPoints;
    public int startingPatrolIdx = 0;
    public float pointReachedDistance = 0.5f;

    public Transform player;
    public float detectRadius = 6f;
    public float loseRadius = 9f;

    private NavMeshAgent agent;
    private int currentPatrolIdx;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        currentPatrolIdx = startingPatrolIdx;

        if (agent.isOnNavMesh && patrolPoints.Length > 0)
        {
            agent.SetDestination(patrolPoints[currentPatrolIdx].position);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!agent.isOnNavMesh) return;

        float distToPlayer = Vector3.Distance(transform.position, player.position);

        switch (currentState)
        {
            case GhostState.Patrol:
                PatrolBehavior();

                if (distToPlayer <= detectRadius)
                {
                    currentState = GhostState.Chase;
                }
                break;
            
            case GhostState.Chase:
                agent.SetDestination(player.position);

                if (distToPlayer > loseRadius)
                {
                    currentState = GhostState.Return;
                    agent.SetDestination(GetClosestWaypoint().position);
                }
                break;

            case GhostState.Return:
                if (!agent.pathPending && agent.remainingDistance <= pointReachedDistance)
                {
                    currentState = GhostState.Patrol;
                }
                break;
        }
    }

    void PatrolBehavior()
    {
        if (!agent.pathPending && agent.remainingDistance <= pointReachedDistance)
        {
            currentPatrolIdx = (currentPatrolIdx + 1) % patrolPoints.Length;
            agent.SetDestination(patrolPoints[currentPatrolIdx].position);
        }
    }

    Transform GetClosestWaypoint()
    {
        Transform closest = patrolPoints[0];
        float minDist = Vector3.Distance(transform.position, patrolPoints[0].position);

        foreach (Transform wp in patrolPoints)
        {
            float dist = Vector3.Distance(transform.position, wp.position);

            if (dist < minDist)
            {
                minDist = dist;
                closest = wp;
            }
        }

        return closest;
    }

}
