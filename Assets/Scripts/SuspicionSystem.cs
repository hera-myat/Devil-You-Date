using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SuspicionSystem : MonoBehaviour
{
    [Header("Suspicion Settings")]
    public int suspicionLevel = 0;
    public int maxSuspicion = 5;

    [Header("UI")]
    public Image suspicionIcon;
    public TMP_Text endingText;

    private bool endingTriggered = false;

    void Start()
    {
        UpdateSuspicionIcon();

        if (endingText != null)
        {
            endingText.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        // TEST KEY
        if (Input.GetKeyDown(KeyCode.P))
        {
            IncreaseSuspicion();
        }

        // OPTIONAL TEST KEY
        if (Input.GetKeyDown(KeyCode.O))
        {
            DecreaseSuspicion();
        }
    }

    public void IncreaseSuspicion()
    {
        if (endingTriggered) return;

        suspicionLevel++;

        if (suspicionLevel > maxSuspicion)
            suspicionLevel = maxSuspicion;

        UpdateSuspicionIcon();

        if (suspicionLevel >= maxSuspicion)
        {
            TriggerEnding();
        }
    }

    public void DecreaseSuspicion()
    {
        if (endingTriggered) return;

        suspicionLevel--;

        if (suspicionLevel < 0)
            suspicionLevel = 0;

        UpdateSuspicionIcon();
    }

    public void ResetSuspicion()
    {
        if (endingTriggered)
            return;

        suspicionLevel = 0;
        UpdateSuspicionIcon();

        Debug.Log("Suspicion has been cleared through repentance.");
    }

    void UpdateSuspicionIcon()
    {
        if (suspicionIcon == null) return;

        Color color = suspicionIcon.color;

        if (suspicionLevel == 0)
        {
            color = Color.white;
            color.a = 0f;
        }
        else if (suspicionLevel >= 1 && suspicionLevel <= 4)
        {
            color = Color.white;
            color.a = suspicionLevel / 4f;
        }
        else if (suspicionLevel == 5)
        {
            color = Color.red;
            color.a = 1f;
        }

        suspicionIcon.color = color;
    }

    void TriggerEnding()
    {
        endingTriggered = true;

        if (endingText != null)
        {
            endingText.gameObject.SetActive(true);
            endingText.text = "We triggered one of the endings.";
        }

        Debug.Log("Ending triggered because suspicion reached level 5.");
    }
}