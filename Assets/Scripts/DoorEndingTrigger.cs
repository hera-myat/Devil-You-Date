using UnityEngine;

public class DoorEndingTrigger : MonoBehaviour
{
    public FinalEndingSequence finalSequence;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (finalSequence != null)
        {
            finalSequence.StartFinalEnding();
        }
    }
}