using UnityEngine;
using TMPro;

public class AreaHintTrigger : MonoBehaviour
{
    public TextMeshProUGUI hintText;

    [TextArea]
    public string message = "There might be something useful around the garbage cans.";

    public float showTime = 3f;

    private static bool hasShown = false;

    private void OnTriggerEnter(Collider other)
    {
        if (hasShown)
            return;

        if (other.CompareTag("Player"))
        {
            hasShown = true;

            if (hintText != null)
            {
                hintText.text = message;
                hintText.gameObject.SetActive(true);

                CancelInvoke(nameof(HideHint));
                Invoke(nameof(HideHint), showTime);
            }
        }
    }

    void HideHint()
    {
        if (hintText != null)
            hintText.gameObject.SetActive(false);
    }
}