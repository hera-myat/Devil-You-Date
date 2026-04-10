using UnityEngine;

public class DateDialogue : MonoBehaviour
{
    [Header("References")]
    public DialogueManager dialogueManager;
    public SuspicionSystem suspicionSystem;
    public InventoryManager inventoryManager;
    public DateEventManager dateEventManager;

    [Header("Quest Reward")]
    public string coinAwardItemId = "coinaward";

    [HideInInspector]
    public bool hasTalkedToDate = false;

    private string currentGift = "";

    private bool leftWithExcuse = false;
    private int excuseIndex = 0;
    private bool requestLeaveChair = false;

    private string[] leaveExcuses =
    {
        "I need to use the restroom for a while.",
        "I need to make a quick call.",
        "I think I forgot something.",
        "I'll be right back."
    };

    // Active special return conversation
    private string activeReturnEventId = "";
    private bool activeReturnLate = false;
    private bool eventTopic1Used = false;
    private bool eventTopic2Used = false;

    public void StartDateDialogue()
    {
        if (dialogueManager == null)
            return;

        if (dialogueManager.isDialogueOpen)
            return;

        hasTalkedToDate = true;
        requestLeaveChair = false;

        if (GameProgressManager.Instance != null)
        {
            GameProgressManager.Instance.MarkMetDate();
        }

        // New reward just returned
        if (dateEventManager != null && dateEventManager.HasPendingEvent())
        {
            dateEventManager.PlayerReturnedForCurrentEvent();

            activeReturnEventId = dateEventManager.GetCurrentEventId();
            activeReturnLate = dateEventManager.IsCurrentEventLate();
            eventTopic1Used = false;
            eventTopic2Used = false;

            ShowReturnEventMenu(true);
            return;
        }


        if (!string.IsNullOrEmpty(activeReturnEventId))
        {
            ShowReturnEventMenu(true);
            return;
        }

        if (leftWithExcuse)
        {
            leftWithExcuse = false;
            ShowMainMenu("Date", "You're back.");
            return;
        }

        ShowMainMenu("Date", "Hi, how's it going?");
    }

    void ShowMainMenu(string speaker, string line)
    {
        if (dialogueManager == null)
            return;

        string option2Text = string.IsNullOrEmpty(currentGift)
            ? "Ask what he does for work"
            : "Give him " + currentGift;

        dialogueManager.StartSideQuestDialogue(
            speaker,
            line,
            "Ask about your day",
            option2Text,
            GetCurrentExcuseText(),
            OnMainChoice
        );
    }

