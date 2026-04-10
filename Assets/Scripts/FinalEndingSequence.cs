using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class FinalEndingSequence : MonoBehaviour
{
    [Header("UI")]
    public Image fadePanel;
    public TextMeshProUGUI sequenceText;

    [Header("Player Lock")]
    public MonoBehaviour playerMovement;
    public MonoBehaviour playerLook;

    private bool isPlaying = false;

    private string[] lines =
    {
        "You reach the door.",
        "Your hand trembles as you walk through.",
        "For a moment… you feel like something is watching you.",
        "But nothing stops you.",
        "You step outside.",
        "You made it out.",
        "...for now."
    };

    public void StartFinalEnding()
    {
        if (isPlaying)
            return;

        StartCoroutine(PlaySequence());
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
            yield return StartCoroutine(ShowLine(lines[i]));
        }

        isPlaying = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        
        SceneManager.LoadScene("MainMenu");
    }

    IEnumerator ShowLine(string line)
    {
        if (sequenceText == null)
            yield break;

        sequenceText.text = line;

        yield return new WaitForSeconds(1.5f);

        sequenceText.text += "\n\n<size=70%><color=#CCCCCC>Click to continue</color></size>";

        yield return new WaitUntil(() => Input.GetMouseButtonDown(0));
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
}