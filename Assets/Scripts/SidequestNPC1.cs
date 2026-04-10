using UnityEngine;

public class SideQuestNPC : MonoBehaviour
{
    [Header("Quest Settings")]
    public string npcName = "Girl";
    public GameObject diaryObject;
    public InventoryManager inventoryManager;

    [Header("Optional Prompt")]
    public GameObject interactionPrompt;

    [Header("Quest State")]
    public bool questStarted = false;
    public bool questCompleted = false;
    public bool playerHasDiary = false;
    public bool diaryReturnedLate = false;

    [Header("Timer")]
    public SideQuestTimer sideQuestTimer;

    [Header("Animation")]
    public string talkTriggerName = "Talk";

    [Header("Suspicion")]
    public SuspicionSystem suspicionSystem;

    [Header("Date Event")]
    public DateEventManager dateEventManager;
    public string rewardItemId = "planner";

    private bool playerInRange = false;
    private DialogueManager dialogueManager;
    private Animator anim;
    private bool hasCalmedDown = false;

    private bool askedFoundSomeone = false;
    private bool askedLostDiary = false;

    void Start()
    {
        dialogueManager = FindObjectOfType<DialogueManager>();
        anim = GetComponent<Animator>();

        if (diaryObject != null)
        {
            diaryObject.SetActive(false);
        }
    }

