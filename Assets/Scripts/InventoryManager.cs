using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryManager : MonoBehaviour
{
    [Header("UI")]
    public Image[] slotImages;
    public TextMeshProUGUI[] slotTexts;

    [Header("Slot Sprites")]
    public Sprite normalSlot;
    public Sprite selectedSlotSprite;

    private string[] inventory = new string[4];
    private int selectedSlotIndex = 0;

    void Start()
    {
        UpdateInventoryUI();
        UpdateSelectedSlot();
    }

    void Update()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll > 0f)
        {
            selectedSlotIndex--;
            if (selectedSlotIndex < 0)
                selectedSlotIndex = inventory.Length - 1;

            UpdateSelectedSlot();
        }
        else if (scroll < 0f)
        {
            selectedSlotIndex++;
            if (selectedSlotIndex >= inventory.Length)
                selectedSlotIndex = 0;

            UpdateSelectedSlot();
        }
    }

    public bool AddItem(string itemSymbol)
    {
        for (int i = 0; i < inventory.Length; i++)
        {
            if (string.IsNullOrEmpty(inventory[i]))
            {
                inventory[i] = itemSymbol;
                UpdateInventoryUI();
                return true;
            }
        }

        Debug.Log("Inventory is full.");
        return false;
    }

    void UpdateInventoryUI()
    {
        for (int i = 0; i < slotTexts.Length; i++)
        {
            if (string.IsNullOrEmpty(inventory[i]))
                slotTexts[i].text = "";
            else
                slotTexts[i].text = inventory[i];
        }
    }

    void UpdateSelectedSlot()
    {
        for (int i = 0; i < slotImages.Length; i++)
        {
            if (i == selectedSlotIndex)
                slotImages[i].sprite = selectedSlotSprite;
            else
                slotImages[i].sprite = normalSlot;
        }
    }
}