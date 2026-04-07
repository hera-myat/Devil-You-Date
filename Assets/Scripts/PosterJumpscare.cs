using UnityEngine;

public class PosterJumpscare : MonoBehaviour
{
    public Transform poster;
    public float slideDistance = 1.5f;
    public float moveSpeed = 1.5f;

    private Vector3 hiddenPosition;
    private Vector3 visiblePosition;

    private float t = 0f;
    private bool movingForward = true;

    void Start()
    {
        visiblePosition = poster.position;
        hiddenPosition = visiblePosition + Vector3.right * slideDistance;

        poster.position = hiddenPosition;
    }

    void Update()
    {
        t += Time.deltaTime * moveSpeed;

        float smoothT = t * t * (3f - 2f * t);

        if (movingForward)
            poster.position = Vector3.Lerp(hiddenPosition, visiblePosition, smoothT);
        else
            poster.position = Vector3.Lerp(visiblePosition, hiddenPosition, smoothT);

        if (t >= 1f)
        {
            t = 0f;
            movingForward = !movingForward; // reverse direction
        }
    }
}