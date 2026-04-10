using UnityEngine;

public class GoodEndingManager : MonoBehaviour
{
    public InventoryManager inventoryManager;
    public FreedomEndingSequence endingSequence;

    private bool endingTriggered = false;

    public string[] requiredItems =
    {
        "planner",
        "trashclean",
        "coinaward",
        "cross"
    };

    void Update()
    {
        if (endingTriggered) return;

        if (HasAllItems())
        {
            TriggerEnding();
        }
    }

    bool HasAllItems()
    {
        if (inventoryManager == null)
            return false;

        foreach (string item in requiredItems)
        {
            if (!inventoryManager.HasItem(item))
                return false;
        }

        return true;
    }

    void TriggerEnding()
    {
        endingTriggered = true;

        if (endingSequence != null)
        {
            endingSequence.StartFreedomEnding();
        }
    }
}