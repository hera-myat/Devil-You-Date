using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SearchableObject : MonoBehaviour
{
    [Header("UI")]
    public GameObject searchPrompt;
    public Slider progressBar;
    public TextMeshProUGUI foundItemText;

    [Header("Search Settings")]
    public float searchTime = 2f;

    [Tooltip("What shows inside the inventory slot")]
    public string inventorySymbol = "C";

    [TextArea]
    public string discoveryMessage = "You found a strange coin engraved with unknown symbols.";

    [Header("Inventory")]
    public InventoryManager inventoryManager;

    [Header("Dialogue")]
    public DialogueManager dialogueManager;

    private bool playerInRange = false;
    private bool hasBeenSearched = false;
    private float holdTimer = 0f;

    void Start()
    {
        if (searchPrompt != null)
            searchPrompt.SetActive(false);

        if (progressBar != null)
        {
            progressBar.gameObject.SetActive(false);
            progressBar.value = 0f;
            progressBar.maxValue = searchTime;
        }

        if (foundItemText != null)
            foundItemText.gameObject.SetActive(false);
    }

    void Update()
    {
        if (!playerInRange || hasBeenSearched)
            return;

        if (Input.GetKey(KeyCode.E))
        {
            holdTimer += Time.deltaTime;

            if (progressBar != null)
            {
                progressBar.gameObject.SetActive(true);
                progressBar.value = holdTimer;
            }

            if (holdTimer >= searchTime)
            {
                CompleteSearch();
            }
        }

        if (Input.GetKeyUp(KeyCode.E))
        {
            ResetSearch();
        }
    }

    void CompleteSearch()
    {
        hasBeenSearched = true;

        if (searchPrompt != null)
            searchPrompt.SetActive(false);

        if (progressBar != null)
        {
            progressBar.value = searchTime;
            progressBar.gameObject.SetActive(false);
        }

        if (inventoryManager != null)
        {
            inventoryManager.AddItem(inventorySymbol);
        }

        if (dialogueManager != null)
        {
            dialogueManager.hasCoin = true;
        }

        if (foundItemText != null)
        {
            foundItemText.text = discoveryMessage;
            foundItemText.gameObject.SetActive(true);
            Invoke(nameof(HideFoundText), 4f);
        }

        Debug.Log("Search complete: " + inventorySymbol);
    }

    void HideFoundText()
    {
        if (foundItemText != null)
            foundItemText.gameObject.SetActive(false);
    }

    void ResetSearch()
    {
        holdTimer = 0f;

        if (progressBar != null)
        {
            progressBar.value = 0f;
            progressBar.gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasBeenSearched)
        {
            playerInRange = true;

            if (searchPrompt != null)
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

            ResetSearch();
        }
    }
}