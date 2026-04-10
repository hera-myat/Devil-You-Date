using UnityEngine;
using System.Collections;

public class GhostAttack : MonoBehaviour
{
    public Transform attackDestination;
    public GhostUI messageUI;
    public float atkCooldown = 2f;

    public AudioSource jumpscareSource;
    public AudioSource breathingSource;
    public AudioClip jumpscareClip;
    public AudioClip breathingClip;
    public float breathingDelay = 0.4f;

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
            messageUI.ShowMessage("You were chased away by a ghost!");
        }

        if (jumpscareSource != null && jumpscareClip != null)
        {
            jumpscareSource.PlayOneShot(jumpscareClip);
        }

        StartCoroutine(PlayBreathingAfterDelay());
    }

    IEnumerator PlayBreathingAfterDelay()
    {
        yield return new WaitForSeconds(breathingDelay);

        if (breathingSource != null && breathingClip != null)
        {
            breathingSource.PlayOneShot(breathingClip);
        }
    }
}
