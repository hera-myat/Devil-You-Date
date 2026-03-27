using UnityEngine;

public class GhostAttack : MonoBehaviour
{
    public Transform attackDestination;
    public GhostUI messageUI;
    public float atkCooldown = 2f;

    private float lastAttackTime;

    private void OnTriggerEnter(Collider other)
    {
        Transform root = other.transform.root;

        if (!root.CompareTag("Player")) return;

        if (Time.time - lastAttackTime < atkCooldown) return;

        lastAttackTime = Time.time;
        CharacterController cc = root.GetComponent<CharacterController>();

        if (cc != null) cc.enabled = false;

        root.position = attackDestination.position;

        if (cc != null) cc.enabled = true;

        if (messageUI != null)
        {
            messageUI.ShowMessage("You were attacked by a ghost!");
        }
    }
}
