using UnityEngine;

public class efPlayerMovement : MonoBehaviour
{
    public CharacterController controller;
    public Animator animator;

    [Header("Movement")]
    public float walkSpeed = 6f;
    public float runSpeed = 12f;
    public float gravity = -9.81f;
    public float jumpHeight = 3f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundDistance = 0.8f;
    public LayerMask groundMask;

    [Header("Sprint Stamina")]
    public float maxSprintTime = 5f;
    public float restTime = 3f;

    [Header("Breathing Sound")]
    public AudioSource breathingAudioSource;
    public AudioClip heavyBreathingClip;

    private float currentSprintTime;
    private float restTimer = 0f;

    private bool isGrounded;
    private bool isResting = false;

    private Vector3 velocity;

    void Start()
    {
        currentSprintTime = maxSprintTime;

        if (breathingAudioSource != null)
        {
            breathingAudioSource.playOnAwake = false;
            breathingAudioSource.loop = false;
        }
    }

    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        bool isMoving = x != 0 || z != 0;
        bool wantsToRun = Input.GetKey(KeyCode.LeftShift);

        if (isResting)
        {
            restTimer -= Time.deltaTime;

            if (restTimer <= 0f)
            {
                isResting = false;
                currentSprintTime = maxSprintTime;
                StopHeavyBreathing();
            }
        }

        bool canRun = !isResting && currentSprintTime > 0f;
        bool isRunning = isMoving && wantsToRun && canRun;

        if (isRunning)
        {
            currentSprintTime -= Time.deltaTime;

            if (currentSprintTime <= 0f)
            {
                currentSprintTime = 0f;
                isResting = true;
                restTimer = restTime;
                PlayHeavyBreathing();
            }
        }

        float currentSpeed = isRunning ? runSpeed : walkSpeed;

        Vector3 move = transform.right * x + transform.forward * z;

        if (move.magnitude > 1f)
        {
            move = move.normalized;
        }

        controller.Move(move * currentSpeed * Time.deltaTime);

        if (animator != null)
        {
            animator.SetBool("isWalking", isMoving);
            animator.SetBool("isRunning", isRunning);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Space pressed, grounded = " + isGrounded);
        }

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Debug.Log("Jump triggered");
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    void PlayHeavyBreathing()
    {
        if (breathingAudioSource == null || heavyBreathingClip == null)
            return;

        if (breathingAudioSource.isPlaying)
            return;

        breathingAudioSource.clip = heavyBreathingClip;
        breathingAudioSource.loop = true;
        breathingAudioSource.Play();
    }

    void StopHeavyBreathing()
    {
        if (breathingAudioSource == null)
            return;

        if (breathingAudioSource.isPlaying)
            breathingAudioSource.Stop();

        breathingAudioSource.loop = false;
        breathingAudioSource.clip = null;
    }
}