using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class NormalEndingSequence : MonoBehaviour
{
    [Header("UI")]
    public Image fadePanel;
    public TextMeshProUGUI sequenceText;

    [Header("Player Lock")]
    public MonoBehaviour playerMovement;
    public MonoBehaviour playerLook;

    [Header("Teleport")]
    public Transform playerRoot;
    public Transform asylumSpawnPoint;
    public CharacterController characterController;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip preTeleportBGM;
    public AudioClip asylumBGM;
    public AudioSource spawnAreaBGM;

    [Header("Timing")]
    public float preLineDelay = 1.5f;
    public float lookAroundDuration = 8f;
    public float endWaitTime = 2f;

    private bool isPlaying = false;

    private string[] linesBeforeTeleport =
    {
        "You sit down across from him.",
        "...You took your time.",
        "...Did you find what you were looking for?",
        "Your hand shifts slightly.",
        "The weight feels... natural.",
        "...Strange, isn't it?",
        "...How some things feel familiar... even when they shouldn't.",
        "Fragments flash through your mind.",
        "...Those places you visited... they weren't random.",
        "...You've been trying to understand it.",
        "...Not what happened... but why it didn't feel wrong.",
        "...I don't judge you.",
        "...I only show people what they already are.",
        "...You were never lost.",
        "...You were just... remembering."
    };

    private string[] linesAfterTeleport =
    {
        "You open your eyes.",
        "White ceiling.",
        "Cold air.",
        "You sit up slowly.",
        "Everything feels quiet.",
        "...So this is where I ended up.",
        "...It feels... familiar."
    };

    public void StartNormalEnding()
    {
        if (isPlaying) return;
        StartCoroutine(PlaySequence());
    }

    void DisableAllOtherAudio()
    {
        AudioSource[] allSources = FindObjectsOfType<AudioSource>(true);

        foreach (AudioSource src in allSources)
        {
            if (src != audioSource)
            {
                src.Stop();
                src.enabled = false;
            }
        }
    }

    IEnumerator KeepOtherAudioDisabled()
    {
        while (isPlaying)
        {
            AudioSource[] allSources = FindObjectsOfType<AudioSource>(true);

            foreach (AudioSource src in allSources)
            {
                if (src != audioSource)
                {
                    src.Stop();
                    src.enabled = false;
                }
            }

            yield return new WaitForSeconds(0.2f);
        }
    }

    IEnumerator PlaySequence()
    {
        isPlaying = true;

        if (playerMovement != null) playerMovement.enabled = false;
        if (playerLook != null) playerLook.enabled = false;

        if (spawnAreaBGM != null)
        {
            spawnAreaBGM.Stop();
            spawnAreaBGM.enabled = false;
        }

        DisableAllOtherAudio();
        StartCoroutine(KeepOtherAudioDisabled());

        if (audioSource != null && preTeleportBGM != null)
        {
            audioSource.enabled = true;
            audioSource.clip = preTeleportBGM;
            audioSource.loop = true;
            audioSource.Play();
        }

        // FIRST: fade to black and show the first sequence
        yield return StartCoroutine(FadeToBlack());

        if (sequenceText != null)
            sequenceText.gameObject.SetActive(true);

        foreach (string line in linesBeforeTeleport)
        {
            yield return StartCoroutine(ShowLine(line));
        }

        if (sequenceText != null)
            sequenceText.gameObject.SetActive(false);

        // SECOND: teleport to asylum
        yield return StartCoroutine(TeleportToAsylum());

        // THIRD: switch to asylum BGM
        if (audioSource != null && asylumBGM != null)
        {
            audioSource.Stop();
            audioSource.clip = asylumBGM;
            audioSource.loop = true;
            audioSource.Play();
        }

        // FOURTH: fade back in and let player look around

        yield return new WaitForSeconds(lookAroundDuration);

        // FIFTH: final asylum sequence
        yield return StartCoroutine(FadeToBlack());
        yield return new WaitForSeconds(2f);

        if (sequenceText != null)
            sequenceText.gameObject.SetActive(true);

        foreach (string line in linesAfterTeleport)
        {
            yield return StartCoroutine(ShowLine(line));
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        yield return new WaitForSeconds(endWaitTime);

        isPlaying = false;
        SceneManager.LoadScene("MainMenu");
    }

    IEnumerator ShowLine(string line)
    {
        if (sequenceText == null)
            yield break;

        sequenceText.text = line;

        yield return new WaitForSeconds(preLineDelay);

        sequenceText.text += "\n\n<size=70%><color=#CCCCCC>Click to continue</color></size>";

        yield return new WaitUntil(() => Input.GetMouseButtonDown(0));
    }

    IEnumerator TeleportToAsylum()
    {
        if (playerRoot == null || asylumSpawnPoint == null)
            yield break;

        if (characterController != null)
            characterController.enabled = false;

        yield return null;

        playerRoot.position = asylumSpawnPoint.position;

        yield return null;

        if (characterController != null)
            characterController.enabled = true;
    }

    IEnumerator FadeToBlack()
    {
        if (fadePanel == null)
            yield break;

        fadePanel.gameObject.SetActive(true);

        Color c = fadePanel.color;
        float timer = 0f;

        while (timer < 1f)
        {
            timer += Time.deltaTime;
            c.a = Mathf.Lerp(0f, 1f, timer);
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

        fadePanel.gameObject.SetActive(true);

        Color c = fadePanel.color;
        float timer = 0f;

        while (timer < 1f)
        {
            timer += Time.deltaTime;
            c.a = Mathf.Lerp(1f, 0f, timer);
            fadePanel.color = c;
            yield return null;
        }

        c.a = 0f;
        fadePanel.color = c;
        fadePanel.gameObject.SetActive(false);
    }
}