    void ShowMainMenuFollowUp(string speaker, string line)
    {
        if (dialogueManager == null)
            return;

        string option2Text = string.IsNullOrEmpty(currentGift)
            ? "Ask what he does for work"
            : "Give him " + currentGift;

        dialogueManager.ShowSideQuestFollowUp(
            speaker,
            line,
            "Ask about your day",
            option2Text,
            GetCurrentExcuseText(),
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
                "Pretty quiet. I walked a little, listened to people pass by, and watched the light change. It tells you more than people think.",
                "You notice things like that a lot?",
                "That sounds kind of lonely.",
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
                ShowOccupationMenu();
            }
        }
        else if (choice == 3)
        {
            leftWithExcuse = true;

            string chosenExcuse = GetCurrentExcuseText();
            AdvanceExcuse();

            dialogueManager.ShowSideQuestSingleLine(
                "Date",
                GetExcuseResponse(chosenExcuse)
            );

            requestLeaveChair = true;
        }
    }

    void OnDayChoice(int choice)
    {
        if (dialogueManager == null)
            return;

        if (choice == 1)
        {
            ShowMainMenuFollowUp("Date", "Sometimes. People say one thing, but they carry another in their face, in their hands, in the way they sit still.");
        }
        else if (choice == 2)
        {
            ShowMainMenuFollowUp("Date", "Maybe. Though I think most people are lonely long before they realize it.");
        }
        else
        {
            ShowMainMenuFollowUp("Date", "Of course. We have time.");
        }
    }

    void ShowOccupationMenu()
    {
        if (dialogueManager == null)
            return;

        dialogueManager.ShowSideQuestFollowUp(
            "Date",
            "Work? That's a difficult thing to answer cleanly. I listen. I wait. Sometimes people tell me things they don't even mean to say.",
            "That sounds mysterious.",
            "So what do you actually do?",
            "I should tell you what I do too.",
            OnOccupationChoice
        );
    }

    void OnOccupationChoice(int choice)
    {
        if (dialogueManager == null)
            return;

        if (choice == 1)
        {
            ShowMainMenuFollowUp("Date", "Maybe mystery is just honesty with better manners.");
        }
        else if (choice == 2)
        {
            ShowMainMenuFollowUp("Date", "I could give you a simple answer. But simple answers are usually the least true ones.");
        }
        else
        {
            dialogueManager.ShowSideQuestFollowUp(
                "Date",
                "Then tell me. What kind of work suits you?",
                "Something ordinary. Nothing interesting.",
                "I keep busy. That's enough.",
                "Maybe I'll tell you another time.",
                OnPlayerOccupationChoice
            );
        }
    }

    void OnPlayerOccupationChoice(int choice)
    {
        if (dialogueManager == null)
            return;

        if (choice == 1)
        {
            ShowMainMenuFollowUp("Date", "Ordinary can hide many things. Some people are safest when no one looks twice at them.");
        }
        else if (choice == 2)
        {
            ShowMainMenuFollowUp("Date", "Keeping busy is one way to stay ahead of your own thoughts.");
        }
        else
        {
            ShowMainMenuFollowUp("Date", "Fair enough. People usually reveal themselves slowly anyway.");
        }
    }

    void HandleGiftResponse()
    {
        string gift = currentGift.ToLower();
        RemoveGiftFromInventory(gift);

        if (inventoryManager != null && !inventoryManager.HasItem(coinAwardItemId))
        {
            bool addedAward = inventoryManager.AddItem(coinAwardItemId);

            if (!addedAward)
            {
                Debug.Log("Could not add coinaward because inventory is full.");
            }
        }

        if (gift == "soda")
        {
            if (suspicionSystem != null)
                suspicionSystem.DecreaseSuspicion();

            currentGift = "";
            ShowMainMenuFollowUp("Date", "Oh, soda? Thanks... that's actually thoughtful. Most people only offer things when they want something back.");
        }
        else if (gift == "coffee")
        {
            currentGift = "";
            ShowMainMenuFollowUp("Date", "Coffee... I see. Bitter things suit some evenings better than sweet ones.");
        }
        else if (gift == "food")
        {
            currentGift = "";
            ShowMainMenuFollowUp("Date", "Food? Thanks. Sharing a table with someone changes the air a little.");
        }
        else
        {
            currentGift = "";
            ShowMainMenuFollowUp("Date", "Oh, thanks. I'll remember that.");
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


    void ShowReturnEventMenu(bool firstOpen)
    {
        if (dialogueManager == null || string.IsNullOrEmpty(activeReturnEventId))
            return;

        string introLine = GetReturnIntroLine(activeReturnEventId, activeReturnLate);

        string option1 = eventTopic1Used ? "" : GetReturnOption1(activeReturnEventId);
        string option2 = eventTopic2Used ? "" : GetReturnOption2(activeReturnEventId);
        string option3 = GetReturnLeaveOption(activeReturnEventId);

        if (eventTopic1Used && eventTopic2Used)
        {
            option1 = GetReturnLeaveOption(activeReturnEventId);
            option2 = "";
            option3 = "";
        }

        if (firstOpen)
        {
            dialogueManager.StartSideQuestDialogue(
                "Date",
                introLine,
                option1,
                option2,
                option3,
                OnReturnEventMenuChoice
            );
        }
        else
        {
            dialogueManager.ShowSideQuestFollowUp(
                "Date",
                introLine,
                option1,
                option2,
                option3,
                OnReturnEventMenuChoice
            );
        }
    }

    void OnReturnEventMenuChoice(int choice)
    {
        if (string.IsNullOrEmpty(activeReturnEventId) || dialogueManager == null)
            return;

        if (eventTopic1Used && eventTopic2Used)
        {
            LeaveAfterReturnEvent(GetReturnLeaveResponse(activeReturnEventId, activeReturnLate));
            return;
        }

        if (choice == 1 && !eventTopic1Used)
        {
            eventTopic1Used = true;

            dialogueManager.ShowSideQuestFollowUp(
                "Date",
                GetReturnTopic1Response(activeReturnEventId, activeReturnLate),
                GetRemainingReturnOption1(),
                GetRemainingReturnOption2(),
                GetReturnLeaveOption(activeReturnEventId),
                OnReturnEventFollowUpChoice
            );
            return;
        }

        if (choice == 2 && !eventTopic2Used)
        {
            eventTopic2Used = true;

            dialogueManager.ShowSideQuestFollowUp(
                "Date",
                GetReturnTopic2Response(activeReturnEventId, activeReturnLate),
                GetRemainingReturnOption1(),
                GetRemainingReturnOption2(),
                GetReturnLeaveOption(activeReturnEventId),
                OnReturnEventFollowUpChoice
            );
            return;
        }

        LeaveAfterReturnEvent(GetReturnLeaveResponse(activeReturnEventId, activeReturnLate));
    }

    void OnReturnEventFollowUpChoice(int choice)
    {
        if (string.IsNullOrEmpty(activeReturnEventId) || dialogueManager == null)
            return;

        string remaining1 = GetRemainingReturnOption1();
        string remaining2 = GetRemainingReturnOption2();

        if (string.IsNullOrEmpty(remaining1) && string.IsNullOrEmpty(remaining2))
        {
            LeaveAfterReturnEvent(GetReturnLeaveResponse(activeReturnEventId, activeReturnLate));
            return;
        }

        if (!string.IsNullOrEmpty(remaining1) && choice == 1)
        {
            if (!eventTopic1Used)
            {
                eventTopic1Used = true;
                dialogueManager.ShowSideQuestFollowUp(
                    "Date",
                    GetReturnTopic1Response(activeReturnEventId, activeReturnLate),
                    GetRemainingReturnOption1(),
                    GetRemainingReturnOption2(),
                    GetReturnLeaveOption(activeReturnEventId),
                    OnReturnEventFollowUpChoice
                );
                return;
            }

            if (!eventTopic2Used)
            {
                eventTopic2Used = true;
                dialogueManager.ShowSideQuestFollowUp(
                    "Date",
                    GetReturnTopic2Response(activeReturnEventId, activeReturnLate),
                    GetRemainingReturnOption1(),
                    GetRemainingReturnOption2(),
                    GetReturnLeaveOption(activeReturnEventId),
                    OnReturnEventFollowUpChoice
                );
                return;
            }
        }

        if (!string.IsNullOrEmpty(remaining2) && choice == 2)
        {
            if (!eventTopic2Used)
            {
                eventTopic2Used = true;
                dialogueManager.ShowSideQuestFollowUp(
                    "Date",
                    GetReturnTopic2Response(activeReturnEventId, activeReturnLate),
                    GetRemainingReturnOption1(),
                    GetRemainingReturnOption2(),
                    GetReturnLeaveOption(activeReturnEventId),
                    OnReturnEventFollowUpChoice
                );
                return;
            }
        }

        LeaveAfterReturnEvent(GetReturnLeaveResponse(activeReturnEventId, activeReturnLate));
    }

    string GetRemainingReturnOption1()
    {
        if (!eventTopic1Used)
            return GetReturnOption1(activeReturnEventId);

        if (!eventTopic2Used)
            return GetReturnOption2(activeReturnEventId);

        return GetReturnLeaveOption(activeReturnEventId);
    }

    string GetRemainingReturnOption2()
    {
        if (!eventTopic1Used && !eventTopic2Used)
            return GetReturnOption2(activeReturnEventId);

        if (!eventTopic1Used && eventTopic2Used)
            return "";

        if (eventTopic1Used && !eventTopic2Used)
            return GetReturnLeaveOption(activeReturnEventId);

        return "";
    }

    void LeaveAfterReturnEvent(string line)
    {
        leftWithExcuse = true;
        requestLeaveChair = true;

        dialogueManager.ShowSideQuestSingleLine("Date", line);

        if (dateEventManager != null)
            dateEventManager.MarkCurrentEventPlayed();

        activeReturnEventId = "";
        activeReturnLate = false;
        eventTopic1Used = false;
        eventTopic2Used = false;
    }

    string GetReturnIntroLine(string eventId, bool late)
    {
        if (eventId == "planner")
        {
            return late
                ? "You were gone longer than I expected. You came back looking like your mind's still somewhere else."
                : "Oh... you're back. You look like you found what you went looking for.";
        }

        if (eventId == "cross")
        {
            return late
                ? "You disappeared for a while... but you came back quieter. Almost like you left something behind."
                : "You seem calmer than before. That's rare.";
        }

        if (eventId == "trashclean")
        {
            return late
                ? "You took your time. You look tired... like you've been trying to make something disappear."
                : "You look tired. Were you doing something useful, or just avoiding me?";
        }

        if (eventId == "knife")
        {
            return late
                ? "Something happened out there, didn't it? You look like the night followed you back."
                : "You look like you've been somewhere unpleasant. Your face changed before your words did.";
        }

        return late ? "You were gone longer than I expected." : "Oh... you're alright.";
    }

    string GetReturnOption1(string eventId)
    {
        if (eventId == "planner") return "Sorry, I'm back.";
        if (eventId == "cross") return "Do I seem that different?";
        if (eventId == "trashclean") return "You make that sound suspicious.";
        if (eventId == "knife") return "Do I really look that bad?";
        return "Sorry, I'm back.";
    }

    string GetReturnOption2(string eventId)
    {
        if (eventId == "planner") return "Did I keep you waiting?";
        if (eventId == "cross") return "Do you believe people can change?";
        if (eventId == "trashclean") return "Can we talk about something normal?";
        if (eventId == "knife") return "Can we talk about something else?";
        return "Did I keep you waiting?";
    }

    string GetReturnLeaveOption(string eventId)
    {
        if (eventId == "planner") return "Can I head out again for a bit?";
        if (eventId == "cross") return "I should get going.";
        if (eventId == "trashclean") return "I should get going.";
        if (eventId == "knife") return "I should get going.";
        return "I should get going.";
    }

    string GetReturnTopic1Response(string eventId, bool late)
    {
        if (eventId == "planner")
        {
            return late
                ? "It's alright. Waiting tells you what kind of person is worth staying for."
                : "You made it back. That matters more than the excuse.";
        }

        if (eventId == "cross")
        {
            return late
                ? "A person can return carrying less than what they left with. Guilt. Noise. Fear."
                : "A little. Like you've stopped arguing with yourself for a moment.";
        }

        if (eventId == "trashclean")
        {
            return late
                ? "Maybe. But tired people usually tell more truth than rested ones."
                : "Only a little. Avoidance and usefulness can look very similar from a distance.";
        }

        if (eventId == "knife")
        {
            return late
                ? "You don't owe me the truth. But you do look like you've seen something you wish you hadn't."
                : "A little. Mostly in the eyes. Fear lingers there longer than people expect.";
        }

        return "I see.";
    }

    string GetReturnTopic2Response(string eventId, bool late)
    {
        if (eventId == "planner")
        {
            return late
                ? "A little. But you came back with your breathing steadier than before."
                : "Not really. Just long enough to notice the silence.";
        }

        if (eventId == "cross")
        {
            return "Change? Yes. Escape? That's harder.";
        }

        if (eventId == "trashclean")
        {
            return "Normal is overrated. People show themselves best when they think they're doing something ordinary.";
        }

        if (eventId == "knife")
        {
            return late
                ? "Of course. But changing the subject doesn't always change what stays with you."
                : "We can. Though some places cling to people longer than they should.";
        }

        return "Not really.";
    }

    string GetReturnLeaveResponse(string eventId, bool late)
    {
        if (eventId == "planner")
            return "Alright. Just don't vanish completely.";

        if (eventId == "cross")
            return "Alright. Go carefully.";

        if (eventId == "trashclean")
            return "Alright. I suppose ordinary conversation can wait.";

        if (eventId == "knife")
        {
            return late
                ? "Alright... but whatever found you out there seems reluctant to let go."
                : "Alright. You still look unsettled.";
        }

        return "Alright. Go carefully.";
    }

    public bool ConsumeLeaveChairRequest()
    {
        bool result = requestLeaveChair;
        requestLeaveChair = false;
        return result;
    }

    string GetCurrentExcuseText()
    {
        return leaveExcuses[excuseIndex];
    }

    void AdvanceExcuse()
    {
        excuseIndex++;
        if (excuseIndex >= leaveExcuses.Length)
            excuseIndex = 0;
    }

    string GetExcuseResponse(string excuse)
    {
        if (excuse.Contains("restroom"))
            return "Of course. Even short absences can change a mood.";
        if (excuse.Contains("call"))
            return "That's fine. Some voices insist on being answered.";
        if (excuse.Contains("forgot"))
            return "Alright. Memory has a habit of tugging at inconvenient times.";
        return "Okay. I'll wait here.";
    }
}