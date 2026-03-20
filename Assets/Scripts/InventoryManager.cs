using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryManager : MonoBehaviour
{
    [Header("UI")]
    public Image[] slotHighlights;
    public TextMeshProUGUI[] slotTexts;

    private string[] inventory = new string[4];
    private int selectedSlot = 0;

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
            selectedSlot--;
            if (selectedSlot < 0)
            {
                selectedSlot = inventory.Length - 1;
            }
            UpdateSelectedSlot();
        }
        else if (scroll < 0f)
        {
            selectedSlot++;
            if (selectedSlot >= inventory.Length)
            {
                selectedSlot = 0;
            }
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
            {
                slotTexts[i].text = "";
            }
            else
            {
                slotTexts[i].text = inventory[i];
            }
        }
    }

    void UpdateSelectedSlot()
    {
        for (int i = 0; i < slotHighlights.Length; i++)
        {
            Color c = slotHighlights[i].color;
            c.a = (i == selectedSlot) ? 0.9f : 0.35f;
            slotHighlights[i].color = c;
        }
    }
}