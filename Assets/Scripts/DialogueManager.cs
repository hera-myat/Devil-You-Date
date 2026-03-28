using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

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

    [Header("Typing Effect")]
    public float typingSpeed = 0.03f;

    [Header("State")]
    public bool isDialogueOpen = false;
    public bool hasCoin = false;
    public int suspicion = 0;

    private int dialogueState = 0;
    private bool waitingForNext = false;
    private int pendingResponse = 0;

    private Coroutine typingCoroutine;
    private bool isTyping = false;
    private string currentFullLine = "";
    private string currentSpeaker = "";

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

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            EndDialogue();
            return;
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (isTyping)
            {
                FinishTypingInstantly();
                return;
            }

            if (waitingForNext)
            {
                AdvanceDialogue();
            }
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
        HideChoices();
        waitingForNext = true;
        StartTyping("Player", "Hi.");
    }

    void AdvanceDialogue()
    {
        if (dialogueState == 0)
        {
            ShowFirstNpcReply();
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

    void ShowFirstNpcReply()
    {
        HideChoices();
        dialogueState = 1;
        waitingForNext = false;

        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeFirstNpcReply());
    }

    IEnumerator TypeFirstNpcReply()
    {
        yield return StartCoroutine(TypeLine("Date", "Hi, how's it going?"));

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
    }

    void StartTyping(string speaker, string line)
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeLine(speaker, line));
    }

    IEnumerator TypeLine(string speaker, string line)
    {
        isTyping = true;
        currentSpeaker = speaker;
        currentFullLine = line;

        speakerText.text = speaker;
        dialogueText.text = "";

        foreach (char c in line)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
        typingCoroutine = null;
    }

    void FinishTypingInstantly()
    {
        if (!isTyping) return;

        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        speakerText.text = currentSpeaker;
        dialogueText.text = currentFullLine;
        isTyping = false;
        typingCoroutine = null;
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
            HideChoices();
            waitingForNext = true;
            pendingResponse = 1;
            StartTyping("Player", "How has your day been?");
            return;
        }

        if (dialogueState == 2)
        {
            HideChoices();
            waitingForNext = true;
            pendingResponse = 4;
            StartTyping("Player", "It looked unusual, so I thought I'd ask.");
            return;
        }
    }

    void ResponseOption1()
    {
        HideChoices();

        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeResponseOption1());
    }

    IEnumerator TypeResponseOption1()
    {
        yield return StartCoroutine(TypeLine("Date", "Pretty normal. Just walking around and getting some fresh air."));

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
            HideChoices();
            waitingForNext = true;
            pendingResponse = 2;

            if (hasCoin)
            {
                StartTyping("Player", "Hey, I found this coin nearby. Is it yours?");
            }
            else
            {
                StartTyping("Player", "Can you tell me a little about your past?");
            }

            return;
        }

        if (dialogueState == 2)
        {
            HideChoices();
            waitingForNext = true;
            pendingResponse = 5;
            StartTyping("Player", "Are you sure? You reacted kind of strangely.");
            return;
        }
    }

    void ResponseOption2()
    {
        HideChoices();

        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeResponseOption2());
    }

    IEnumerator TypeResponseOption2()
    {
        if (hasCoin && dialogueState == 1)
        {
            yield return StartCoroutine(TypeLine("Date", "No, I don't think so. It looks pretty old though."));

            dialogueState = 2;
            waitingForNext = false;

            ShowChoices(
                "It looked unusual, so I thought I'd ask.",
                "Are you sure? You reacted kind of strangely.",
                "Never mind, just wanted to check."
            );
            yield break;
        }

        yield return StartCoroutine(TypeLine("Date", "I moved around a lot, so I don't really stay in one place for long."));

        waitingForNext = false;

        ShowChoices(
            "Ask why you moved so much",
            "Change the topic",
            "End conversation"
        );
    }

    void ChooseOption3()
    {
        if (dialogueState == 1)
        {
            HideChoices();
            waitingForNext = true;
            pendingResponse = 3;
            StartTyping("Player", "I should get going. See you.");
            return;
        }

        if (dialogueState == 2)
        {
            HideChoices();
            waitingForNext = true;
            pendingResponse = 6;
            StartTyping("Player", "Never mind, just wanted to check.");
            return;
        }
    }

    void CoinResponseSafe()
    {
        HideChoices();

        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeCoinResponseSafe());
    }

    IEnumerator TypeCoinResponseSafe()
    {
        yield return StartCoroutine(TypeLine("Date", "Fair enough. It does look a little odd."));

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
        HideChoices();

        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeCoinResponseSuspicious());
    }

    IEnumerator TypeCoinResponseSuspicious()
    {
        yield return StartCoroutine(TypeLine("Date", "What? No, I didn't. Why are you making this such a big deal?\n\nSuspicion +1"));

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
        HideChoices();

        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeCoinResponseNeutral());
    }

    IEnumerator TypeCoinResponseNeutral()
    {
        yield return StartCoroutine(TypeLine("Date", "Alright. No problem."));

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

        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        isTyping = false;
        typingCoroutine = null;
        currentFullLine = "";
        currentSpeaker = "";

        if (playerMovement != null) playerMovement.enabled = true;
        if (playerLook != null) playerLook.enabled = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}