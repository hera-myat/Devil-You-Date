using UnityEngine;

public class DateDialogue : MonoBehaviour
{
    [Header("References")]
    public DialogueManager dialogueManager;
    public SuspicionSystem suspicionSystem;
    public InventoryManager inventoryManager;
    public DateEventManager dateEventManager;
    public NormalEndingSequence normalEndingSequence;

    [Header("Quest Reward")]
    public string coinAwardItemId = "coinaward";

    [Header("Objective UI")]
    public ObjectiveUI objectiveUI;

    private bool firstMeetingDone = false;

    [HideInInspector]
    public bool hasTalkedToDate = false;

    private string currentGift = "";

    private bool leftWithExcuse = false;
    private int excuseIndex = 0;
    private bool requestLeaveChair = false;

    private bool pendingObjectiveAfterLeave = false;

    private string[] leaveExcuses =
    {
        "I need to use the restroom for a while.",
        "I need to make a quick call.",
        "I think I forgot something.",
        "I'll be right back."
    };

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

        if (inventoryManager != null && inventoryManager.HasItem("knife"))
        {
            // stop any return-to-date event / timer
            if (dateEventManager != null)
            {
                dateEventManager.LockDateSystem();
            }

            activeReturnEventId = "";
            activeReturnLate = false;
            eventTopic1Used = false;
            eventTopic2Used = false;
            pendingObjectiveAfterLeave = false;
            requestLeaveChair = false;
            leftWithExcuse = false;

            if (normalEndingSequence != null)
            {
                normalEndingSequence.StartNormalEnding();
            }
            return;
        }

        hasTalkedToDate = true;
        requestLeaveChair = false;

        if (GameProgressManager.Instance != null)
        {
            GameProgressManager.Instance.MarkMetDate();
        }

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

        if (!firstMeetingDone)
        {
            StartFirstMeetingDialogue();
            return;
        }

        if (leftWithExcuse)
        {
            leftWithExcuse = false;
            ShowMainMenu("Date", "You're back.");
            return;
        }

