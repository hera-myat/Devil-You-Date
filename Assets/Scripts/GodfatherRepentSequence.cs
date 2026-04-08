using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class GodfatherRepentSequence : MonoBehaviour
{
    [Header("UI")]
    public Image fadePanel;
    public TextMeshProUGUI sequenceText;

    [Header("Timing")]
    public float fadeDuration = 1.5f;
    public float continueHintDelay = 2f;

    [Header("Optional Player Lock")]
    public MonoBehaviour playerMovement;
    public MonoBehaviour playerLook;

    public SuspicionSystem suspicionSystem;
    public bool repentCompleted = false;

    private string[] lines =
    {
        "You close your eyes and press your hands together.",
        "Why are you repenting?",
        "Because you took lives.",
        "They deserved it.",
        "...Did they?",
        "You hear the Godfather murmuring softly.",
        "\"He can hear you.\"",
        "\"When a person has taken lives...\"",
        "\"What else can they do?\"",
        "...",
        "\"I don't know.\"",
        "\"...\"",
        "\"I should repent.\""
    };

    private bool isPlaying = false;

    public void StartRepentSequence()
    {
        if (!isPlaying)
        {
            StartCoroutine(PlaySequence());
        }
    }

    IEnumerator PlaySequence()
    {
        isPlaying = true;

        if (playerMovement != null)
            playerMovement.enabled = false;

        if (playerLook != null)
            playerLook.enabled = false;

        yield return StartCoroutine(FadeToBlack());

        if (sequenceText != null)
            sequenceText.gameObject.SetActive(true);

        for (int i = 0; i < lines.Length; i++)
        {
            yield return StartCoroutine(ShowLineAndWaitForClick(lines[i]));
        }

        if (sequenceText != null)
        {
            sequenceText.text = "";
            sequenceText.gameObject.SetActive(false);
        }

        yield return new WaitForSeconds(0.2f);
        yield return StartCoroutine(FadeFromBlack());

        if (playerMovement != null)
            playerMovement.enabled = true;

        if (playerLook != null)
            playerLook.enabled = true;

        if (suspicionSystem != null && suspicionSystem.suspicionLevel < 5)
        {
            suspicionSystem.ResetSuspicion();
        }

        repentCompleted = true;
        isPlaying = false;


    }

    IEnumerator ShowLineAndWaitForClick(string line)
    {
        if (sequenceText == null)
            yield break;

        sequenceText.text = line;

        yield return new WaitForSeconds(continueHintDelay);

        sequenceText.text = line + "\n\n<size=70%><color=#CCCCCC>Left click to continue</color></size>";

        yield return new WaitUntil(() => Input.GetMouseButtonDown(0));

        yield return null;
    }

    IEnumerator FadeToBlack()
    {
        if (fadePanel == null)
            yield break;

        fadePanel.gameObject.SetActive(true);

        Color c = fadePanel.color;
        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            c.a = Mathf.Lerp(0f, 1f, timer / fadeDuration);
            fadePanel.color = c;
            yield return null;
        }

        c.a = 1f;
        fadePanel.color = c;
    }

    IEnumerator FadeFromBlack()
    {
        if (fadePanel == null)
            yield break;

        Color c = fadePanel.color;
        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            c.a = Mathf.Lerp(1f, 0f, timer / fadeDuration);
            fadePanel.color = c;
            yield return null;
        }

        c.a = 0f;
        fadePanel.color = c;
        fadePanel.gameObject.SetActive(false);
    }
}