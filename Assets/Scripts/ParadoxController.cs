using UnityEngine;

public class ParadoxController : MonoBehaviour
{
    public Transform pivot;
    public float closedAngle = 0f;
    public float openAngle = 90f;
    public float rotateSpeed = 90f;
    public float stateDuration = 10f;

    private bool isOpen = false;
    private float timer = 0f;

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= stateDuration)
        {
            timer = 0f;
            isOpen = !isOpen;
        }

        float targetAngle = isOpen ? openAngle : closedAngle;
        Quaternion targetRotation = Quaternion.Euler(0f, targetAngle, 0f);

        pivot.localRotation = Quaternion.RotateTowards(
            pivot.localRotation,
            targetRotation,
            rotateSpeed * Time.deltaTime
        );
    }

    public bool IsOpen()
    {
        return isOpen;
    }
}
