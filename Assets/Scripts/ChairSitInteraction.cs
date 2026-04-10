using UnityEngine;
using System.Collections;

public class ChairCameraSitInteraction : MonoBehaviour
{
    [Header("References")]
    public GameObject interactPrompt;
    public Transform seatCameraPoint;
    public DateDialogue dateDialogue;
    public DialogueManager dialogueManager;

    [Header("Player References")]
    public Transform cameraTransform;
    public efPlayerMovement playerMovement;
    public efPlayerLook playerLook;

    [Header("Keys")]
    public KeyCode sitKey = KeyCode.E;

    private bool playerInRange = false;
    private bool isSitting = false;
    private bool waitingForDialogueEnd = false;

    private Vector3 originalCameraWorldPosition;
    private Quaternion originalCameraWorldRotation;

    void Start()
    {
        if (interactPrompt != null)
            interactPrompt.SetActive(false);
    }

    void Update()
    {
        if (!isSitting)
        {
            if (playerInRange && Input.GetKeyDown(sitKey))
            {
                SitDown();
            }
        }
    }

    void SitDown()
    {
        if (cameraTransform == null || seatCameraPoint == null)
            return;

        isSitting = true;
        waitingForDialogueEnd = false;

        originalCameraWorldPosition = cameraTransform.position;
        originalCameraWorldRotation = cameraTransform.rotation;

        if (playerMovement != null)
            playerMovement.enabled = false;

        if (playerLook != null)
            playerLook.enabled = false;

        cameraTransform.position = seatCameraPoint.position;
        cameraTransform.rotation = seatCameraPoint.rotation;

        if (interactPrompt != null)
            interactPrompt.SetActive(false);

        if (dateDialogue != null)
        {
            dateDialogue.StartDateDialogue();

            if (!waitingForDialogueEnd)
            {
                StartCoroutine(WaitForDialogueToClose());
            }
        }
    }

    IEnumerator WaitForDialogueToClose()
    {
        waitingForDialogueEnd = true;

        // wait until dialogue actually opens
        while (dialogueManager != null && !dialogueManager.isDialogueOpen)
            yield return null;

        // wait until dialogue closes
        while (dialogueManager != null && dialogueManager.isDialogueOpen)
            yield return null;

        yield return new WaitForSeconds(0.1f);

        waitingForDialogueEnd = false;

        // ONLY leave chair if DateDialogue requested it
        if (dateDialogue != null && dateDialogue.ConsumeLeaveChairRequest())
        {
            ExitChair();
        }
    }

    void ExitChair()
    {
        if (cameraTransform == null)
            return;

        isSitting = false;

        cameraTransform.position = originalCameraWorldPosition;
        cameraTransform.rotation = originalCameraWorldRotation;

        if (playerMovement != null)
            playerMovement.enabled = true;

        if (playerLook != null)
            playerLook.enabled = true;

        if (playerInRange && interactPrompt != null)
            interactPrompt.SetActive(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        playerInRange = true;

        if (!isSitting && interactPrompt != null)
            interactPrompt.SetActive(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        playerInRange = false;

        if (!isSitting && interactPrompt != null)
            interactPrompt.SetActive(false);
    }
}