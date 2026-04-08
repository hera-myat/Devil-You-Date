using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class SearchableObject : MonoBehaviour
{
    [Header("Search Lock")]
    public bool canSearch = true;

    [Header("UI")]
    public GameObject searchPrompt;
    public Slider progressSlider;
    public TextMeshProUGUI resultText;

    [Header("Search Settings")]
    public float searchTime = 2f;

    [Header("Reward")]
    public InventoryManager inventoryManager;
    public string rewardItemId = "coin";

    [Header("Suspicion")]
    public DateDialogue dateDialogue;
    public SuspicionSystem suspicionSystem;

    private bool playerInRange = false;
    private bool alreadySearched = false;
    private bool isSearching = false;
    private bool suspicionApplied = false;

    void Start()
    {
        if (searchPrompt != null)
            searchPrompt.SetActive(false);

        if (progressSlider != null)
        {
            progressSlider.gameObject.SetActive(false);
            progressSlider.value = 0f;
        }

        if (resultText != null)
            resultText.gameObject.SetActive(false);
    }

    void Update()
    {
        if (playerInRange && canSearch && Input.GetKeyDown(KeyCode.E) && !isSearching)
        {
            if (!alreadySearched)
            {
                StartCoroutine(SearchRoutine());
            }
            else
            {
                ShowMessage("There is nothing else here.");
            }
        }
    }

    IEnumerator SearchRoutine()
    {
        isSearching = true;

        if (searchPrompt != null)
            searchPrompt.SetActive(false);

        if (progressSlider != null)
        {
            progressSlider.gameObject.SetActive(true);
            progressSlider.value = 0f;
        }

        float timer = 0f;

        while (timer < searchTime)
        {
            timer += Time.deltaTime;

            if (progressSlider != null)
                progressSlider.value = timer / searchTime;

            yield return null;
        }

        alreadySearched = true;
        isSearching = false;

        if (progressSlider != null)
        {
            progressSlider.gameObject.SetActive(false);
            progressSlider.value = 0f;
        }

        bool gotReward = false;

        if (inventoryManager != null)
            gotReward = inventoryManager.AddItem(rewardItemId);

        if (dateDialogue != null && !dateDialogue.hasTalkedToDate && !suspicionApplied && suspicionSystem != null)
        {
            suspicionSystem.IncreaseSuspicion();
            suspicionApplied = true;
        }

        if (gotReward)
            ShowMessage("You searched the trash can and found a coin, maybe you can use it for the vending machine around you.");
        else
            ShowMessage("You found a coin for the vending machine, but your inventory is full.");
    }

    void ShowMessage(string message)
    {
        if (resultText != null)
        {
            resultText.text = message;
            resultText.gameObject.SetActive(true);
            CancelInvoke(nameof(HideMessage));
            Invoke(nameof(HideMessage), 3f);
        }
    }

    void HideMessage()
    {
        if (resultText != null)
            resultText.gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;

            if (canSearch && searchPrompt != null && !alreadySearched && !isSearching)
                searchPrompt.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;

            if (searchPrompt != null)
                searchPrompt.SetActive(false);

            if (progressSlider != null)
            {
                progressSlider.gameObject.SetActive(false);
                progressSlider.value = 0f;
            }

            isSearching = false;
            StopAllCoroutines();
        }
    }

    public void UnlockSearch()
    {
        canSearch = true;

        if (playerInRange && !alreadySearched && !isSearching && searchPrompt != null)
            searchPrompt.SetActive(true);
    }
}