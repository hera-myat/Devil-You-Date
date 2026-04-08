using UnityEngine;
using TMPro;

public class SideQuestTimer : MonoBehaviour
{
    [Header("UI")]
    public GameObject timerPanel;
    public TextMeshProUGUI timerText;

    [Header("Timer Settings")]
    public float totalTime = 120f;

    [Header("Color Settings")]
    public Color normalColor = Color.white;
    public Color warningColor = new Color(1f, 0.5f, 0f);
    public Color dangerColor = Color.red;

    [Header("References")]
    public SuspicionSystem suspicionSystem;
    public SideQuestNPC questNPC;

    private float currentTime = 0f;
    private bool timerRunning = false;
    private bool timerFinished = false;

    void Start()
    {
        if (timerPanel != null)
            timerPanel.SetActive(false);
    }

    void Update()
    {
        if (!timerRunning)
            return;

        currentTime -= Time.deltaTime;

        if (currentTime < 0f)
            currentTime = 0f;

        UpdateTimerUI();

        if (currentTime <= 0f && !timerFinished)
        {
            timerFinished = true;
            timerRunning = false;

            if (suspicionSystem != null)
            {
                suspicionSystem.IncreaseSuspicion();
            }

            if (questNPC != null)
            {
                questNPC.diaryReturnedLate = true;
            }

            Debug.Log("Side quest timer ended. Suspicion increased by 1.");
            HideTimer();
        }
    }

    public void StartTimer()
    {
        currentTime = totalTime;
        timerRunning = true;
        timerFinished = false;

        if (questNPC != null)
        {
            questNPC.diaryReturnedLate = false;
        }

        if (timerPanel != null)
            timerPanel.SetActive(true);

        UpdateTimerUI();
        Debug.Log("Side quest timer started.");
    }

    public void StopTimer()
    {
        timerRunning = false;
        timerFinished = false;
        HideTimer();
        Debug.Log("Side quest timer stopped.");
    }

    public bool IsTimerRunning()
    {
        return timerRunning;
    }

    public bool HasTimerFinished()
    {
        return timerFinished;
    }

    void HideTimer()
    {
        if (timerPanel != null)
            timerPanel.SetActive(false);
    }

    void UpdateTimerUI()
    {
        if (timerText == null)
            return;

        int minutes = Mathf.FloorToInt(currentTime / 60f);
        int seconds = Mathf.FloorToInt(currentTime % 60f);

        timerText.text = minutes.ToString("00") + ":" + seconds.ToString("00");

        if (currentTime <= 30f)
        {
            timerText.color = dangerColor;
        }
        else if (currentTime <= 60f)
        {
            timerText.color = warningColor;
        }
        else
        {
            timerText.color = normalColor;
        }
    }
}