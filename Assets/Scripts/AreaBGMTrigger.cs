using UnityEngine;

public class AreaBGMTrigger : MonoBehaviour
{
    [Header("Audio")]
    public AudioSource bgmSource;
    public AudioClip bgmClip;

    [Header("Default BGM")]
    public SpawnAreaBGM spawnAreaBGM;

    private bool playerInside = false;

    void Start()
    {
        if (bgmSource != null)
        {
            bgmSource.playOnAwake = false;
            bgmSource.loop = true;

            if (bgmClip != null)
                bgmSource.clip = bgmClip;
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

        if (bgmSource != null && !bgmSource.isPlaying)
            bgmSource.Play();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        playerInside = false;

        if (bgmSource != null && bgmSource.isPlaying)
            bgmSource.Stop();

        if (spawnAreaBGM != null)
            spawnAreaBGM.PlayBGM();
    }
}