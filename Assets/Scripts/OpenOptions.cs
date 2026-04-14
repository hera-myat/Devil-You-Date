using UnityEngine;
using TMPro;
using System.Collections;

public class OpenOptions : MonoBehaviour
{
    public GameObject optionsPanel;
    public MonoBehaviour playerMovement;
    public MonoBehaviour playerLook;

    private bool isOpen = false;

    public TextMeshProUGUI hintText;
    public float hintDisplayTime = 3f;
    public float hintFadeDuration = 1f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (optionsPanel != null)
        {
            optionsPanel.SetActive(false);
        }

        if (hintText != null)
        {
            hintText.gameObject.SetActive(true);
            StartCoroutine(FadeHint());
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleOptionsPanel();
        }
    }

    public void ToggleOptionsPanel()
    {
        if (optionsPanel == null) return;

        isOpen = !isOpen;
        optionsPanel.SetActive(isOpen);

        if (playerMovement != null)
        {
            playerMovement.enabled = !isOpen;
        }

        if (playerLook != null)
        {
            playerLook.enabled = !isOpen;
        }

        if (isOpen)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    IEnumerator FadeHint()
    {
        yield return new WaitForSecondsRealtime(hintDisplayTime);

        float t = 0;
        Color startColor = hintText.color;

        while (t < hintFadeDuration)
        {
            t += Time.unscaledDeltaTime;
            float alpha = Mathf.Lerp(1, 0, t / hintFadeDuration);
            hintText.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
            yield return null;
        }

        hintText.gameObject.SetActive(false);
    }

    public void CloseOptionsPanel()
    {
        if (optionsPanel == null) return;

        isOpen = false;
        optionsPanel.SetActive(false);

        if (playerMovement != null)
        {
            playerMovement.enabled = true;
        }

        if (playerLook != null)
        {
            playerLook.enabled = true;
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
