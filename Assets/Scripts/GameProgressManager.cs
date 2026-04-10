using UnityEngine;

public class GameProgressManager : MonoBehaviour
{
    public static GameProgressManager Instance;

    [Header("Date Progress")]
    public bool hasMetDate = false;

    [Header("Side Quest Rewards")]
    public bool hasBloodyAreaReward = false;
    public bool hasDiaryReward = false;
    public bool hasTrashReward = false;
    public bool hasRepentReward = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void MarkMetDate()
    {
        hasMetDate = true;
    }

    public void UnlockBloodyAreaReward()
    {
        hasBloodyAreaReward = true;
    }

    public void UnlockDiaryReward()
    {
        hasDiaryReward = true;
    }

    public void UnlockTrashReward()
    {
        hasTrashReward = true;
    }

    public void UnlockRepentReward()
    {
        hasRepentReward = true;
    }

    public void ResetAllProgress()
    {
        hasMetDate = false;
        hasBloodyAreaReward = false;
        hasDiaryReward = false;
        hasTrashReward = false;
        hasRepentReward = false;
    }
}