using UnityEngine;

public class ParadoxTP : MonoBehaviour
{
    public ParadoxController paradoxZone;
    public Transform paraDestination;
    public GhostUI messageUI;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (!paradoxZone.IsOpen())
        {
            CharacterController cc = other.GetComponent<CharacterController>();
            
            if (cc != null) cc.enabled = false;

            other.transform.position = paraDestination.position;

            if (cc != null) cc.enabled = true;

            if (messageUI != null)
            {
                messageUI.ShowMessage("Wait... what happened?");
            }
        }
    }
}
