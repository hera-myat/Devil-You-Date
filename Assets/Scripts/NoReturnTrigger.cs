using UnityEngine;
using TMPro;

public class NoReturnTrigger : MonoBehaviour
{
    public TextMeshProUGUI messageText;

    private bool shown = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (shown) return;

        shown = true;

        if (messageText != null)
        {
            messageText.text = "I won't go back.";
            messageText.gameObject.SetActive(true);
        }
    }
}