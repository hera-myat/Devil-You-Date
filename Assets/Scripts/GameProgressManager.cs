using UnityEngine;

public class GameProgressManager : MonoBehaviour
{
    public static GameProgressManager Instance;

    [Header("Progress State")]
    public bool hasMetDate = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void MarkMetDate()
    {
        if (!hasMetDate)
        {
            hasMetDate = true;
            Debug.Log("Player has met the Date. Exploration is now unlocked.");
        }
    }
}