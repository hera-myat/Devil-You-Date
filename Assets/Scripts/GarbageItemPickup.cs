using UnityEngine;
using TMPro;

public class GarbageItemPickup : MonoBehaviour
{
    [Header("References")]
    public InventoryManager inventoryManager;
    public GameObject interactPrompt;
    public TextMeshProUGUI resultText;

    [Header("Item")]
    public string itemId = "pizzabox";

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
            PickUpItem();
        }
    }

    void PickUpItem()
    {
        if (inventoryManager == null)
            return;

        bool added = inventoryManager.AddItem(itemId);

        if (!added)
        {
            ShowMessage("Inventory is full.");
            return;
        }

        pickedUp = true;

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