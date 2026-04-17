using UnityEngine;
using TMPro;
using System.Collections;

public class ObjectiveUI : MonoBehaviour
{
    public TextMeshProUGUI objectiveText;
    public float showDuration = 8f;

    private Coroutine currentRoutine;

    void Start()
    {
        if (objectiveText != null)
            objectiveText.gameObject.SetActive(false);
    }

    public void ShowObjective(string message)
    {
        if (objectiveText == null)
            return;

        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        currentRoutine = StartCoroutine(ShowObjectiveRoutine(message));
    }

    IEnumerator ShowObjectiveRoutine(string message)
    {
        objectiveText.gameObject.SetActive(true);
        objectiveText.text = message;

        yield return new WaitForSeconds(showDuration);

        objectiveText.gameObject.SetActive(false);
        currentRoutine = null;
    }
}