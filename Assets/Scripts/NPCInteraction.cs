using UnityEngine;

public class NPCInteraction : MonoBehaviour
{
    public GameObject interactionPrompt;
    public DialogueManager dialogueManager;

    private bool playerInRange = false;

    void Start()
    {
        if (interactionPrompt != null)
        {
            interactionPrompt.SetActive(false);
        }
    }

    void Update()
    {
        if (playerInRange && !dialogueManager.isDialogueOpen)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                dialogueManager.StartDialogue();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;

            if (interactionPrompt != null && !dialogueManager.isDialogueOpen)
            {
                interactionPrompt.SetActive(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;

            if (interactionPrompt != null)
            {
                interactionPrompt.SetActive(false);
            }
        }
    }
}