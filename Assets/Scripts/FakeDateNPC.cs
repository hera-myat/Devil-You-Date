using UnityEngine;

public class FakeDateNPC : MonoBehaviour
{
    [Header("References")]
    public DialogueManager dialogueManager;
    public GameObject interactPrompt;

    [Header("Settings")]
    public KeyCode interactKey = KeyCode.E;

    [Header("Dialogue")]
    public string speakerName = "Stranger";
    [TextArea]
    public string[] dialogueLines =
    {
        "Umm... how can I help you?",
        "Your date?",
        "Haha, I know I am pretty charming, but I’m not your date and you've got the wrong person."
    };

    private bool playerInRange = false;
    private bool isTalking = false;

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(interactKey) && !isTalking)
        {
            StartConversation();
        }
    }

    void StartConversation()
    {
        if (dialogueManager == null) return;

        isTalking = true;

        StartCoroutine(PlayDialogue());
    }

    System.Collections.IEnumerator PlayDialogue()
    {
        foreach (string line in dialogueLines)
        {
            dialogueManager.ShowSideQuestSingleLine(speakerName, line);

            yield return new WaitUntil(() => dialogueManager.isDialogueOpen);

            yield return new WaitUntil(() => !dialogueManager.isDialogueOpen);
        }

        isTalking = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        playerInRange = true;

        if (interactPrompt != null)
            interactPrompt.SetActive(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        playerInRange = false;

        if (interactPrompt != null)
            interactPrompt.SetActive(false);
    }
}