using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class DateKillEndingSequence : MonoBehaviour
{
    [Header("UI")]
    public Image fadePanel;
    public TextMeshProUGUI sequenceText;

    [Header("Timing")]
    public float fadeDuration = 1.5f;
    public float continueHintDelay = 1.5f;
    public float waitBeforeLoadMenu = 0.5f;

    [Header("References")]
    public DateEventManager dateEventManager;

    [Header("Optional Player Lock")]
    public MonoBehaviour playerMovement;
    public MonoBehaviour playerLook;

    [Header("Optional Audio")]
    public AudioSource audioSource;
    public AudioClip endingSting;

    [Header("Scene")]
    public string mainMenuSceneName = "MainMenu";

    [Header("Ending State")]
    public bool endingCompleted = false;

    private bool isPlaying = false;

    private string[] lines =
    {
        "A sharp pain suddenly crushes your chest.",
        "Your knees weaken. Breathing becomes impossible.",
        "As darkness creeps into your vision, you see your date standing before you.",
        "\"I gave you a chance.\"",
        "\"But you refused it.\"",
        "\"You reached for the wrong ending.\"",
        "\"Congratulations.\"",
        "\"This is where your story ends.\""
    };

    public void StartEndingSequence()
    {
        if (isPlaying)
            return;

        StartCoroutine(PlaySequence());
    }

    IEnumerator PlaySequence()
    {
        isPlaying = true;

        if (dateEventManager != null)
        {
            dateEventManager.LockDateSystem();
        }

        if (playerMovement != null)
            playerMovement.enabled = false;

        if (playerLook != null)
            playerLook.enabled = false;

        if (audioSource != null && endingSting != null)
        {
            audioSource.PlayOneShot(endingSting);
        }

        yield return StartCoroutine(FadeToBlack());

        if (sequenceText != null)
        {
            sequenceText.gameObject.SetActive(true);
        }

        for (int i = 0; i < lines.Length; i++)
        {
            yield return StartCoroutine(ShowLineAndWaitForClick(lines[i]));
        }

        endingCompleted = true;
        isPlaying = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        yield return new WaitForSeconds(waitBeforeLoadMenu);

        SceneManager.LoadScene(mainMenuSceneName);
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
}