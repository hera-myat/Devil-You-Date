using UnityEngine;

public class RestaurantBGMTrigger : MonoBehaviour
{
    [Header("Audio")]
    public AudioSource barBGM;
    public AudioClip barClip;

    [Header("Default BGM")]
    public SpawnAreaBGM spawnAreaBGM;

    private bool playerInside = false;

    void Start()
    {
        if (barBGM != null)
        {
            barBGM.playOnAwake = false;
            barBGM.loop = true;

            if (barClip != null)
                barBGM.clip = barClip;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (playerInside)
            return;

        playerInside = true;

        if (spawnAreaBGM != null)
            spawnAreaBGM.StopBGM();

        if (barBGM != null && !barBGM.isPlaying)
            barBGM.Play();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        playerInside = false;

        if (barBGM != null && barBGM.isPlaying)
            barBGM.Stop();

        if (spawnAreaBGM != null)
            spawnAreaBGM.PlayBGM();
    }
}