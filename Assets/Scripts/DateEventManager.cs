using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class DateEventManager : MonoBehaviour
{
    [Header("References")]
    public SuspicionSystem suspicionSystem;
    public TextMeshProUGUI returnTimerText;

    [Header("Settings")]
    public float defaultReturnDuration = 90f;

    private string currentEventId = "";
    private float currentTimer = 0f;
    private bool timerRunning = false;
    private bool currentEventLate = false;
    private bool suspicionAppliedForCurrentEvent = false;
    private bool systemLocked = false;

    private HashSet<string> playedEvents = new HashSet<string>();

    void Start()
    {
        if (returnTimerText != null)
            returnTimerText.gameObject.SetActive(false);
    }

    void Update()
    {
        if (systemLocked)
            return;

        if (!timerRunning)
            return;

        currentTimer -= Time.deltaTime;
        if (currentTimer < 0f)
            currentTimer = 0f;

        UpdateTimerUI();

        if (currentTimer <= 0f)
        {
            timerRunning = false;
            currentEventLate = true;

            if (!suspicionAppliedForCurrentEvent && suspicionSystem != null)
            {
                suspicionSystem.IncreaseSuspicion();
                suspicionAppliedForCurrentEvent = true;
            }

            if (returnTimerText != null)
                returnTimerText.gameObject.SetActive(false);
        }
    }

    public void StartReturnEvent(string eventId, float duration = -1f)
    {
        if (systemLocked)
            return;

        if (string.IsNullOrEmpty(eventId))
            return;

        currentEventId = eventId;
        currentTimer = duration > 0f ? duration : defaultReturnDuration;
        timerRunning = true;
        currentEventLate = false;
        suspicionAppliedForCurrentEvent = false;

        if (returnTimerText != null)
            returnTimerText.gameObject.SetActive(true);

        UpdateTimerUI();
    }

    public bool HasPendingEvent()
    {
        if (systemLocked)
            return false;

        return !string.IsNullOrEmpty(currentEventId) && !HasEventPlayed(currentEventId);
    }

    public string GetCurrentEventId()
    {
        return currentEventId;
    }

    public bool IsCurrentEventLate()
    {
        return currentEventLate;
    }

    public void PlayerReturnedForCurrentEvent()
    {
        timerRunning = false;

        if (returnTimerText != null)
            returnTimerText.gameObject.SetActive(false);
    }

    public bool HasEventPlayed(string eventId)
    {
        return playedEvents.Contains(eventId);
    }

    public void MarkCurrentEventPlayed()
    {
        if (!string.IsNullOrEmpty(currentEventId))
        {
            playedEvents.Add(currentEventId);
            currentEventId = "";
            timerRunning = false;
            currentEventLate = false;
            suspicionAppliedForCurrentEvent = false;

            if (returnTimerText != null)
                returnTimerText.gameObject.SetActive(false);
        }
    }

    public void StopAllReturnEvents()
    {
        currentEventId = "";
        currentTimer = 0f;
        timerRunning = false;
        currentEventLate = false;
        suspicionAppliedForCurrentEvent = false;

        if (returnTimerText != null)
            returnTimerText.gameObject.SetActive(false);

        Debug.Log("DateEventManager: All return events stopped.");
    }

    public void LockDateSystem()
    {
        StopAllReturnEvents();
        systemLocked = true;
        Debug.Log("DateEventManager: Date system locked.");
    }

    public void UnlockDateSystem()
    {
        systemLocked = false;
        Debug.Log("DateEventManager: Date system unlocked.");
    }

    void UpdateTimerUI()
    {
        if (returnTimerText == null)
            return;

        int minutes = Mathf.FloorToInt(currentTimer / 60f);
        int seconds = Mathf.FloorToInt(currentTimer % 60f);
        returnTimerText.text = $"Return to Date: {minutes:00}:{seconds:00}";

        if (currentTimer <= 30f)
            returnTimerText.color = Color.red;
        else
            returnTimerText.color = Color.white;
    }
}