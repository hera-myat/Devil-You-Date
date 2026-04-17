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

    [Header("Blood Effect")]
    public Image bloodImage;
    public float bloodFadeIn = 0.15f;
    public float bloodHold = 0.4f;
    public float bloodFadeOut = 0.6f;

    [Header("Timing")]
    public float fadeDuration = 1.5f;
    public float continueHintDelay = 1.5f;
    public float waitBeforeLoadMenu = 0.5f;

    [Header("References")]
    public DateEventManager dateEventManager;

    [Header("Optional Player Lock")]
    public MonoBehaviour playerMovement;
    public MonoBehaviour playerLook;

    [Header("Monster")]
    public GameObject monsterPrefab;
    public Transform playerCamera;
    public float spawnDistance = 1.8f;
    public float heightOffset = -1.2f;

    [Header("Monster Audio")]
    public AudioClip monsterScreamClip;
    public AudioSource audioSource;

    [Header("Optional Audio")]
    public AudioClip endingSting;

    public AudioSource spawnAreaBGM;

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

    void Start()
    {
        if (bloodImage != null)
        {
            Color c = bloodImage.color;
            c.a = 0f;
            bloodImage.color = c;
        }
    }

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
            dateEventManager.LockDateSystem();

        if (playerMovement != null)
            playerMovement.enabled = false;

        if (playerLook != null)
            playerLook.enabled = false;

        if (spawnAreaBGM != null)
        {
            spawnAreaBGM.Stop();
            spawnAreaBGM.enabled = false;
        }

        if (audioSource != null && endingSting != null)
            audioSource.PlayOneShot(endingSting);

        yield return StartCoroutine(FadeToBlack());

        GameObject monster = SpawnMonsterInFront();

        SetFadeAlpha(0f);

        if (monster != null)
        {
            PlayMonsterSound(monster);

            if (bloodImage != null)
                StartCoroutine(PlayBloodEffect());

            StartCoroutine(MonsterLunge(monster));
        }

        yield return new WaitForSeconds(4f);
        yield return new WaitForSeconds(0.5f);

        yield return StartCoroutine(FadeToBlack());

        if (sequenceText != null)
            sequenceText.gameObject.SetActive(true);

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

    IEnumerator PlayBloodEffect()
    {
        yield return FadeBlood(0f, 1f, bloodFadeIn);
        yield return new WaitForSeconds(bloodHold);
        yield return FadeBlood(1f, 0f, bloodFadeOut);
    }

    IEnumerator FadeBlood(float start, float end, float duration)
    {
        float t = 0f;
        Color c = bloodImage.color;

        while (t < duration)
        {
            t += Time.deltaTime;
            c.a = Mathf.Lerp(start, end, t / duration);
            bloodImage.color = c;
            yield return null;
        }

        c.a = end;
        bloodImage.color = c;
    }

    void PlayMonsterSound(GameObject monster)
    {
        if (monsterScreamClip == null)
            return;

        AudioSource monsterAudio = monster.GetComponent<AudioSource>();

        if (monsterAudio != null)
            monsterAudio.PlayOneShot(monsterScreamClip);
        else if (audioSource != null)
            audioSource.PlayOneShot(monsterScreamClip);
    }

    GameObject SpawnMonsterInFront()
    {
        if (monsterPrefab == null || playerCamera == null)
            return null;

        Vector3 spawnPos = playerCamera.position + playerCamera.forward * spawnDistance;
        spawnPos.y = playerCamera.position.y + heightOffset;

        Vector3 lookDir = (playerCamera.position - spawnPos).normalized;
        Quaternion rotation = Quaternion.LookRotation(lookDir);

        GameObject monster = Instantiate(monsterPrefab, spawnPos, rotation);

        monster.transform.position += playerCamera.forward * 0.3f;

        Animator anim = monster.GetComponent<Animator>();
        if (anim != null)
            anim.SetTrigger("Attack");

        return monster;
    }

    IEnumerator MonsterLunge(GameObject monster)
    {
        float duration = 0.4f;
        float timer = 0f;

        Vector3 start = monster.transform.position;
        Vector3 end = start + playerCamera.forward * 0.8f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            monster.transform.position = Vector3.Lerp(start, end, timer / duration);
            yield return null;
        }
    }

    IEnumerator ShowLineAndWaitForClick(string line)
    {
        sequenceText.text = line;

        yield return new WaitForSeconds(continueHintDelay);

        sequenceText.text = line + "\n\n<size=70%><color=#CCCCCC>Left click to continue</color></size>";

        yield return new WaitUntil(() => Input.GetMouseButtonDown(0));
    }

    IEnumerator FadeToBlack()
    {
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

    void SetFadeAlpha(float alpha)
    {
        Color c = fadePanel.color;
        c.a = alpha;
        fadePanel.color = c;
    }
}
