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
    private int remainingCans;

    void Start()
    {
        remainingCans = canAmount;

        if (interactPrompt != null)
            interactPrompt.SetActive(false);

        if (resultText != null)
            resultText.gameObject.SetActive(false);
    }

    void Update()
    {
        if (!playerInRange)
            return;

        if (remainingCans <= 0)
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

        for (int i = 0; i < remainingCans; i++)
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

        remainingCans -= addedCount;

        if (cleanupTimer != null && !cleanupTimer.IsRunning() && !cleanupTimer.HasFinished())
        {
            cleanupTimer.StartTimer();
        }

        if (remainingCans <= 0)
        {
            ShowMessage("You picked up all the cans.");

            if (interactPrompt != null)
                interactPrompt.SetActive(false);

            gameObject.SetActive(false);
        }
        else
        {
            ShowMessage("You picked up " + addedCount + " cans. " + remainingCans + " left.");
        }
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
        if (other.CompareTag("Player") && remainingCans > 0)
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