using UnityEngine;

public class FloatingHintArrow : MonoBehaviour
{
    public float floatAmplitude = 0.15f;
    public float floatSpeed = 2f;

    private Vector3 startLocalPos;

    void OnEnable()
    {
        startLocalPos = transform.localPosition;
    }

    void Update()
    {
        float yOffset = Mathf.Sin(Time.time * floatSpeed) * floatAmplitude;
        transform.localPosition = startLocalPos + new Vector3(0f, yOffset, 0f);
    }
}