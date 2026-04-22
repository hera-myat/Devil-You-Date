using UnityEngine;

public class GarbageHintManager : MonoBehaviour
{
    [Header("References")]
    public InventoryManager inventoryManager;

    [Header("Arrow Objects")]
    public GameObject[] paperArrows;
    public GameObject[] plasticArrows;
    public GameObject[] metalArrows;
    public GameObject[] trashArrows;

    private bool playerInGarbageArea = false;

    void Start()
    {
        HideAllArrows();
    }

    void OnEnable()
    {
        if (inventoryManager != null)
            inventoryManager.OnSelectedSlotChanged += HandleSelectedSlotChanged;
    }

    void OnDisable()
    {
        if (inventoryManager != null)
            inventoryManager.OnSelectedSlotChanged -= HandleSelectedSlotChanged;
    }

    public void EnterGarbageArea()
    {
        playerInGarbageArea = true;
        RefreshHint();
    }

    public void ExitGarbageArea()
    {
        playerInGarbageArea = false;
        HideAllArrows();
    }

    void HandleSelectedSlotChanged(int slotIndex, string selectedItem)
    {
        RefreshHint();
    }

    public void RefreshHint()
    {
        HideAllArrows();

        if (!playerInGarbageArea || inventoryManager == null)
            return;

        string selectedItem = inventoryManager.GetSelectedItem();

        if (string.IsNullOrEmpty(selectedItem))
            return;

        if (selectedItem == "pizzabox")
        {
            ShowArrows(paperArrows);
        }
        else if (selectedItem == "bottle")
        {
            ShowArrows(plasticArrows);
        }
        else if (selectedItem == "metalcan" || selectedItem == "spraycan" || selectedItem == "sodacan")
        {
            ShowArrows(metalArrows);
        }
        else if (selectedItem == "cigarette")
        {
            ShowArrows(trashArrows);
        }
    }

    void ShowArrows(GameObject[] arrows)
    {
        if (arrows == null)
            return;

        for (int i = 0; i < arrows.Length; i++)
        {
            if (arrows[i] != null)
                arrows[i].SetActive(true);
        }
    }

    void HideAllArrows()
    {
        HideArrowGroup(paperArrows);
        HideArrowGroup(plasticArrows);
        HideArrowGroup(metalArrows);
        HideArrowGroup(trashArrows);
    }

    void HideArrowGroup(GameObject[] arrows)
    {
        if (arrows == null)
            return;

        for (int i = 0; i < arrows.Length; i++)
        {
            if (arrows[i] != null)
                arrows[i].SetActive(false);
        }
    }
}