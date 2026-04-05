using UnityEngine;

public class Minimap : MonoBehaviour
{
    public Transform player;
    public Camera minimapCamera;
    public GameObject minimapUI;

    public float zoomSpeed = 10f;
    public float minZoom = 10f;
    public float maxZoom = 80f;

    public KeyCode toggleKey = KeyCode.M;

    private bool minimapActive = false;

    void Start()
    {
        SetMinimapActive(minimapActive);
    }

    void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            minimapActive = !minimapActive;
            SetMinimapActive(minimapActive);
        }

        if (minimapActive && minimapCamera != null)
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel");

            if (scroll != 0f)
            {
                minimapCamera.orthographicSize -= scroll * zoomSpeed;
                minimapCamera.orthographicSize = Mathf.Clamp(minimapCamera.orthographicSize, minZoom, maxZoom);
            }
        }
    }

    void LateUpdate()
    {
        Vector3 newPosition = player.position;
        newPosition.y = transform.position.y;
        transform.position = newPosition;

        transform.rotation = Quaternion.Euler(90f, player.eulerAngles.y, 0f);
    }

    void SetMinimapActive(bool active)
    {
        if (minimapCamera != null)
        {
            minimapCamera.enabled = active;
        }

        if (minimapUI != null)
        {
            minimapUI.SetActive(active);
        }
    }
}
