using UnityEngine;

public class GarbageAreaTrigger : MonoBehaviour
{
    public GarbageQuestManager garbageQuestManager;
    public GarbageHintManager garbageHintManager;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (garbageQuestManager != null)
            garbageQuestManager.EnterGarbageArea();

        if (garbageHintManager != null)
            garbageHintManager.EnterGarbageArea();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (garbageHintManager != null)
            garbageHintManager.ExitGarbageArea();
    }
}