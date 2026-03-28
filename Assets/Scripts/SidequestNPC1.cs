using UnityEngine;

public class SidequestNPC1 : MonoBehaviour
{
    public string npcName = "Mysterious Girl";

    [TextArea(2, 5)]
    public string openingLine = "Oh god, finally I see a person! I lost my diary somewhere and I can't find it! Can you help me look around?";

    [TextArea(2, 5)]
    public string response1 = "I've been standing here for what feels like forever... Nobody stopped. Nobody even looked at me.";

    [TextArea(2, 5)]
    public string response2 = "I was writing near the road, then I heard something behind me. I panicked and ran. When I came back, it was gone.";

    [TextArea(2, 5)]
    public string response3 = "Really? Thank you... I think I dropped it somewhere nearby. Please be careful.";

    private bool playerInRange = false;
    private bool questStarted = false;

    private void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            if (!DialogueManager.instance.isDialogueOpen)
            {
                StartDialogue();
            }
        }
    }

    void StartDialogue()
    {
        DialogueManager.instance.StartSideQuestDialogue(
            npcName,
            openingLine,
            new string[]
            {
                "What do you mean finally see a person?",
                "Can you remember how you lost it?",
                "I can help you look for it."
            },
            OnChoiceSelected
        );
    }

    void OnChoiceSelected(int choiceIndex)
    {
        switch (choiceIndex)
        {
            case 0:
                DialogueManager.instance.ShowFollowUpDialogue(
                    npcName,
                    response1,
                    new string[] { "Can you remember how you lost it?", "I'll help you look for it." },
                    OnSecondChoiceAfterResponse1
                );
                break;

            case 1:
                DialogueManager.instance.ShowFollowUpDialogue(
                    npcName,
                    response2,
                    new string[] { "That sounds strange...", "I'll help you look for it." },
                    OnSecondChoiceAfterResponse2
                );
                break;

            case 2:
                StartQuestNow();
                break;
        }
    }

    void OnSecondChoiceAfterResponse1(int choiceIndex)
    {
        if (choiceIndex == 0)
        {
            DialogueManager.instance.ShowFollowUpDialogue(
                npcName,
                response2,
                new string[] { "I'll help you look for it." },
                OnFinalHelpChoice
            );
        }
        else
        {
            StartQuestNow();
        }
    }

    void OnSecondChoiceAfterResponse2(int choiceIndex)
    {
        if (choiceIndex == 0)
        {
            DialogueManager.instance.ShowFollowUpDialogue(
                npcName,
                "I know... this place doesn't feel right.",
                new string[] { "I'll help you look for it." },
                OnFinalHelpChoice
            );
        }
        else
        {
            StartQuestNow();
        }
    }

    void OnFinalHelpChoice(int choiceIndex)
    {
        StartQuestNow();
    }

    void StartQuestNow()
    {
        if (!questStarted)
        {
            questStarted = true;
            DialogueManager.instance.ShowSingleLine(
                npcName,
                "Thank you... I think it should be somewhere around the road or near the trees."
            );

            Debug.Log("Side quest started: Find the diary");
        }
        else
        {
            DialogueManager.instance.ShowSingleLine(
                npcName,
                "Please look around nearby. I know it has to be here somewhere..."
            );
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            InteractionPromptUI.instance?.ShowPrompt("Press E to talk");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            InteractionPromptUI.instance?.HidePrompt();
        }
    }
}