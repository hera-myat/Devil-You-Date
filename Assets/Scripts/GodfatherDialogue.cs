using UnityEngine;
using System.Collections;

public class GodfatherDialogue : MonoBehaviour
{
    [Header("References")]
    public DialogueManager dialogueManager;
    public GodfatherRepentSequence repentSequence;
    public MoralStateManager moralStateManager;
    public GameObject interactPrompt;

    private bool playerInRange = false;
    private bool hasShownPostRepentLine = false;

    void Start()
    {
        if (interactPrompt != null)
            interactPrompt.SetActive(false);
    }

    void Update()
    {
        if (!playerInRange)
            return;

        if (dialogueManager != null && dialogueManager.isDialogueOpen)
            return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            StartGodfatherDialogue();
        }
    }

    void StartGodfatherDialogue()
    {
        if (dialogueManager == null)
            return;

        if (interactPrompt != null)
            interactPrompt.SetActive(false);

        // After full repent sequence
        if (repentSequence != null && repentSequence.repentCompleted)
        {
            if (!hasShownPostRepentLine)
            {
                hasShownPostRepentLine = true;
                dialogueManager.ShowSideQuestSingleLine(
                    "Godfather",
                    "He knows your decision."
                );
            }
            else
            {
                dialogueManager.ShowSideQuestSingleLine(
                    "Godfather",
                    "He's always here."
                );
            }

            StartCoroutine(WaitForDialogueToCloseThenShowPrompt());
            return;
        }

        // If player already touched bloody area, block repentance options
        if (moralStateManager != null && moralStateManager.hasTouchedBloodyArea)
        {
            dialogueManager.StartSideQuestDialogue(
                "Godfather",
                "There is nothing for you here now.",
                "You don't need it.",
                "",
                "",
                OnBlockedChoiceSelected
            );

            return;
        }

        // Normal dialogue
        dialogueManager.StartSideQuestDialogue(
            "Godfather",
            "You look troubled, my child. Tell me… why have you come?",
            "I've come to repent.",
            "I don't know what I'm supposed to do anymore.",
            "I did what I had to do.",
            OnFirstChoiceSelected
        );
    }

    void OnBlockedChoiceSelected(int choice)
    {
        if (dialogueManager == null)
            return;

        dialogueManager.EndDialogue();
        StartCoroutine(WaitForDialogueToCloseThenShowPrompt());
    }

    void OnFirstChoiceSelected(int choice)
    {
        if (dialogueManager == null)
            return;

        if (choice == 1)
        {
            dialogueManager.ShowSideQuestSingleLine(
                "Godfather",
                "Repentance is not spoken with the lips alone. Close your eyes… and place your hands together."
            );

            StartCoroutine(StartRepentAfterDialogueCloses());
        }
        else if (choice == 2)
        {
            dialogueManager.ShowSideQuestFollowUp(
                "Godfather",
                "When the heart is lost, silence can help you hear it again.",
                "I will try.",
                "I am still confused.",
                "Goodbye.",
                OnSecondChoiceSelected
            );
        }
        else if (choice == 3)
        {
            dialogueManager.ShowSideQuestFollowUp(
                "Godfather",
                "Perhaps. But one day every person must face their own choices.",
                "Maybe you're right.",
                "I don't regret it.",
                "Goodbye.",
                OnThirdChoiceSelected
            );
        }
    }

    void OnSecondChoiceSelected(int choice)
    {
        if (dialogueManager == null)
            return;

        if (choice == 1)
        {
            dialogueManager.ShowSideQuestSingleLine(
                "Godfather",
                "Then be still, even if only for a moment. A quiet heart hears more clearly."
            );
        }
        else if (choice == 2)
        {
            dialogueManager.ShowSideQuestSingleLine(
                "Godfather",
                "Confusion is not a sin, my child. Refusing to face it is."
            );
        }
        else
        {
            dialogueManager.EndDialogue();
        }

        StartCoroutine(WaitForDialogueToCloseThenShowPrompt());
    }

    void OnThirdChoiceSelected(int choice)
    {
        if (dialogueManager == null)
            return;

        if (choice == 1)
        {
            dialogueManager.ShowSideQuestSingleLine(
                "Godfather",
                "It is never too late to question the road beneath your feet."
            );
        }
        else if (choice == 2)
        {
            dialogueManager.ShowSideQuestSingleLine(
                "Godfather",
                "Then perhaps one day regret will come to you before peace does."
            );
        }
        else
        {
            dialogueManager.EndDialogue();
        }

        StartCoroutine(WaitForDialogueToCloseThenShowPrompt());
    }

    IEnumerator StartRepentAfterDialogueCloses()
    {
        while (dialogueManager != null && dialogueManager.isDialogueOpen)
            yield return null;

        if (repentSequence != null)
            repentSequence.StartRepentSequence();

        while (repentSequence != null && !repentSequence.repentCompleted)
            yield return null;

        if (playerInRange && interactPrompt != null && (dialogueManager == null || !dialogueManager.isDialogueOpen))
            interactPrompt.SetActive(true);
    }

    IEnumerator WaitForDialogueToCloseThenShowPrompt()
    {
        while (dialogueManager != null && dialogueManager.isDialogueOpen)
            yield return null;

        if (playerInRange && interactPrompt != null)
            interactPrompt.SetActive(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;

            if (interactPrompt != null && (dialogueManager == null || !dialogueManager.isDialogueOpen))
                interactPrompt.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;

            if (interactPrompt != null)
                interactPrompt.SetActive(false);
        }
    }
}