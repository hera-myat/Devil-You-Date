using UnityEngine;
using TMPro;

public class VendingMachine : MonoBehaviour
{
    [Header("Vending Settings")]
    [Tooltip("Type soda, coffee, or food")]
    public string itemName = "soda";

    [Header("UI")]
    public GameObject interactPrompt;
    public TextMeshProUGUI resultText;

    [Header("References")]
    public DateDialogue dateDialogue;
    public InventoryManager inventoryManager;
    public SuspicionSystem suspicionSystem;

    private bool playerInRange = false;
    private bool usedMachine = false;
    private bool suspicionApplied = false;

    void Start()
    {
        if (interactPrompt != null)
            interactPrompt.SetActive(false);

        if (resultText != null)
            resultText.gameObject.SetActive(false);
    }

    void Update()
    {
        if (!playerInRange || usedMachine)
            return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            TryUseMachine();
        }
    }

    void TryUseMachine()
    {
        if (inventoryManager == null)
        {
            Debug.LogWarning("VendingMachine: InventoryManager is missing.");
            return;
        }

        if (!inventoryManager.HasItem("coin"))
        {
            ShowMessage("You need a coin to use this vending machine.");
            return;
        }

        string itemSymbol = GetItemSymbol();

        if (string.IsNullOrEmpty(itemSymbol))
        {
            ShowMessage("Unknown vending item.");
            return;
        }

        bool removedCoin = inventoryManager.RemoveItem("coin");

        if (!removedCoin)
        {
            ShowMessage("You need a coin to use this vending machine.");
            return;
        }

        bool addedItem = inventoryManager.AddItem(itemSymbol);

        if (!addedItem)
        {
            inventoryManager.AddItem("coin");
            ShowMessage("Your inventory is full.");
            return;
        }

        if (dateDialogue != null)
        {
            dateDialogue.SetGiftItem(itemName);

            if (!dateDialogue.hasTalkedToDate && !suspicionApplied && suspicionSystem != null)
            {
                suspicionSystem.IncreaseSuspicion();
                suspicionApplied = true;
            }
        }

        usedMachine = true;

        ShowMessage("You got a " + itemName + ", maybe you can give it to your Date.");

        if (interactPrompt != null)
            interactPrompt.SetActive(false);

        Debug.Log("Vending machine used. Item received: " + itemName + " (" + itemSymbol + ")");
    }

    string GetItemSymbol()
    {
        string lowerItem = itemName.ToLower();

        if (lowerItem == "soda")
            return "soda";
        else if (lowerItem == "food")
            return "food";
        else if (lowerItem == "coffee")
            return "coffee";

        return "";
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
        if (other.CompareTag("Player") && !usedMachine)
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