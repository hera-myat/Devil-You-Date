using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryManager : MonoBehaviour
{
    [Header("UI")]
    public Image[] slotImages;
    public Image[] itemIcons;
    public TextMeshProUGUI[] slotTexts;

    [Header("Slot Sprites")]
    public Sprite normalSlot;
    public Sprite selectedSlotSprite;

    [Header("Item Icons")]
    public Sprite canIcon;
    public Sprite coinIcon;
    public Sprite diaryIcon;
    public Sprite sodaIcon;
    public Sprite foodIcon;
    public Sprite coffeeIcon;
    public Sprite knifeIcon;
    public Sprite plannerIcon;
    public Sprite coinAwardIcon;
    public Sprite crossIcon;
    public Sprite trashCleanIcon;

    public Sprite bottleIcon;
    public Sprite pizzaBoxIcon;
    public Sprite sodaCanIcon;
    public Sprite sprayCanIcon;
    public Sprite metalCanIcon;
    public Sprite cigaretteIcon;


    private string[] inventory = new string[4];
    private int selectedSlotIndex = 0;

    void Start()
    {
        UpdateInventoryUI();
        UpdateSelectedSlot();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            selectedSlotIndex = 0;
            UpdateSelectedSlot();
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            selectedSlotIndex = 1;
            UpdateSelectedSlot();
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            selectedSlotIndex = 2;
            UpdateSelectedSlot();
        }

        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            selectedSlotIndex = 3;
            UpdateSelectedSlot();
        }
    }

    public bool AddItem(string itemId)
    {
        for (int i = 0; i < inventory.Length; i++)
        {
            if (string.IsNullOrEmpty(inventory[i]))
            {
                inventory[i] = itemId;
                UpdateInventoryUI();
                return true;
            }
        }

        Debug.Log("Inventory is full.");
        return false;
    }

    public bool RemoveItem(string itemId)
    {
        for (int i = 0; i < inventory.Length; i++)
        {
            if (inventory[i] == itemId)
            {
                inventory[i] = "";
                UpdateInventoryUI();
                return true;
            }
        }

        Debug.Log(itemId + " was not found in inventory.");
        return false;
    }

    public bool RemoveSelectedItem()
    {
        if (string.IsNullOrEmpty(inventory[selectedSlotIndex]))
        {
            Debug.Log("Selected slot is empty.");
            return false;
        }

        inventory[selectedSlotIndex] = "";
        UpdateInventoryUI();
        return true;
    }

    public string GetSelectedItem()
    {
        return inventory[selectedSlotIndex];
    }

    public int GetSelectedSlotIndex()
    {
        return selectedSlotIndex;
    }

    public bool HasItem(string itemId)
    {
        for (int i = 0; i < inventory.Length; i++)
        {
            if (inventory[i] == itemId)
                return true;
        }

        return false;
    }

    Sprite GetIconForItem(string itemId)
    {
        if (itemId == "can") return canIcon;
        if (itemId == "coin") return coinIcon;
        if (itemId == "diary") return diaryIcon;
        if (itemId == "soda") return sodaIcon;
        if (itemId == "food") return foodIcon;
        if (itemId == "coffee") return coffeeIcon;
        if (itemId == "knife") return knifeIcon;
        if (itemId == "planner") return plannerIcon;
        if (itemId == "coinaward") return coinAwardIcon;
        if (itemId == "cross") return crossIcon;
        if (itemId == "trashclean") return trashCleanIcon;

        if (itemId == "bottle") return bottleIcon;
        if (itemId == "pizzabox") return pizzaBoxIcon;
        if (itemId == "sodacan") return sodaCanIcon;
        if (itemId == "spraycan") return sprayCanIcon;
        if (itemId == "metalcan") return metalCanIcon;
        if (itemId == "cigarette") return cigaretteIcon;

        return null;
    }

    void UpdateInventoryUI()
    {
        for (int i = 0; i < inventory.Length; i++)
        {
            if (slotTexts != null && i < slotTexts.Length && slotTexts[i] != null)
            {
                slotTexts[i].text = "";
            }

            if (itemIcons != null && i < itemIcons.Length && itemIcons[i] != null)
            {
                if (string.IsNullOrEmpty(inventory[i]))
                {
                    itemIcons[i].sprite = null;
                    itemIcons[i].enabled = false;
                }
                else
                {
                    Sprite icon = GetIconForItem(inventory[i]);

                    Debug.Log("Slot " + i + " item = " + inventory[i] +
                              " | icon = " + (icon != null ? icon.name : "NULL"));

                    if (icon != null)
                    {
                        itemIcons[i].sprite = icon;
                        itemIcons[i].enabled = true;
                        itemIcons[i].color = Color.white;
                    }
                    else
                    {
                        itemIcons[i].sprite = null;
                        itemIcons[i].enabled = false;
                    }
                }
            }
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