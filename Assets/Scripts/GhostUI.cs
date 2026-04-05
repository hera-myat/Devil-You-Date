using UnityEngine;
using TMPro;
using System.Collections;

public class GhostUI : MonoBehaviour
{
    public TextMeshProUGUI messageText;
    public float messageDuration = 2f;

    private Coroutine currentRoutine;

    void Start()
    {
        messageText.gameObject.SetActive(false);
    }

    public void ShowMessage(string message)
    {
        if (currentRoutine != null)
        {
            StopCoroutine(currentRoutine);
        }

        currentRoutine = StartCoroutine(ShowMessageRoutine(message));
    }

    IEnumerator ShowMessageRoutine(string message)
    {
        messageText.text = message;
        messageText.gameObject.SetActive(true);

        yield return new WaitForSeconds(messageDuration);

        messageText.gameObject.SetActive(false);
    }
}
