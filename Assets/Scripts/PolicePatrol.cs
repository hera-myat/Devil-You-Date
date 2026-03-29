using UnityEngine;

public class PolicePatrol : MonoBehaviour
{
    [Header("Patrol Points")]
    public Transform[] patrolPoints;

    [Header("Movement")]
    public float moveSpeed = 2f;
    public float rotateSpeed = 5f;
    public float reachDistance = 0.8f;

    [Header("Animator")]
    public Animator animator;
    public bool useWalkingAnimation = false;

    private int currentPoint = 0;
    private int direction = 1; // 1 = forward, -1 = backward

    void Update()
    {
        if (patrolPoints == null || patrolPoints.Length == 0) return;

        Transform target = patrolPoints[currentPoint];

        Vector3 targetPos = target.position;
        targetPos.y = transform.position.y;

        Vector3 flatDirection = targetPos - transform.position;
        flatDirection.y = 0f;

        float flatDistance = flatDirection.magnitude;

        if (flatDistance <= reachDistance)
        {
            // If we reach the last point, reverse direction
            if (currentPoint == patrolPoints.Length - 1)
            {
                direction = -1;
            }
            // If we reach the first point, go forward again
            else if (currentPoint == 0)
            {
                direction = 1;
            }

            currentPoint += direction;
            return;
        }

        Quaternion lookRotation = Quaternion.LookRotation(flatDirection);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, rotateSpeed * Time.deltaTime);

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPos,
            moveSpeed * Time.deltaTime
        );

        if (animator != null && useWalkingAnimation)
        {
            animator.SetBool("isWalking", true);
        }
    }

    void OnDrawGizmos()
    {
        if (patrolPoints == null || patrolPoints.Length == 0) return;

        Gizmos.color = Color.yellow;

        for (int i = 0; i < patrolPoints.Length; i++)
        {
            if (patrolPoints[i] == null) continue;
            Gizmos.DrawSphere(patrolPoints[i].position, 0.3f);
        }

        // Draw patrol path in order
        for (int i = 0; i < patrolPoints.Length - 1; i++)
        {
            if (patrolPoints[i] != null && patrolPoints[i + 1] != null)
            {
                Gizmos.DrawLine(patrolPoints[i].position, patrolPoints[i + 1].position);
            }
        }
    }
}