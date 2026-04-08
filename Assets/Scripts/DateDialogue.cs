using UnityEngine;

public class DateDialogue : MonoBehaviour
{
    [Header("References")]
    public DialogueManager dialogueManager;
    public GameObject interactPrompt;
    public SuspicionSystem suspicionSystem;
    public InventoryManager inventoryManager;

    [HideInInspector]
    public bool hasTalkedToDate = false;

    private bool playerInRange = false;
    private string currentGift = "";

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
            StartDateDialogue();
        }
    }

    public void StartDateDialogue()
    {
        if (dialogueManager == null)
            return;

        hasTalkedToDate = true;

        if (interactPrompt != null)
            interactPrompt.SetActive(false);

        ShowMainMenu("Date", "Hi, how's it going?");
    }

    void ShowMainMenu(string speaker, string line)
    {
        if (dialogueManager == null)
            return;

        string option2Text = string.IsNullOrEmpty(currentGift)
            ? "Ask about your past"
            : "Give him " + currentGift;

        dialogueManager.StartSideQuestDialogue(
            speaker,
            line,
            "Ask about your day",
            option2Text,
            "I want to use the restroom for a while.",
            OnMainChoice
        );
    }

    void ShowMainMenuFollowUp(string speaker, string line)
    {
        if (dialogueManager == null)
            return;

        string option2Text = string.IsNullOrEmpty(currentGift)
            ? "Ask about your past"
            : "Give him " + currentGift;

        dialogueManager.ShowSideQuestFollowUp(
            speaker,
            line,
            "Ask about your day",
            option2Text,
            "I want to use the restroom for a while.",
            OnMainChoice
        );
    }

    void OnMainChoice(int choice)
    {
        if (dialogueManager == null)
            return;

        if (choice == 1)
        {
            dialogueManager.ShowSideQuestFollowUp(
                "Date",
                "Pretty normal. Just walking around, thinking a little, and getting some fresh air.",
                "That sounds nice.",
                "Were you waiting long?",
                "Let's talk about something else.",
                OnDayChoice
            );
        }
        else if (choice == 2)
        {
            if (!string.IsNullOrEmpty(currentGift))
            {
                HandleGiftResponse();
            }
            else
            {
                ShowPastMenu();
            }
        }
        else if (choice == 3)
        {
            dialogueManager.ShowSideQuestSingleLine(
                "Date",
                "Oh, of course. Take your time."
            );
        }
    }

    void OnDayChoice(int choice)
    {
        if (dialogueManager == null)
            return;

        if (choice == 1)
        {
            ShowMainMenuFollowUp("Date", "Yeah... it helps clear my head a little.");
        }
        else if (choice == 2)
        {
            ShowMainMenuFollowUp("Date", "Not really. I only got here a little before you did.");
        }
        else
        {
            ShowMainMenuFollowUp("Date", "Sure. What do you want to talk about?");
        }
    }

    void ShowPastMenu()
    {
        if (dialogueManager == null)
            return;

        dialogueManager.ShowSideQuestFollowUp(
            "Date",
            "I moved around a lot, so I never really stayed anywhere for very long.",
            "Did you hate that?",
            "That sounds lonely.",
            "We can talk about something else.",
            OnPastChoice
        );
    }

    void OnPastChoice(int choice)
    {
        if (dialogueManager == null)
            return;

        if (choice == 1)
        {
            ShowMainMenuFollowUp("Date", "Sometimes. But after a while, you just stop expecting things to stay the same.");
        }
        else if (choice == 2)
        {
            ShowMainMenuFollowUp("Date", "Maybe it was. I think I just got used to it.");
        }
        else
        {
            ShowMainMenuFollowUp("Date", "Sure. We don't have to stay on heavy things.");
        }
    }

    void HandleGiftResponse()
    {
        string gift = currentGift.ToLower();
        RemoveGiftFromInventory(gift);

        if (gift == "soda")
        {
            if (suspicionSystem != null)
                suspicionSystem.DecreaseSuspicion();

            currentGift = "";
            ShowMainMenuFollowUp("Date", "Oh, soda? Thanks... that's actually really nice of you.");
        }
        else if (gift == "coffee")
        {
            currentGift = "";
            ShowMainMenuFollowUp("Date", "Oh... coffee. Thanks, I guess.");
        }
        else if (gift == "food")
        {
            currentGift = "";
            ShowMainMenuFollowUp("Date", "Food? Thanks.");
        }
        else
        {
            currentGift = "";
            ShowMainMenuFollowUp("Date", "Oh, thanks.");
        }
    }

    void RemoveGiftFromInventory(string gift)
    {
        if (inventoryManager == null)
            return;

        inventoryManager.RemoveItem(gift);
    }

    public void SetGiftItem(string giftName)
    {
        currentGift = giftName.ToLower();
        Debug.Log("Date gift set to: " + currentGift);
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