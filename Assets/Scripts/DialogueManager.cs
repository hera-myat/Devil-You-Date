using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    [Header("UI")]
    public GameObject dialoguePanel;
    public TextMeshProUGUI speakerText;
    public TextMeshProUGUI dialogueText;

    public Button choiceButton1;
    public Button choiceButton2;
    public Button choiceButton3;

    public TextMeshProUGUI choiceButton1Text;
    public TextMeshProUGUI choiceButton2Text;
    public TextMeshProUGUI choiceButton3Text;

    [Header("Player Scripts To Disable During Dialogue")]
    public efPlayerMovement playerMovement;
    public efPlayerLook playerLook;

    [Header("Optional Prompt")]
    public GameObject interactionPrompt;

    [Header("State")]
    public bool isDialogueOpen = false;
    public bool hasCoin = false;
    public int suspicion = 0;

    private int dialogueState = 0;
    private bool waitingForNext = false;
    private int pendingResponse = 0;

    void Start()
    {
        dialoguePanel.SetActive(false);

        choiceButton1.onClick.AddListener(ChooseOption1);
        choiceButton2.onClick.AddListener(ChooseOption2);
        choiceButton3.onClick.AddListener(ChooseOption3);

        HideChoices();
    }

    void Update()
    {
        if (!isDialogueOpen) return;

        if (waitingForNext && Input.GetKeyDown(KeyCode.E))
        {
            AdvanceDialogue();
        }
    }

    public void StartDialogue()
    {
        isDialogueOpen = true;
        dialogueState = 0;
        waitingForNext = true;
        pendingResponse = 0;

        dialoguePanel.SetActive(true);

        if (interactionPrompt != null)
            interactionPrompt.SetActive(false);

        if (playerMovement != null) playerMovement.enabled = false;
        if (playerLook != null) playerLook.enabled = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        HideChoices();
        ShowGreeting();
    }

    void ShowGreeting()
    {
        speakerText.text = "Player";
        dialogueText.text = "Hi.";
    }

    void AdvanceDialogue()
    {
        if (dialogueState == 0)
        {
            speakerText.text = "Date";
            dialogueText.text = "Hi, how's it going?";
            dialogueState = 1;
            waitingForNext = false;

            if (hasCoin)
            {
                ShowChoices(
                    "Ask about their day",
                    "Ask about the coin",
                    "Say goodbye"
                );
            }
            else
            {
                ShowChoices(
                    "Ask about their day",
                    "Ask about their past",
                    "Say goodbye"
                );
            }
            return;
        }

        if (pendingResponse == 1)
        {
            ResponseOption1();
            pendingResponse = 0;
            return;
        }

        if (pendingResponse == 2)
        {
            ResponseOption2();
            pendingResponse = 0;
            return;
        }

        if (pendingResponse == 3)
        {
            EndDialogue();
            pendingResponse = 0;
            return;
        }

        if (pendingResponse == 4)
        {
            CoinResponseSafe();
            pendingResponse = 0;
            return;
        }

        if (pendingResponse == 5)
        {
            CoinResponseSuspicious();
            pendingResponse = 0;
            return;
        }

        if (pendingResponse == 6)
        {
            CoinResponseNeutral();
            pendingResponse = 0;
            return;
        }
    }

    void ShowChoices(string option1, string option2, string option3)
    {
        choiceButton1.gameObject.SetActive(true);
        choiceButton2.gameObject.SetActive(true);
        choiceButton3.gameObject.SetActive(true);

        choiceButton1Text.text = option1;
        choiceButton2Text.text = option2;
        choiceButton3Text.text = option3;
    }

    void ChooseOption1()
    {
        if (dialogueState == 1)
        {
            speakerText.text = "Player";
            dialogueText.text = "How has your day been?";

            HideChoices();
            waitingForNext = true;
            pendingResponse = 1;
            return;
        }

        if (dialogueState == 2)
        {
            speakerText.text = "Player";
            dialogueText.text = "It looked unusual, so I thought I'd ask.";

            HideChoices();
            waitingForNext = true;
            pendingResponse = 4;
            return;
        }
    }

    void ResponseOption1()
    {
        speakerText.text = "Date";
        dialogueText.text = "Pretty normal. Just walking around and getting some fresh air.";

        waitingForNext = false;

        ShowChoices(
            "Ask about their day",
            "Ask about their past",
            "End conversation"
        );
    }

    void ChooseOption2()
    {
        if (dialogueState == 1)
        {
            if (hasCoin)
            {
                speakerText.text = "Player";
                dialogueText.text = "Hey, I found this coin nearby. Is it yours?";

                HideChoices();
                waitingForNext = true;
                pendingResponse = 2;
            }
            else
            {
                speakerText.text = "Player";
                dialogueText.text = "Can you tell me a little about your past?";

                HideChoices();
                waitingForNext = true;
                pendingResponse = 2;
            }

            return;
        }

        if (dialogueState == 2)
        {
            speakerText.text = "Player";
            dialogueText.text = "Are you sure? You reacted kind of strangely.";

            HideChoices();
            waitingForNext = true;
            pendingResponse = 5;
            return;
        }
    }

    void ResponseOption2()
    {
        if (hasCoin && dialogueState == 1)
        {
            speakerText.text = "Date";
            dialogueText.text = "No, I don't think so. It looks pretty old though.";

            dialogueState = 2;
            waitingForNext = false;

            ShowChoices(
                "It looked unusual, so I thought I'd ask.",
                "Are you sure? You reacted kind of strangely.",
                "Never mind, just wanted to check."
            );
            return;
        }

        speakerText.text = "Date";
        dialogueText.text = "I moved around a lot, so I don't really stay in one place for long.";

        waitingForNext = false;

        ShowChoices(
            "Ask why he moved so much",
            "Change the topic",
            "End conversation"
        );
    }

    void ChooseOption3()
    {
        if (dialogueState == 1)
        {
            speakerText.text = "Player";
            dialogueText.text = "I should get going. See you.";

            HideChoices();
            waitingForNext = true;
            pendingResponse = 3;
            return;
        }

        if (dialogueState == 2)
        {
            speakerText.text = "Player";
            dialogueText.text = "Never mind, just wanted to check.";

            HideChoices();
            waitingForNext = true;
            pendingResponse = 6;
            return;
        }
    }

    void CoinResponseSafe()
    {
        speakerText.text = "Date";
        dialogueText.text = "Fair enough. It does look a little odd.";

        waitingForNext = false;

        ShowChoices(
            "Ask about their day",
            "Ask about their past",
            "End conversation"
        );

        dialogueState = 1;
    }

    void CoinResponseSuspicious()
    {
        suspicion += 1;

        speakerText.text = "Date";
        dialogueText.text = "What? No, I didn't. Why are you making this such a big deal?\n\n\nSuspicion +";

        waitingForNext = false;

        ShowChoices(
            "Ask about their day",
            "Ask about their past",
            "End conversation"
        );

        dialogueState = 1;
    }

    void CoinResponseNeutral()
    {
        speakerText.text = "Date";
        dialogueText.text = "Alright. No problem.";

        waitingForNext = false;

        ShowChoices(
            "Ask about their day",
            "Ask about their past",
            "End conversation"
        );

        dialogueState = 1;
    }

    void HideChoices()
    {
        choiceButton1.gameObject.SetActive(false);
        choiceButton2.gameObject.SetActive(false);
        choiceButton3.gameObject.SetActive(false);
    }

    public void EndDialogue()
    {
        isDialogueOpen = false;
        dialoguePanel.SetActive(false);

        if (playerMovement != null) playerMovement.enabled = true;
        if (playerLook != null) playerLook.enabled = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}