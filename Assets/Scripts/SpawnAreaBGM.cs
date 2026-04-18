using UnityEngine;

public class SpawnAreaBGM : MonoBehaviour
{
    public AudioSource bgmSource;
    public AudioClip bgmClip;

    private bool isLocked = false;

    void Start()
    {
        if (bgmSource == null) return;

        bgmSource.loop = true;
        bgmSource.playOnAwake = false;

        if (bgmClip != null)
            bgmSource.clip = bgmClip;

        PlayBGM();
    }

    public void PlayBGM()
    {
        if (isLocked || bgmSource == null) return;

        if (!bgmSource.isPlaying)
            bgmSource.Play();
    }

    public void StopBGM()
    {
        if (bgmSource == null) return;

        bgmSource.Stop();
    }

    public void StopAndLockBGM()
    {
        isLocked = true;
        StopBGM();
    }

    public void ForceKill()
    {
        if (bgmSource == null) return;

        bgmSource.Stop();
        bgmSource.clip = null;
        bgmSource.enabled = false;
    }
}