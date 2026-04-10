using UnityEngine;

public class DistanceSound : MonoBehaviour
{
    public Transform player;
    public AudioSource audioSource;
    public float triggerDistance = 5f;

    void Update()
    {
        float distance = Vector3.Distance(player.position, transform.position);

        if (distance <= triggerDistance)
        {
            if (!audioSource.isPlaying)
                audioSource.Play();
        }
        else
        {
            if (audioSource.isPlaying)
                audioSource.Stop();
        }
    }
}