        ShowMainMenu("Date", "Hello again.");
    }


    void StartFirstMeetingDialogue()
    {
        if (dialogueManager == null)
            return;

        dialogueManager.StartSideQuestDialogue(
            "Date",
            "Hello.",
            "Hi... sorry for being late.",
            "Hello. Have you been waiting long?",
            "You are my date, right?",
            OnFirstMeetingChoice
        );
    }

    void OnFirstMeetingChoice(int choice)
    {
        if (dialogueManager == null)
            return;

        if (choice == 1)
        {
            dialogueManager.ShowSideQuestFollowUp(
                "Date",
                "A little. But not long enough to be disappointed.",
                "That's surprisingly nice of you.",
                "You say that like you expected me to run.",
                "Can we sit and talk?",
                OnFirstMeetingChoice1
            );
        }
        else if (choice == 2)
        {
            dialogueManager.ShowSideQuestFollowUp(
                "Date",
                "Not really. I like watching people before they notice they're being watched.",
                "That sounds a little creepy.",
                "So you've just been sitting here observing everyone?",
                "You make it sound normal.",
                OnFirstMeetingChoice2
            );
        }
        else if (choice == 3)
        {
            dialogueManager.ShowSideQuestFollowUp(
                "Date",
                "That depends. Are you hoping I am?",
                "That's not really an answer.",
                "You're hard to read.",
                "I think I found the right person.",
                OnFirstMeetingChoice3
            );
        }
    }

    void OnFirstMeetingChoice1(int choice)
    {
        if (choice == 1)
        {
            FinishFirstMeeting("Date", "I try to be polite the first time I meet someone. It keeps people from leaving too early.");
        }
        else if (choice == 2)
        {
            FinishFirstMeeting("Date", "People leave for all kinds of reasons. Fear. Boredom. Guilt.");
        }
        else
        {
            FinishFirstMeeting("Date", "Of course. We have the whole evening ahead of us.");
        }
    }

    void OnFirstMeetingChoice2(int choice)
    {
        if (choice == 1)
        {
            FinishFirstMeeting("Date", "Maybe. But honesty usually sounds strange at first.");
        }
        else if (choice == 2)
        {
            FinishFirstMeeting("Date", "Only the interesting ones.");
        }
        else
        {
            FinishFirstMeeting("Date", "A lot of strange things become normal if you sit with them long enough.");
        }
    }

    void OnFirstMeetingChoice3(int choice)
    {
        if (choice == 1)
        {
            FinishFirstMeeting("Date", "I've been told I answer questions like I'm hiding something.");
        }
        else if (choice == 2)
        {
            FinishFirstMeeting("Date", "That usually makes people curious.");
        }
        else
        {
            FinishFirstMeeting("Date", "Good. Then let's see how long that feeling lasts.");
        }
    }

    void FinishFirstMeeting(string speaker, string line)
    {
        firstMeetingDone = true;
        ShowMainMenuFollowUp(speaker, line);
    }
    void ShowMainMenu(string speaker, string line)
    {
        if (dialogueManager == null)
            return;

        string option2Text = string.IsNullOrEmpty(currentGift)
            ? "You seem like you notice everything."
            : "Give him " + currentGift;

        dialogueManager.StartSideQuestDialogue(
            speaker,
            line,
            "Do you come here often?",
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
            ? "You seem like you notice everything."
            : "Give him " + currentGift;

        dialogueManager.ShowSideQuestFollowUp(
            speaker,
            line,
            "Do you come here often?",
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
                "Enough to know what people look like when they're pretending to be comfortable.",
                "And what do I look like?",
                "You really study people that closely?",
                "Let's talk about something else.",
                OnComeHereChoice
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
                dialogueManager.ShowSideQuestFollowUp(
                    "Date",
                    "Not everything. Just the things people try hardest to hide.",
                    "That sounds dangerous.",
                    "Maybe people hide things for a reason.",
                    "You make that sound personal.",
                    OnNoticeEverythingChoice
                );
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
            pendingObjectiveAfterLeave = true;
        }
    }

    void Update()
    {
        if (pendingObjectiveAfterLeave && dialogueManager != null && !dialogueManager.isDialogueOpen)
        {
            pendingObjectiveAfterLeave = false;
            ShowNextObjective();
        }
    }

    void OnComeHereChoice(int choice)
    {
        if (choice == 1)
        {
            ShowMainMenuFollowUp("Date", "Careful. Like you don't enjoy being read too quickly.");
        }
        else if (choice == 2)
        {
            ShowMainMenuFollowUp("Date", "Only when they give me a reason.");
        }
        else
        {
            ShowMainMenuFollowUp("Date", "Then pick another subject.");
        }
    }

    void OnNoticeEverythingChoice(int choice)
    {
        if (choice == 1)
        {
            ShowMainMenuFollowUp("Date", "Only for the right person.");
        }
        else if (choice == 2)
        {
            ShowMainMenuFollowUp("Date", "Usually. But sometimes people hide things because they enjoy keeping them.");
        }
        else
        {
            ShowMainMenuFollowUp("Date", "Maybe it is.");
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

    void ShowNextObjective()
    {
        if (objectiveUI == null || inventoryManager == null)
            return;

        bool hasCoinAward = inventoryManager.HasItem("coinaward");
        bool hasPlanner = inventoryManager.HasItem("planner");
        bool hasTrashClean = inventoryManager.HasItem("trashclean");
        bool hasCross = inventoryManager.HasItem("cross");

        // All done → no hint (good ending handles it)
        if (hasCoinAward && hasPlanner && hasTrashClean && hasCross)
            return;

        // FIRST step → go to vending/trash area
        if (!hasCoinAward)
        {
            objectiveUI.ShowObjective("Objective: Check the trash cans near the vending machines.");
            return;
        }

        // SECOND → diary
        if (!hasPlanner)
        {
            objectiveUI.ShowObjective("Objective: I heard someone scream nearby... I should look around.");
            return;
        }

        // THIRD → garbage cleaning
        if (!hasTrashClean)
        {
            objectiveUI.ShowObjective("Objective: Something feels off near the police area... I should check it out.");
            return;
        }

        // FOURTH → church
        if (!hasCross)
        {
            objectiveUI.ShowObjective("Objective: I heard there's a church nearby... maybe I should take a look.");
            return;
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

        if (GameProgressManager.Instance != null)
        {
            GameProgressManager.Instance.hasStartedCoinSearch = false;
        }

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

        if (choice == 1)
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

            LeaveAfterReturnEvent(GetReturnLeaveResponse(activeReturnEventId, activeReturnLate));
            return;
        }

        if (choice == 2)
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

            dialogueManager.ShowSideQuestFollowUp(
                "Date",
                GetReturnExtraResponse(activeReturnEventId, activeReturnLate),
                GetRemainingReturnOption1(),
                "",
                GetReturnLeaveOption(activeReturnEventId),
                OnReturnEventFollowUpChoice
            );
            return;
        }

        LeaveAfterReturnEvent(GetReturnLeaveResponse(activeReturnEventId, activeReturnLate));
    }

    string GetReturnExtraResponse(string eventId, bool late)
    {
        if (eventId == "planner")
            return "Different? Maybe. People always look a little different after reading something they shouldn't.";

        if (eventId == "cross")
            return "Strange is just another word for honest, when honesty makes people uncomfortable.";

        if (eventId == "trashclean")
            return "Ordinary things are usually the best place to hide unusual intentions.";

        if (eventId == "knife")
            return "Maybe I do. Or maybe you just look like someone waiting to be understood.";

        return "Maybe that's because I'm still deciding what to make of you.";
    }

    string GetReturnExtraOption(string eventId)
    {
        if (eventId == "planner") return "You seem different tonight.";
        if (eventId == "cross") return "You're speaking strangely again.";
        if (eventId == "trashclean") return "You always make ordinary things sound suspicious.";
        if (eventId == "knife") return "You keep looking at me like you know something.";
        return "You seem hard to read.";
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
        // First screen: show the normal second topic
        if (!eventTopic1Used && !eventTopic2Used)
            return GetReturnOption2(activeReturnEventId);

        // After topic 1 is used: show extra option
        if (eventTopic1Used && !eventTopic2Used)
            return GetReturnExtraOption(activeReturnEventId);

        // After topic 2 is used: show extra option
        if (!eventTopic1Used && eventTopic2Used)
            return GetReturnExtraOption(activeReturnEventId);

        // Both used: no second option
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

        ShowNextObjective();
    }

    string GetReturnIntroLine(string eventId, bool late)
    {
        if (eventId == "planner")
        {
            return late
                ? "You came back late... and with the look of someone who read something they weren't meant to."
                : "You found something, didn't you? Something private.";
        }

        if (eventId == "cross")
        {
            return late
                ? "You were gone long enough to pray... or long enough to regret."
                : "You seem quieter. People usually get that look when they ask for forgiveness.";
        }

        if (eventId == "trashclean")
        {
            return late
                ? "You took your time. You smell like dust, cardboard, and something almost scrubbed away."
                : "You came back smelling like dust and cardboard. Trying to clean something up?";
        }

        if (eventId == "knife")
        {
            return late
                ? "You look pale. Like something recognized you before you understood why."
                : "You look different. Like you remembered something you tried to bury.";
        }

        return late ? "You were gone longer than I expected." : "You're back.";
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

