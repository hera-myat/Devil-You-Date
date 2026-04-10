using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SuspicionSystem : MonoBehaviour
{
    [Header("Suspicion Settings")]
    public int suspicionLevel = 0;
    public int maxSuspicion = 4;

    [Header("UI")]
    public Image skeleton1;
    public Image skeleton2;
    public Image skeleton3;
    public TMP_Text endingText;

    [Header("Heartbeat Sound")]
    public AudioSource heartbeatAudioSource;
    public AudioClip heartbeatClip;
    private bool isHeartbeatPlaying = false;

    [Header("Ending Sequence")]
    public DateKillEndingSequence dateKillEndingSequence;

    private bool endingTriggered = false;

    void Start()
    {
        UpdateSkeletonUI();

        if (endingText != null)
            endingText.gameObject.SetActive(false);
    }

    void Update()
    {
        // test keys
        if (Input.GetKeyDown(KeyCode.P))
            IncreaseSuspicion();

        if (Input.GetKeyDown(KeyCode.O))
            DecreaseSuspicion();
    }

    public void IncreaseSuspicion()
    {
        if (endingTriggered)
            return;

        suspicionLevel++;

        if (suspicionLevel > maxSuspicion)
            suspicionLevel = maxSuspicion;

        UpdateSkeletonUI();

        if (suspicionLevel >= maxSuspicion)
        {
            TriggerEnding();
        }
    }

    public void DecreaseSuspicion()
    {
        if (endingTriggered)
            return;

        suspicionLevel--;

        if (suspicionLevel < 0)
            suspicionLevel = 0;

        UpdateSkeletonUI();
    }

    public void ResetSuspicion()
    {
        if (endingTriggered)
            return;

        suspicionLevel = 0;
        UpdateSkeletonUI();

        Debug.Log("Suspicion has been cleared through repentance.");
    }

    void UpdateSkeletonUI()
    {
        if (skeleton1 == null || skeleton2 == null || skeleton3 == null)
            return;

        skeleton1.gameObject.SetActive(false);
        skeleton2.gameObject.SetActive(false);
        skeleton3.gameObject.SetActive(false);

        skeleton1.color = Color.white;
        skeleton2.color = Color.white;
        skeleton3.color = Color.white;

        if (suspicionLevel >= 1)
            skeleton1.gameObject.SetActive(true);

        if (suspicionLevel >= 2)
            skeleton2.gameObject.SetActive(true);

        if (suspicionLevel >= 3)
            skeleton3.gameObject.SetActive(true);

        // Heartbeat logic
        if (suspicionLevel == 3 && !isHeartbeatPlaying)
        {
            PlayHeartbeat();
        }
        else if (suspicionLevel < 3 && isHeartbeatPlaying)
        {
            StopHeartbeat();
        }

        if (suspicionLevel >= 4)
        {
            skeleton1.gameObject.SetActive(true);
            skeleton2.gameObject.SetActive(true);
            skeleton3.gameObject.SetActive(true);

            skeleton1.color = Color.red;
            skeleton2.color = Color.red;
            skeleton3.color = Color.red;
        }
    }

    void TriggerEnding()
    {
        endingTriggered = true;
        StopHeartbeat();

        if (endingText != null)
        {
            endingText.gameObject.SetActive(false);
        }

        if (dateKillEndingSequence != null)
        {
            dateKillEndingSequence.StartEndingSequence();
        }

        Debug.Log("Ending triggered because suspicion reached level 4.");
    }

    public bool IsEndingTriggered()
    {
        return endingTriggered;
    }

    void PlayHeartbeat()
    {
        if (heartbeatAudioSource == null || heartbeatClip == null)
            return;

        heartbeatAudioSource.clip = heartbeatClip;
        heartbeatAudioSource.loop = true;
        heartbeatAudioSource.Play();

        isHeartbeatPlaying = true;
    }

    void StopHeartbeat()
    {
        if (heartbeatAudioSource == null)
            return;

        if (heartbeatAudioSource.isPlaying)
            heartbeatAudioSource.Stop();

        heartbeatAudioSource.loop = false;
        heartbeatAudioSource.clip = null;

        isHeartbeatPlaying = false;
    }
}