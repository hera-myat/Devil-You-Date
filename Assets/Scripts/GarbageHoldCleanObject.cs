using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GarbageHoldCleanObject : MonoBehaviour
{
    [Header("References")]
    public GarbageQuestManager garbageQuestManager;
    public GameObject interactPrompt;
    public Slider holdProgressBar;
    public TextMeshProUGUI resultText;

    [Header("Settings")]
    public float holdDuration = 4f;

    [Header("Sound Effects")]
    public AudioSource audioSource;
    public AudioClip holdLoopClip;
    public AudioClip finishClip;

    private bool playerInRange = false;
    private bool cleaned = false;
    private float holdTimer = 0f;
    private bool isPlayingHoldSound = false;

    void Start()
    {
        if (interactPrompt != null)
            interactPrompt.SetActive(false);

        if (holdProgressBar != null)
        {
            holdProgressBar.gameObject.SetActive(false);
            holdProgressBar.value = 0f;
        }

        if (resultText != null)
            resultText.gameObject.SetActive(false);

        if (audioSource != null)
        {
            audioSource.loop = false;
            audioSource.playOnAwake = false;
        }
    }

    void Update()
    {
        if (!playerInRange || cleaned)
            return;

        if (Input.GetKey(KeyCode.E))
        {
            holdTimer += Time.deltaTime;

            if (holdProgressBar != null)
            {
                holdProgressBar.gameObject.SetActive(true);
                holdProgressBar.value = holdTimer / holdDuration;
            }

            PlayHoldSound();

            if (holdTimer >= holdDuration)
            {
                CleanObject();
            }
        }
        else
        {
            ResetHoldProgress();
        }
    }

    void CleanObject()
    {
        cleaned = true;
        StopHoldSound();

        if (finishClip != null && audioSource != null)
        {
            audioSource.PlayOneShot(finishClip);
        }

        if (garbageQuestManager != null)
            garbageQuestManager.RegisterCardboardCleaned();

        if (interactPrompt != null)
            interactPrompt.SetActive(false);

        if (holdProgressBar != null)
        {
            holdProgressBar.value = 0f;
            holdProgressBar.gameObject.SetActive(false);
        }

        gameObject.SetActive(false);
    }

    void ResetHoldProgress()
    {
        holdTimer = 0f;
        StopHoldSound();

        if (holdProgressBar != null)
        {
            holdProgressBar.value = 0f;
            holdProgressBar.gameObject.SetActive(false);
        }
    }

    void PlayHoldSound()
    {
        if (audioSource == null || holdLoopClip == null || isPlayingHoldSound)
            return;

        audioSource.clip = holdLoopClip;
        audioSource.loop = true;
        audioSource.Play();
        isPlayingHoldSound = true;
    }

    void StopHoldSound()
    {
        if (audioSource == null)
            return;

        if (audioSource.isPlaying)
            audioSource.Stop();

        isPlayingHoldSound = false;
        audioSource.loop = false;
        audioSource.clip = null;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player") || cleaned)
            return;

        playerInRange = true;

        if (interactPrompt != null)
            interactPrompt.SetActive(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        playerInRange = false;
        ResetHoldProgress();

        if (interactPrompt != null)
            interactPrompt.SetActive(false);
    }
}