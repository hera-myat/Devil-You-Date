using UnityEngine;
using TMPro;

public class GarbageQuestManager : MonoBehaviour
{
    [Header("References")]
    public InventoryManager inventoryManager;
    public SuspicionSystem suspicionSystem;
    public DialogueManager dialogueManager;
    public DateEventManager dateEventManager;

    [Header("UI")]
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI resultText;

    [Header("Timer")]
    public float totalTime = 300f;
    public bool questStarted = false;
    public bool questCompleted = false;
    private float currentTime;

    [Header("Required Counts")]
    public int pizzaRequired = 7;
    public int bottleRequired = 2;
    public int metalRequired = 14;
    public int cardboardRequired = 7;
    public int cigaretteRequired = 3;

    [Header("Progress")]
    public int pizzaDone = 0;
    public int bottleDone = 0;
    public int metalDone = 0;
    public int cardboardDone = 0;
    public int cigaretteDone = 0;

    [Header("Reward")]
    public string rewardItemId = "trashclean";


    private bool introShown = false;
    private bool failureHandled = false;

    void Start()
    {
        currentTime = totalTime;

        if (timerText != null)
            timerText.gameObject.SetActive(false);

        if (resultText != null)
            resultText.gameObject.SetActive(false);
    }

    void Update()
    {
        if (!questStarted || questCompleted)
            return;

        currentTime -= Time.deltaTime;
        if (currentTime < 0f)
            currentTime = 0f;

        UpdateTimerUI();

        if (currentTime <= 0f && !failureHandled)
        {
            failureHandled = true;
            questStarted = false;

            if (timerText != null)
                timerText.gameObject.SetActive(false);

            if (suspicionSystem != null)
            {
                suspicionSystem.IncreaseSuspicion();
            }

            ShowMessage("You ran out of time.");
        }
    }

    public void EnterGarbageArea()
    {
        if (!introShown)
        {
            introShown = true;

            if (dialogueManager != null)
            {
                dialogueManager.ShowSideQuestSingleLine("", "Maybe I should clean these garbages around.");
            }
        }

        if (!questStarted && !questCompleted && !failureHandled)
        {
            questStarted = true;
            currentTime = totalTime;

            if (timerText != null)
                timerText.gameObject.SetActive(true);
        }
    }

    public void RegisterTrashThrown(string itemId)
    {
        if (questCompleted)
            return;

        if (itemId == "pizzabox")
            pizzaDone++;
        else if (itemId == "bottle")
            bottleDone++;
        else if (itemId == "metalcan" || itemId == "spraycan" || itemId == "sodacan")
            metalDone++;
        else if (itemId == "cigarette")
            cigaretteDone++;

        CheckCompletion();
    }

    public void RegisterCardboardCleaned()
    {
        if (questCompleted)
            return;

        cardboardDone++;
        CheckCompletion();
    }

    void CheckCompletion()
    {
        if (pizzaDone >= pizzaRequired &&
            bottleDone >= bottleRequired &&
            metalDone >= metalRequired &&
            cardboardDone >= cardboardRequired &&
            cigaretteDone >= cigaretteRequired)
        {
            questCompleted = true;
            questStarted = false;

            if (timerText != null)
                timerText.gameObject.SetActive(false);

            bool rewardAdded = false;

            if (inventoryManager != null)
            {
                if (inventoryManager.HasItem(rewardItemId))
                {
                    rewardAdded = true;
                }
                else
                {
                    rewardAdded = inventoryManager.AddItem(rewardItemId);
                    if (!rewardAdded)
                    {
                        Debug.Log("Could not add trashclean reward because inventory is full.");
                    }
                }
            }

            if (rewardAdded)
            {
                if (GameProgressManager.Instance != null)
                {
                    GameProgressManager.Instance.hasTrashReward = true;
                }

                if (dateEventManager != null)
                {
                    dateEventManager.StartReturnEvent(rewardItemId);
                }
            }

            ShowMessage("You finished cleaning the area.");
            Debug.Log("Trash quest completed. Date return event started.");
        }
    }

    void UpdateTimerUI()
    {
        if (timerText == null)
            return;

        int minutes = Mathf.FloorToInt(currentTime / 60f);
        int seconds = Mathf.FloorToInt(currentTime % 60f);
        timerText.text = $"Clean Up Time: {minutes:00}:{seconds:00}";

        if (currentTime <= 30f)
            timerText.color = Color.red;
        else if (currentTime <= 60f)
            timerText.color = new Color(1f, 0.5f, 0f);
        else
            timerText.color = Color.white;
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
}