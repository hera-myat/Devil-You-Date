using UnityEngine;

public class SecurityPatrol : MonoBehaviour
{
    [Header("Patrol Points")]
    public Transform[] patrolPoints;

    [Header("Movement")]
    public float patrolSpeed = 2f;
    public float rotateSpeed = 5f;
    public float reachDistance = 0.5f;

    [Header("Animator")]
    public Animator animator;
    public bool useWalkingAnimation = true;

    private int currentPoint = 0;
    private bool currentWalkingState = false;

    void Start()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        SetWalkingState(true);
    }

    void Update()
    {
        if (patrolPoints == null || patrolPoints.Length == 0)
            return;

        Patrol();
    }

    void Patrol()
    {
        Transform target = patrolPoints[currentPoint];

        Vector3 targetPos = target.position;
        targetPos.y = transform.position.y;

        Vector3 flatDirection = targetPos - transform.position;
        flatDirection.y = 0f;

        float flatDistance = flatDirection.magnitude;

        if (flatDistance <= reachDistance)
        {
            currentPoint = (currentPoint + 1) % patrolPoints.Length;
            return;
        }

        MoveToPosition(targetPos, patrolSpeed);
        SetWalkingState(true);
    }

    void MoveToPosition(Vector3 destination, float speed)
    {
        Vector3 targetPos = destination;
        targetPos.y = transform.position.y;

        Vector3 flatDirection = targetPos - transform.position;
        flatDirection.y = 0f;

        if (flatDirection.magnitude > 0.01f)
        {
            Quaternion lookRotation = Quaternion.LookRotation(flatDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, rotateSpeed * Time.deltaTime);
        }

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPos,
            speed * Time.deltaTime
        );
    }

    void SetWalkingState(bool walking)
    {
        if (!useWalkingAnimation || animator == null)
            return;

        if (currentWalkingState == walking)
            return;

        currentWalkingState = walking;
        animator.SetBool("isWalking", walking);
    }
}