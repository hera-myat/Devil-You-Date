using UnityEngine;
using TMPro;

public class CanPickup : MonoBehaviour
{
    [Header("References")]
    public InventoryManager inventoryManager;
    public GameObject interactPrompt;
    public TextMeshProUGUI resultText;
    public CanCleanupTimer cleanupTimer;

    [Header("Item")]
    public string itemId = "can";
    public int canAmount = 4;

    private bool playerInRange = false;
    private bool pickedUp = false;

    void Start()
    {
        if (interactPrompt != null)
            interactPrompt.SetActive(false);

        if (resultText != null)
            resultText.gameObject.SetActive(false);
    }

    void Update()
    {
        if (!playerInRange || pickedUp)
            return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            PickUpCans();
        }
    }

    void PickUpCans()
    {
        if (inventoryManager == null)
        {
            Debug.LogWarning("CanPickup: InventoryManager is missing.");
            return;
        }

        int addedCount = 0;

        for (int i = 0; i < canAmount; i++)
        {
            bool added = inventoryManager.AddItem(itemId);

            if (added)
                addedCount++;
            else
                break;
        }

        if (addedCount == 0)
        {
            ShowMessage("Inventory is full.");
            return;
        }

        if (cleanupTimer != null && !cleanupTimer.IsRunning() && !cleanupTimer.HasFinished())
        {
            cleanupTimer.StartTimer();
        }

        pickedUp = true;

        if (addedCount == canAmount)
            ShowMessage("You picked up 4 cans.");
        else
            ShowMessage("Only picked up " + addedCount + " cans. Inventory is full.");

        if (interactPrompt != null)
            interactPrompt.SetActive(false);

        gameObject.SetActive(false);
    }

    void ShowMessage(string message)
    {
        if (resultText != null)
        {
            resultText.text = message;
            resultText.gameObject.SetActive(true);
            CancelInvoke(nameof(HideMessage));
            Invoke(nameof(HideMessage), 2f);
        }
    }

    void HideMessage()
    {
        if (resultText != null)
            resultText.gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !pickedUp)
        {
            playerInRange = true;

            if (interactPrompt != null)
                interactPrompt.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;

            if (interactPrompt != null)
                interactPrompt.SetActive(false);
        }
    }
}