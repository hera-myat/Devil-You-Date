using UnityEngine;

public class GarbageAreaTrigger : MonoBehaviour
{
    public GarbageQuestManager garbageQuestManager;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (garbageQuestManager != null)
        {
            garbageQuestManager.EnterGarbageArea();
        }
    }
}