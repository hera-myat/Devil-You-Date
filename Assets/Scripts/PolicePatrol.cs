using UnityEngine;

public class PolicePatrol : MonoBehaviour
{
    [Header("Patrol Points")]
    public Transform[] patrolPoints;

    [Header("Player")]
    public Transform player;

    [Header("Dialogue")]
    public DialogueManager dialogueManager;

    [Header("Movement")]
    public float patrolSpeed = 2f;
    public float chaseSpeed = 4f;
    public float rotateSpeed = 5f;
    public float reachDistance = 0.5f;

    [Header("Detection")]
    public float detectRange = 6f;
    public float talkDistance = 3f;

    [Header("Animator")]
    public Animator animator;
    public bool useWalkingAnimation = true;

    private int currentPoint = 0;
    private int direction = 1;

    private bool isChasingPlayer = false;
    private bool dialogueStarted = false;
    private bool isPausedForDialogue = false;

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
        if (player == null || patrolPoints == null || patrolPoints.Length == 0)
            return;

        if (isPausedForDialogue)
        {
            SetWalkingState(false);
            return;
        }

        Vector3 playerPos = player.position;
        playerPos.y = transform.position.y;

        float playerDistance = Vector3.Distance(transform.position, playerPos);

        if (!dialogueStarted && playerDistance <= detectRange)
        {
            isChasingPlayer = true;
        }

        if (isChasingPlayer)
        {
            MoveToPosition(playerPos, chaseSpeed);
            SetWalkingState(true);

            playerPos = player.position;
            playerPos.y = transform.position.y;
            playerDistance = Vector3.Distance(transform.position, playerPos);

            if (playerDistance <= talkDistance)
            {
                isChasingPlayer = false;
                dialogueStarted = true;
                isPausedForDialogue = true;

                SetWalkingState(false);
                StartPoliceDialogue();
            }

            return;
        }

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
            if (currentPoint == patrolPoints.Length - 1)
            {
                direction = -1;
            }
            else if (currentPoint == 0)
            {
                direction = 1;
            }

            currentPoint += direction;
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

    void StartPoliceDialogue()
    {
        if (dialogueManager == null) return;

        dialogueManager.StartSideQuestDialogue(
            "Police",
            "Hey. You shouldn't be wandering around here alone.",
            "Why?",
            "I'm just exploring.",
            "Okay, I'll leave.",
            OnPoliceChoiceSelected
        );
    }

    void OnPoliceChoiceSelected(int choice)
    {
        if (dialogueManager == null) return;

        if (choice == 1)
        {
            dialogueManager.ShowSideQuestFollowUp(
                "Police",
                "We've had people go missing around here lately.",
                "Missing?",
                "What happened to them?",
                "That sounds like rumors.",
                OnPoliceMissingFollowUp
            );
        }
        else if (choice == 2)
        {
            dialogueManager.ShowSideQuestFollowUp(
                "Police",
                "Then stay away from the abandoned areas. Most of the disappearances happened near those places.",
                "What abandoned places?",
                "I'll be careful.",
                "I can handle myself.",
                OnPoliceExploreFollowUp
            );
        }
        else if (choice == 3)
        {
            dialogueManager.ShowSideQuestSingleLine(
                "Police",
                "Smart decision. Nothing good happens around here after dark."
            );

            Invoke(nameof(ResumePatrol), 2.5f);
        }
    }

    void OnPoliceMissingFollowUp(int choice)
    {
        if (dialogueManager == null) return;

        if (choice == 1)
        {
            dialogueManager.ShowSideQuestSingleLine(
                "Police",
                "A few locals disappeared in the past couple weeks. No witnesses, no clear tracks... just gone."
            );

            Invoke(nameof(ResumePatrol), 3f);
        }
        else if (choice == 2)
        {
            dialogueManager.ShowSideQuestSingleLine(
                "Police",
                "We don't know yet. But some abandoned buildings nearby have... signs of struggle."
            );

            Invoke(nameof(ResumePatrol), 3f);
        }
        else if (choice == 3)
        {
            dialogueManager.ShowSideQuestSingleLine(
                "Police",
                "I thought so too at first. But rumors don't leave blood on the floor."
            );

            Invoke(nameof(ResumePatrol), 3f);
        }
    }

    void OnPoliceExploreFollowUp(int choice)
    {
        if (dialogueManager == null) return;

        if (choice == 1)
        {
            dialogueManager.ShowSideQuestSingleLine(
                "Police",
                "Old shops, empty houses... places people stopped going after sunset. Something about this town changes at night."
            );

            Invoke(nameof(ResumePatrol), 3.5f);
        }
        else if (choice == 2)
        {
            dialogueManager.ShowSideQuestSingleLine(
                "Police",
                "Good. Nighttime is when most of the incidents happened."
            );

            Invoke(nameof(ResumePatrol), 2.5f);
        }
        else if (choice == 3)
        {
            dialogueManager.ShowSideQuestSingleLine(
                "Police",
                "Confidence won't help much if you're alone. Just keep your distance from strangers tonight."
            );

            Invoke(nameof(ResumePatrol), 3f);
        }
    }

    void ResumePatrol()
    {
        isPausedForDialogue = false;
        SetWalkingState(true);
    }
}