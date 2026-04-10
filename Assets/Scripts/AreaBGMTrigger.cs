using UnityEngine;

public class AreaBGMTrigger : MonoBehaviour
{
    [Header("Audio")]
    public AudioSource bgmSource;
    public AudioClip bgmClip;

    private bool isPlaying = false;

    void Start()
    {
        if (bgmSource != null)
        {
            bgmSource.playOnAwake = false;
            bgmSource.loop = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        PlayBGM();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        StopBGM();
    }

    void PlayBGM()
    {
        if (bgmSource == null || bgmClip == null || isPlaying)
            return;

        bgmSource.clip = bgmClip;
        bgmSource.Play();
        isPlaying = true;
    }

    void StopBGM()
    {
        if (bgmSource == null)
            return;

        if (bgmSource.isPlaying)
            bgmSource.Stop();

        isPlaying = false;
    }
}