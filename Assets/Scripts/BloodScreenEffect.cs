using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BloodScreenEffect : MonoBehaviour
{
    public Image bloodImage;
    public float fadeInTime = 0.2f;
    public float holdTime = 0.5f;
    public float fadeOutTime = 0.5f;

    public void ShowBlood()
    {
        StartCoroutine(BloodRoutine());
    }

    IEnumerator BloodRoutine()
    {
        // fade in
        yield return Fade(0f, 1f, fadeInTime);

        // hold
        yield return new WaitForSeconds(holdTime);

        // fade out
        yield return Fade(1f, 0f, fadeOutTime);
    }

    IEnumerator Fade(float start, float end, float duration)
    {
        float t = 0f;
        Color c = bloodImage.color;

        while (t < duration)
        {
            t += Time.deltaTime;
            c.a = Mathf.Lerp(start, end, t / duration);
            bloodImage.color = c;
            yield return null;
        }

        c.a = end;
        bloodImage.color = c;
    }
}