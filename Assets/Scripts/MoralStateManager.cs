using UnityEngine;

public class MoralStateManager : MonoBehaviour
{
    [Header("State")]
    public bool hasRepented = false;
    public bool hasTouchedBloodyArea = false;

    public void MarkRepented()
    {
        hasRepented = true;
    }

    public void MarkBloodyAreaTouched()
    {
        hasTouchedBloodyArea = true;
    }
}