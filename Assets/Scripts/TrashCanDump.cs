using UnityEngine;
using TMPro;

public class TrashCanDump : MonoBehaviour
{
    [Header("References")]
    public InventoryManager inventoryManager;
    public GameObject interactPrompt;
    public TextMeshProUGUI resultText;
    public SearchableObject searchableTrash;
    public CanCleanupTimer cleanupTimer;
    public SuspicionSystem suspicionSystem;
    public DateDialogue dateDialogue;

    [Header("Settings")]
    public int cansNeeded = 4;

    private int dumpedCans = 0;
    private bool playerInRange = false;
    private bool searchUnlocked = false;
    private bool questFailed = false;
    private bool completionSuspicionApplied = false;

    void Start()
    {
        if (interactPrompt != null)
            interactPrompt.SetActive(false);

        if (resultText != null)
        {
            resultText.text = "";
            resultText.gameObject.SetActive(false);
        }

        if (searchableTrash != null)
            searchableTrash.canSearch = false;
    }

    void Update()
    {
        if (cleanupTimer != null && cleanupTimer.HasFinished() && !searchUnlocked && !questFailed)
        {
            questFailed = true;

            if (suspicionSystem != null)
                suspicionSystem.IncreaseSuspicion();

            ShowMessage("I took too long cleaning. Someone might get suspicious.");
        }

        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            DumpCan();
        }
    }

    void DumpCan()
    {
        if (questFailed)
        {
            ShowMessage("Too late. I already missed my chance.");
            return;
        }

        if (searchUnlocked)
        {
            ShowMessage("The cans are already cleaned.");
            return;
        }

        if (inventoryManager == null)
            return;

        bool removed = inventoryManager.RemoveItem("can");

        if (removed)
        {
            dumpedCans++;

            ShowMessage("Cleaned cans: " + dumpedCans + "/" + cansNeeded);

            if (dumpedCans >= cansNeeded)
            {
                searchUnlocked = true;

                if (cleanupTimer != null)
                    cleanupTimer.StopTimer();

                if (searchableTrash != null)
                    searchableTrash.UnlockSearch();

                if (dateDialogue != null && !dateDialogue.hasTalkedToDate && !completionSuspicionApplied && suspicionSystem != null)
                {
                    suspicionSystem.IncreaseSuspicion();
                    completionSuspicionApplied = true;
                }

                ShowMessage("Finally finished cleaning... Maybe I should check the orange trash can.");
            }
        }
        else
        {
            ShowMessage("I do not have any cans to throw away.");
        }
    }

    void ShowMessage(string message)
    {
        if (resultText != null)
        {
            resultText.text = message;
            resultText.gameObject.SetActive(true);

            CancelInvoke(nameof(HideMessage));
            Invoke(nameof(HideMessage), 3f);
        }
    }

    void HideMessage()
    {
        if (resultText != null)
            resultText.gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;

            if (interactPrompt != null)
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