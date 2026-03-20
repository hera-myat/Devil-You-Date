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

    public bool isDialogueOpen = false;

    private int dialogueState = 0;

    void Start()
    {
        dialoguePanel.SetActive(false);

        choiceButton1.onClick.AddListener(ChooseOption1);
        choiceButton2.onClick.AddListener(ChooseOption2);
        choiceButton3.onClick.AddListener(ChooseOption3);
    }

    public void StartDialogue()
    {
        isDialogueOpen = true;
        dialogueState = 0;

        dialoguePanel.SetActive(true);

        if (interactionPrompt != null)
        {
            interactionPrompt.SetActive(false);
        }

        //Stop player movement during conversation
        if (playerMovement != null) playerMovement.enabled = false;
        if (playerLook != null) playerLook.enabled = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        ShowGreeting();
    }

    void ShowGreeting()
    {
        speakerText.text = "Player";
        dialogueText.text = "Hi.";

        choiceButton1.gameObject.SetActive(false);
        choiceButton2.gameObject.SetActive(false);
        choiceButton3.gameObject.SetActive(false);

        Invoke(nameof(ShowNPCGreeting), 1.2f);
    }

    void ShowNPCGreeting()
    {
        speakerText.text = "Date";
        dialogueText.text = "Hi, how's it going?";

        ShowChoices(
            "Ask about their day",
            "Ask about their past",
            "Say goodbye"
        );
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
        speakerText.text = "Player";
        dialogueText.text = "How has your day been?";

        HideChoices();
        Invoke(nameof(ResponseOption1), 1.0f);
    }

    void ResponseOption1()
    {
        speakerText.text = "Date";
        dialogueText.text = "Pretty normal. Just walking around and getting some fresh air.";

        ShowChoices(
            "Ask about their job",
            "Ask about their past",
            "End conversation"
        );
    }

    void ChooseOption2()
    {
        speakerText.text = "Player";
        dialogueText.text = "Can you tell me a little about your past?";

        HideChoices();
        Invoke(nameof(ResponseOption2), 1.0f);
    }

    void ResponseOption2()
    {
        speakerText.text = "Date";
        dialogueText.text = "I moved around a lot, so I don't really stay in one place for long.";

        ShowChoices(
            "Ask why they moved so much",
            "Change the topic",
            "End conversation"
        );
    }

    void ChooseOption3()
    {
        speakerText.text = "Player";
        dialogueText.text = "I should get going. See you.";

        HideChoices();
        Invoke(nameof(EndDialogue), 1.0f);
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