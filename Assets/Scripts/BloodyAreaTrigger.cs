using UnityEngine;

public class BloodyAreaTrigger : MonoBehaviour
{
    [Header("References")]
    public SuspicionSystem suspicionSystem;
    public DialogueManager dialogueManager;
    public MoralStateManager moralStateManager;
    public InventoryManager inventoryManager;
    public DateEventManager dateEventManager;

    [Header("Optional Block Wall")]
    public GameObject blockWall;

    [Header("Dialogue")]
    public string speakerName = "";
    [TextArea] public string enterDialogue = "What the...?";
    [TextArea] public string leaveWithoutInteractionDialogue = "Why did you step in here if you didn't want to remember?";
    [TextArea] public string blockedDialogue = "I don't want to go there.";
    [TextArea] public string rewardDialogue = "You memorized what you did.";

    [Header("Reward")]
    public string rewardItemId = "knife";

    private bool playerInside = false;
    private bool hasInteractedInArea = false;
    private bool enterDialoguePlayed = false;
    private bool blockedLinePlayed = false;
    private bool rewardGiven = false;

    private void Start()
    {
        UpdateBlockWall();
    }

    private void Update()
    {
        UpdateBlockWall();
    }

    private void UpdateBlockWall()
    {
        if (blockWall == null || moralStateManager == null)
            return;

        blockWall.SetActive(moralStateManager.hasRepented);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (moralStateManager != null && moralStateManager.hasRepented)
        {
            if (!blockedLinePlayed && dialogueManager != null && !dialogueManager.isDialogueOpen)
            {
                blockedLinePlayed = true;
                dialogueManager.ShowSideQuestSingleLine(speakerName, blockedDialogue);
            }

            return;
        }

        playerInside = true;
        hasInteractedInArea = false;
        blockedLinePlayed = false;

        if (!enterDialoguePlayed)
        {
            enterDialoguePlayed = true;

            if (dialogueManager != null)
            {
                dialogueManager.ShowSideQuestSingleLine(speakerName, enterDialogue);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (moralStateManager != null && moralStateManager.hasRepented)
        {
            blockedLinePlayed = false;
            return;
        }

        if (!playerInside) return;

        if (hasInteractedInArea)
        {
            if (suspicionSystem != null)
            {
                suspicionSystem.IncreaseSuspicion();
            }

            GiveReward();
        }
        else
        {
            if (dialogueManager != null)
            {
                dialogueManager.ShowSideQuestSingleLine(speakerName, leaveWithoutInteractionDialogue);
            }
        }

        playerInside = false;
        hasInteractedInArea = false;
        enterDialoguePlayed = false;
    }

    public void RegisterInteraction()
    {
        if (!playerInside) return;

        hasInteractedInArea = true;

        if (moralStateManager != null)
        {
            moralStateManager.MarkBloodyAreaTouched();
        }
    }

    private void GiveReward()
    {
        if (rewardGiven)
            return;

        bool rewardAdded = false;

        if (inventoryManager != null)
        {
            if (inventoryManager.HasItem(rewardItemId))
            {
                rewardAdded = true;
            }
            else
            {
                rewardAdded = inventoryManager.AddItem(rewardItemId);
            }
        }

        if (!rewardAdded)
        {
            Debug.Log("BloodyAreaTrigger: Could not add reward item.");
            return;
        }

        rewardGiven = true;

        if (GameProgressManager.Instance != null)
        {
            GameProgressManager.Instance.UnlockBloodyAreaReward();
        }

        if (dateEventManager != null)
        {
            dateEventManager.StartReturnEvent(rewardItemId);
        }

        if (dialogueManager != null)
        {
            dialogueManager.ShowSideQuestSingleLine(speakerName, rewardDialogue);
        }

        Debug.Log("Bloody area reward earned and Date return event started.");
    }
}