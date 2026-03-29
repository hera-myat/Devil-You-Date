using UnityEngine;

public class DiaryPickup : MonoBehaviour
{
    [Header("References")]
    public SideQuestNPC questNPC;
    public InventoryManager inventoryManager;

    [Header("Optional Prompt")]
    public GameObject interactionPrompt;

    [Header("Inventory")]
    public string itemSymbol = "D";

    private bool playerInRange = false;
    private bool pickedUp = false;

    void Start()
    {
        if (interactionPrompt != null)
        {
            interactionPrompt.SetActive(false);
        }
    }

    void Update()
    {
        if (!playerInRange) return;
        if (pickedUp) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            PickUpDiary();
        }
    }

    void PickUpDiary()
    {
        pickedUp = true;

        if (questNPC != null)
        {
            questNPC.SetPlayerHasDiary(true);
        }

        if (inventoryManager != null)
        {
            bool added = inventoryManager.AddItem(itemSymbol);

            if (!added)
            {
                Debug.Log("Diary could not be added because inventory is full.");
            }
        }

        if (interactionPrompt != null)
        {
            interactionPrompt.SetActive(false);
        }

        gameObject.SetActive(false);

        Debug.Log("Diary picked up");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;

            if (interactionPrompt != null)
            {
                interactionPrompt.SetActive(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;

            if (interactionPrompt != null)
            {
                interactionPrompt.SetActive(false);
            }
        }
    }
}