    void Update()
    {
        if (!playerInRange) return;
        if (dialogueManager == null) return;
        if (dialogueManager.isDialogueOpen) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            TalkToNPC();
        }
    }

    void TalkToNPC()
    {
        if (!hasCalmedDown && anim != null)
        {
            anim.SetTrigger(talkTriggerName);
            hasCalmedDown = true;
        }

        if (questCompleted)
        {
            dialogueManager.ShowSideQuestSingleLine(
                npcName,
                "Thank you again... I really thought I had lost it for good."
            );
            return;
        }

        if (questStarted && playerHasDiary)
        {
            CompleteQuest();
            return;
        }

        if (questStarted)
        {
            dialogueManager.ShowSideQuestSingleLine(
                npcName,
                "Please look around nearby... I think I dropped it somewhere around the asylum."
            );
            return;
        }

        ShowMainChoices();
    }

    void ShowMainChoices()
    {
        string option1 = "";
        string option2 = "";
        string option3 = "";

        int filled = 0;

        if (!askedFoundSomeone)
        {
            filled++;
            SetOption(ref option1, ref option2, ref option3, filled, "What do you mean you just found someone?");
        }

        if (!askedLostDiary)
        {
            filled++;
            SetOption(ref option1, ref option2, ref option3, filled, "Can you remember where you lost it?");
        }

        filled++;
        SetOption(ref option1, ref option2, ref option3, filled, "Alright, I'll help you look for it.");

        dialogueManager.StartSideQuestDialogue(
            npcName,
            "Oh god, finally I found someone... I think I lost my diary somewhere around the asylum, and I don't know where to even start looking. Could you help me find it?",
            option1,
            option2,
            option3,
            OnMainChoiceSelected
        );
    }

    void OnMainChoiceSelected(int choice)
    {
        string selectedText = GetCurrentMainChoiceText(choice);

        Debug.Log("Main choice selected: " + selectedText);

        if (selectedText == "What do you mean you just found someone?")
        {
            askedFoundSomeone = true;

            dialogueManager.ShowSideQuestFollowUp(
                npcName,
                "I've been walking around in circles for a while, and I somehow kept ending up back here. I was starting to think I was completely lost.",
                "Can you remember where you lost it?",
                "That sounds unsettling.",
                "I'll help you look for it.",
                OnFoundSomeoneFollowUp
            );
        }
        else if (selectedText == "Can you remember where you lost it?")
        {
            askedLostDiary = true;

            dialogueManager.ShowSideQuestFollowUp(
                npcName,
                "I was writing near the roadside by the asylum, and then I heard something behind me. I got scared and ran without thinking. When I stopped, the diary was gone.",
                "That sounds unsettling.",
                "Maybe it fell somewhere around the asylum?",
                "I'll help you look for it.",
                OnLostDiaryFollowUp
            );
        }
        else if (selectedText == "Alright, I'll help you look for it.")
        {
            StartQuestNow();
        }
    }

    void OnFoundSomeoneFollowUp(int choice)
    {
        if (choice == 1)
        {
            askedLostDiary = true;

            dialogueManager.ShowSideQuestFollowUp(
                npcName,
                "I was writing near the roadside by the asylum, and then I heard something behind me. I got scared and ran without thinking. When I stopped, the diary was gone.",
                "That sounds unsettling.",
                "Maybe it fell somewhere around the asylum?",
                "I'll help you look for it.",
                OnLostDiaryFollowUp
            );
        }
        else if (choice == 2)
        {
            dialogueManager.ShowSideQuestFollowUp(
                npcName,
                "It really is... I don't want to stay out here alone any longer.",
                GetRemainingQuestionOption(),
                "I'll help you look for it.",
                "Let me think for a second.",
                OnReducedChoices
            );
        }
        else if (choice == 3)
        {
            StartQuestNow();
        }
    }

    void OnLostDiaryFollowUp(int choice)
    {
        if (choice == 1)
        {
            dialogueManager.ShowSideQuestFollowUp(
                npcName,
                "Yeah... I know it sounds strange, but I swear I heard something right behind me.",
                GetRemainingQuestionOption(),
                "I'll help you look for it.",
                "Let me think for a second.",
                OnReducedChoices
            );
        }
        else if (choice == 2)
        {
            dialogueManager.ShowSideQuestFollowUp(
                npcName,
                "Maybe... could you check around the asylum first? I feel like it has to be somewhere close.",
                GetRemainingQuestionOption(),
                "I'll help you look for it.",
                "Alright, I'll check there first.",
                OnReducedChoices
            );
        }
        else if (choice == 3)
        {
            StartQuestNow();
        }
    }

    void OnReducedChoices(int choice)
    {
        string option1 = GetRemainingQuestionOption();
        string option2 = "I'll help you look for it.";
        string option3 = "Let me think for a second.";

        string selectedText = "";
        if (choice == 1) selectedText = option1;
        if (choice == 2) selectedText = option2;
        if (choice == 3) selectedText = option3;

        Debug.Log("Reduced choice selected: " + selectedText);

        if (selectedText == "What do you mean you just found someone?")
        {
            askedFoundSomeone = true;

            dialogueManager.ShowSideQuestFollowUp(
                npcName,
                "I've been walking around in circles for a while, and I somehow kept ending up back here. I was starting to think I was completely lost.",
                "I'll help you look for it.",
                "Can you remember where you lost it?",
                "Let me think for a second.",
                OnReducedFoundSomeone
            );
        }
        else if (selectedText == "Can you remember where you lost it?")
        {
            askedLostDiary = true;

            dialogueManager.ShowSideQuestFollowUp(
                npcName,
                "I was writing near the roadside by the asylum, and then I heard something behind me. I got scared and ran without thinking. When I stopped, the diary was gone.",
                "I'll help you look for it.",
                "Maybe it fell somewhere around the asylum?",
                "Let me think for a second.",
                OnReducedLostDiary
            );
        }
        else if (selectedText == "I'll help you look for it.")
        {
            StartQuestNow();
        }
        else
        {
            dialogueManager.ShowSideQuestSingleLine(
                npcName,
                "Please... if you can help, I would really appreciate it."
            );
        }
    }

    void OnReducedFoundSomeone(int choice)
    {
        if (choice == 1)
        {
            StartQuestNow();
        }
        else if (choice == 2)
        {
            askedLostDiary = true;

            dialogueManager.ShowSideQuestFollowUp(
                npcName,
                "I was writing near the roadside by the asylum, and then I heard something behind me. I got scared and ran without thinking. When I stopped, the diary was gone.",
                "I'll help you look for it.",
                "Maybe it fell somewhere around the asylum?",
                "Let me think for a second.",
                OnReducedLostDiary
            );
        }
        else
        {
            dialogueManager.ShowSideQuestSingleLine(
                npcName,
                "Please... I think it has to be somewhere around the asylum."
            );
        }
    }

    void OnReducedLostDiary(int choice)
    {
        if (choice == 1)
        {
            StartQuestNow();
        }
        else if (choice == 2)
        {
            dialogueManager.ShowSideQuestSingleLine(
                npcName,
                "Then maybe the asylum grounds are the best place to start."
            );
        }
        else
        {
            dialogueManager.ShowSideQuestSingleLine(
                npcName,
                "Please... I think it has to be somewhere around the asylum."
            );
        }
    }

    void StartQuestNow()
    {
        questStarted = true;
        diaryReturnedLate = false;

        if (diaryObject != null)
        {
            diaryObject.SetActive(true);
            Debug.Log("Diary object activated.");
        }
        else
        {
            Debug.LogWarning("SideQuestNPC: diaryObject is not assigned.");
        }

        if (sideQuestTimer != null)
        {
            Debug.Log("Starting side quest timer...");
            sideQuestTimer.StartTimer();
        }
        else
        {
            Debug.LogWarning("SideQuestNPC: sideQuestTimer is not assigned.");
        }

        dialogueManager.ShowSideQuestSingleLine(
            npcName,
            "Thank you... I think I dropped it somewhere around the asylum. Please come back if you find it."
        );

        Debug.Log("Side quest started: Find the diary");
    }

    void CompleteQuest()
    {
        questCompleted = true;
        questStarted = false;
        playerHasDiary = false;

        if (sideQuestTimer != null)
        {
            sideQuestTimer.StopTimer();
        }

        bool rewardAdded = false;

        if (inventoryManager != null)
        {
            inventoryManager.RemoveItem("diary");

            if (inventoryManager.HasItem(rewardItemId))
            {
                rewardAdded = true;
            }
            else
            {
                rewardAdded = inventoryManager.AddItem(rewardItemId);

                if (!rewardAdded)
                {
                    Debug.Log("Could not add planner because inventory is full.");
                }
            }
        }

        if (suspicionSystem != null &&
            GameProgressManager.Instance != null &&
            !GameProgressManager.Instance.hasMetDate)
        {
            suspicionSystem.IncreaseSuspicion();
            Debug.Log("Suspicion increased because player completed the side quest before meeting the Date.");
        }

        if (GameProgressManager.Instance != null)
        {
            GameProgressManager.Instance.UnlockDiaryReward();
        }

        if (rewardAdded && dateEventManager != null)
        {
            dateEventManager.StartReturnEvent(rewardItemId);
        }

        string completeLine = "";

        if (diaryReturnedLate)
        {
            completeLine = "Thank you... Even though you brought it back late, I'm still glad you found it.";
        }
        else
        {
            completeLine = "You found it... thank you. I was scared I'd never get it back.";
        }

        dialogueManager.ShowSideQuestSingleLine(
            npcName,
            completeLine
        );

        Debug.Log("Side quest completed: Diary returned. Date return event started.");
    }

    public void SetPlayerHasDiary(bool value)
    {
        playerHasDiary = value;
    }

    void SetOption(ref string option1, ref string option2, ref string option3, int slot, string value)
    {
        if (slot == 1) option1 = value;
        else if (slot == 2) option2 = value;
        else if (slot == 3) option3 = value;
    }

    string GetCurrentMainChoiceText(int choice)
    {
        string[] options = BuildMainOptions();
        int index = choice - 1;

        if (index >= 0 && index < options.Length)
            return options[index];

        return "";
    }

    string[] BuildMainOptions()
    {
        System.Collections.Generic.List<string> options = new System.Collections.Generic.List<string>();

        if (!askedFoundSomeone)
            options.Add("What do you mean you just found someone?");

        if (!askedLostDiary)
            options.Add("Can you remember where you lost it?");

        options.Add("Alright, I'll help you look for it.");

        return options.ToArray();
    }

    string GetRemainingQuestionOption()
    {
        if (!askedFoundSomeone)
            return "What do you mean you just found someone?";

        if (!askedLostDiary)
            return "Can you remember where you lost it?";

        return "Do you remember anything else?";
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;

            if (interactionPrompt != null && dialogueManager != null && !dialogueManager.isDialogueOpen)
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