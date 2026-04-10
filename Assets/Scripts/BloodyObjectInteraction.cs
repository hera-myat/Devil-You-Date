using UnityEngine;

public class BloodyObjectInteraction : MonoBehaviour
{
    [Header("References")]
    public DialogueManager dialogueManager;
    public BloodyAreaTrigger bloodyAreaTrigger;
    public GameObject interactPrompt;

    [Header("Dialogue")]
    public string speakerName = "";
    [TextArea] public string objectDialogue;

    private bool playerInRange = false;
    private bool hasInteracted = false;

    private void Start()
    {
        if (interactPrompt != null)
            interactPrompt.SetActive(false);
    }

    private void Update()
    {
        if (!playerInRange) return;
        if (hasInteracted) return;
        if (dialogueManager != null && dialogueManager.isDialogueOpen) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            Interact();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        playerInRange = true;

        if (interactPrompt != null && !hasInteracted)
            interactPrompt.SetActive(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        playerInRange = false;

        if (interactPrompt != null)
            interactPrompt.SetActive(false);
    }

    private void Interact()
    {
        hasInteracted = true;

        if (interactPrompt != null)
            interactPrompt.SetActive(false);

        if (bloodyAreaTrigger != null)
            bloodyAreaTrigger.RegisterInteraction();

        if (dialogueManager != null)
            dialogueManager.ShowSideQuestSingleLine(speakerName, objectDialogue);
    }
}