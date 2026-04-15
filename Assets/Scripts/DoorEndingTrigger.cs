using UnityEngine;

public class DoorEndingTrigger : MonoBehaviour
{
    public FinalEndingSequence finalSequence;

    private bool canExit = false;

    public void EnableExit()
    {
        canExit = true;
        Debug.Log("DoorEndingTrigger: Exit door enabled.");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (!canExit)
            return;

        if (finalSequence != null)
        {
            finalSequence.StartFinalEnding();
        }
    }
}