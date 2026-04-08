using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System;

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

    private Coroutine typingCoroutine;
    private bool isTyping = false;
    private string currentFullLine = "";
    private string currentSpeaker = "";

    private Action<int> currentSideQuestCallback = null;
    private bool canCloseSingleLine = false;
    private float inputBlockTimer = 0f;

    void Start()
    {
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);

        choiceButton1.onClick.AddListener(ChooseOption1);
        choiceButton2.onClick.AddListener(ChooseOption2);
        choiceButton3.onClick.AddListener(ChooseOption3);

        HideChoices();
    }

    void Update()
    {
        if (inputBlockTimer > 0f)
            inputBlockTimer -= Time.deltaTime;

        if (!isDialogueOpen)
            return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            EndDialogue();
            return;
        }

        if (inputBlockTimer > 0f)
            return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (isTyping)
            {
                FinishTypingInstantly();
                return;
            }

            if (canCloseSingleLine)
            {
                EndDialogue();
            }
        }
    }

    public void StartSideQuestDialogue(string speaker, string line, string option1, string option2, string option3, Action<int> callback)
    {
        currentSideQuestCallback = callback;
        canCloseSingleLine = false;
        inputBlockTimer = 0.2f;

        isDialogueOpen = true;

        if (dialoguePanel != null)
            dialoguePanel.SetActive(true);

        if (interactionPrompt != null)
            interactionPrompt.SetActive(false);

        if (playerMovement != null)
            playerMovement.enabled = false;

        if (playerLook != null)
            playerLook.enabled = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        HideChoices();

        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeLineWithChoicesCoroutine(speaker, line, option1, option2, option3));
    }

    IEnumerator TypeLineWithChoicesCoroutine(string speaker, string line, string option1, string option2, string option3)
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

        ShowChoices(option1, option2, option3);
    }

    public void ShowSideQuestFollowUp(string speaker, string line, string option1, string option2, string option3, Action<int> callback)
    {
        currentSideQuestCallback = callback;
        canCloseSingleLine = false;
        inputBlockTimer = 0.2f;

        HideChoices();

        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeLineWithChoicesCoroutine(speaker, line, option1, option2, option3));
    }

    public void ShowSideQuestSingleLine(string speaker, string line)
    {
        currentSideQuestCallback = null;
        canCloseSingleLine = false;
        inputBlockTimer = 0.2f;

        isDialogueOpen = true;

        if (dialoguePanel != null)
            dialoguePanel.SetActive(true);

        if (interactionPrompt != null)
            interactionPrompt.SetActive(false);

        if (playerMovement != null)
            playerMovement.enabled = false;

        if (playerLook != null)
            playerLook.enabled = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        HideChoices();

        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeSingleLineCoroutine(speaker, line));
    }

    IEnumerator TypeSingleLineCoroutine(string speaker, string line)
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
        canCloseSingleLine = true;
    }

    public void ShowChoices(string option1, string option2, string option3)
    {
        choiceButton1.gameObject.SetActive(!string.IsNullOrEmpty(option1));
        choiceButton2.gameObject.SetActive(!string.IsNullOrEmpty(option2));
        choiceButton3.gameObject.SetActive(!string.IsNullOrEmpty(option3));

        choiceButton1Text.text = option1;
        choiceButton2Text.text = option2;
        choiceButton3Text.text = option3;
    }

    public void HideChoices()
    {
        choiceButton1.gameObject.SetActive(false);
        choiceButton2.gameObject.SetActive(false);
        choiceButton3.gameObject.SetActive(false);
    }

    void ChooseOption1()
    {
        HideChoices();

        Action<int> callback = currentSideQuestCallback;
        currentSideQuestCallback = null;
        callback?.Invoke(1);
    }

    void ChooseOption2()
    {
        HideChoices();

        Action<int> callback = currentSideQuestCallback;
        currentSideQuestCallback = null;
        callback?.Invoke(2);
    }

    void ChooseOption3()
    {
        HideChoices();

        Action<int> callback = currentSideQuestCallback;
        currentSideQuestCallback = null;
        callback?.Invoke(3);
    }

    void FinishTypingInstantly()
    {
        if (!isTyping)
            return;

        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }

        isTyping = false;
        speakerText.text = currentSpeaker;
        dialogueText.text = currentFullLine;

        if (currentSideQuestCallback == null)
            canCloseSingleLine = true;
    }

    public void EndDialogue()
    {
        isDialogueOpen = false;

        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);

        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        isTyping = false;
        typingCoroutine = null;
        currentFullLine = "";
        currentSpeaker = "";
        currentSideQuestCallback = null;
        canCloseSingleLine = false;
        inputBlockTimer = 0f;

        HideChoices();

        if (playerMovement != null)
            playerMovement.enabled = true;

        if (playerLook != null)
            playerLook.enabled = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}