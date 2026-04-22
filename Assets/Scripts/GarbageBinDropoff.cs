using UnityEngine;
using TMPro;

public class GarbageBinDropoff : MonoBehaviour
{
    [Header("References")]
    public InventoryManager inventoryManager;
    public GarbageQuestManager garbageQuestManager;
    public GameObject interactPrompt;
    public TextMeshProUGUI resultText;

    public GarbageHintManager garbageHintManager;

    [Header("Bin Settings")]
    public string acceptedCategory = "paper"; //paper, plastic, metal, general

    private bool playerInRange = false;

    void Start()
    {
        if (interactPrompt != null)
            interactPrompt.SetActive(false);

        if (resultText != null)
            resultText.gameObject.SetActive(false);
    }

    void Update()
    {
        if (!playerInRange)
            return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            TryDropItem();
        }
    }

    void TryDropItem()
    {
        if (inventoryManager == null || garbageQuestManager == null)
            return;

        string selectedItem = inventoryManager.GetSelectedItem();

        if (string.IsNullOrEmpty(selectedItem))
        {
            ShowMessage("Select an item first.");
            return;
        }

        if (!ItemMatchesBin(selectedItem))
        {
            ShowMessage("That item does not belong in this bin.");
            return;
        }

        bool removed = inventoryManager.RemoveSelectedItem();
        if (!removed)
            return;

        garbageQuestManager.RegisterTrashThrown(selectedItem);
        ShowMessage("Thrown away.");

        if (garbageHintManager != null)
            garbageHintManager.RefreshHint();
    }

    bool ItemMatchesBin(string itemId)
    {
        if (acceptedCategory == "paper")
            return itemId == "pizzabox";

        if (acceptedCategory == "plastic")
            return itemId == "bottle";

        if (acceptedCategory == "metal")
            return itemId == "metalcan" || itemId == "spraycan" || itemId == "sodacan";

        if (acceptedCategory == "trash")
            return itemId == "cigarette";

        return false;
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
        if (!other.CompareTag("Player"))
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

        if (interactPrompt != null)
            interactPrompt.SetActive(false);
    }
}