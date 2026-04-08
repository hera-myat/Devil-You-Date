using UnityEngine;

public class DateMeetingSuspicionTimer : MonoBehaviour
{
    [Header("References")]
    public SuspicionSystem suspicionSystem;

    [Header("Settings")]
    public float suspicionInterval = 60f;

    private float timer = 0f;

    void Update()
    {
        if (GameProgressManager.Instance == null)
            return;

        if (GameProgressManager.Instance.hasMetDate)
            return;

        timer += Time.deltaTime;

        if (timer >= suspicionInterval)
        {
            timer = 0f;

            if (suspicionSystem != null)
            {
                suspicionSystem.IncreaseSuspicion();
                Debug.Log("Suspicion increased because player still has not met the Date.");
            }
        }
    }
}