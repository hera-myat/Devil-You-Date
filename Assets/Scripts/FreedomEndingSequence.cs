using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class FreedomEndingSequence : MonoBehaviour
{
    [Header("UI")]
    public Image fadePanel;
    public TextMeshProUGUI sequenceText;

    [Header("References")]
    public DateEventManager dateEventManager;

    [Header("Player")]
    public Transform playerRoot;
    public Transform spawnPoint;
    public MonoBehaviour playerMovement;
    public MonoBehaviour playerLook;
    public CharacterController characterController;

    [Header("After Teleport")]
    public GameObject blockBackTrigger;
    public GameObject invisibleWall1;
    public GameObject invisibleWall2;

    [Header("Door")]
    public DoorEndingTrigger exitDoor;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip goodEndingIntroBGM;

    public AudioSource spawnAreaBGM;
    public AudioSource churchAreaBGM;

    [Header("Debug")]
    public bool debugLogs = true;

    private bool isPlaying = false;

    private string[] lines =
    {
        "You see your date standing in front of you.",
        "You give a small wave.",
        "\"It's surprising... that you made this choice.\"",
        "\"It's a shame. I was hoping to keep you a little longer.\"",
        "\"But it seems I've lost a soul from my place.\"",
        "...",
        "\"But you're free to go now.\"",
        "\"...I'm free to go?\"",
        "\"Yes.\"",
        "\"You're free to go.\"",
        "...",
        "Flames begin to flicker around him.",
        "You freeze.",
        "Something about this isn't right.",
        "Strange symbols start forming in the air.",
        "They twist... watching you.",
        "\"...He's not human.\"",
        "\"...He's a demon.\"",
        "\"I need to leave.\""
    };

    void Start()
    {
        if (blockBackTrigger != null)
            blockBackTrigger.SetActive(false);

        if (invisibleWall1 != null)
            invisibleWall1.SetActive(false);

        if (invisibleWall2 != null)
            invisibleWall2.SetActive(false);

        if (sequenceText != null)
            sequenceText.gameObject.SetActive(false);

        if (fadePanel != null)
            fadePanel.gameObject.SetActive(false);
    }

    public void StartFreedomEnding()
    {
        if (isPlaying)
            return;

        if (debugLogs)
            Debug.Log("FreedomEndingSequence: StartFreedomEnding called.");

        StartCoroutine(PlaySequence());
    }

    IEnumerator PlaySequence()
    {
        isPlaying = true;

        if (dateEventManager != null)
            dateEventManager.LockDateSystem();

        if (playerMovement != null)
            playerMovement.enabled = false;

        if (playerLook != null)
            playerLook.enabled = false;

        // Stop church BGM first
        AudioSource[] allSources = FindObjectsOfType<AudioSource>();

        foreach (AudioSource src in allSources)
        {
            src.Stop();
        }


        // Play first good ending BGM
        if (audioSource != null && goodEndingIntroBGM != null)
        {
            audioSource.Stop();
            audioSource.clip = goodEndingIntroBGM;
            audioSource.loop = true;
            audioSource.Play();
        }

        yield return StartCoroutine(FadeToBlack());

        if (sequenceText != null)
            sequenceText.gameObject.SetActive(true);

        foreach (string line in lines)
        {
            yield return StartCoroutine(ShowLine(line));
        }

        if (sequenceText != null)
            sequenceText.gameObject.SetActive(false);

        if (debugLogs)
            Debug.Log("FreedomEndingSequence: Dialogue finished. Teleporting now.");

        yield return StartCoroutine(TeleportPlayerCoroutine());

        if (playerMovement != null)
            playerMovement.enabled = true;

        if (playerLook != null)
            playerLook.enabled = true;

        isPlaying = false;
    }

    IEnumerator TeleportPlayerCoroutine()
    {
        if (playerRoot == null || spawnPoint == null)
        {
            Debug.LogWarning("FreedomEndingSequence: playerRoot or spawnPoint is missing.");
            yield break;
        }

        if (debugLogs)
        {
            Debug.Log("FreedomEndingSequence: Before teleport");
            Debug.Log("Current player position: " + playerRoot.position);
            Debug.Log("Spawn point position: " + spawnPoint.position);
        }

        if (characterController != null)
            characterController.enabled = false;

        yield return null;

        playerRoot.position = spawnPoint.position;
        playerRoot.rotation = spawnPoint.rotation;

        yield return null;

        if (characterController != null)
            characterController.enabled = true;

        if (debugLogs)
        {
            Debug.Log("FreedomEndingSequence: After teleport");
            Debug.Log("New player position: " + playerRoot.position);
        }

        if (blockBackTrigger != null)
            blockBackTrigger.SetActive(true);

        if (invisibleWall1 != null)
            invisibleWall1.SetActive(true);

        if (invisibleWall2 != null)
            invisibleWall2.SetActive(true);

        if (exitDoor != null)
            exitDoor.EnableExit();

        yield return StartCoroutine(FadeFromBlack());
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

        float t = 0f;
        Color c = fadePanel.color;

        while (t < 1f)
        {
            t += Time.deltaTime;
            c.a = Mathf.Lerp(0f, 1f, t);
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

        float t = 0f;
        Color c = fadePanel.color;

        while (t < 1f)
        {
            t += Time.deltaTime;
            c.a = Mathf.Lerp(1f, 0f, t);
            fadePanel.color = c;
            yield return null;
        }

        fadePanel.gameObject.SetActive(false);
    }
}