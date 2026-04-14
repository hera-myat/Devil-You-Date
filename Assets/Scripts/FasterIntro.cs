using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class FasterIntro : MonoBehaviour
{
    public PlayableDirector director;

    public KeyCode fastForwardKey = KeyCode.Space;
    public float normalSpeed = 1f;
    public float fastForwardSpeed = 3f;

    public KeyCode skipKey = KeyCode.S;

    public string nextSceneName = "FinalScene";

    private bool wasFastForwarding = false;
    private bool hasTransitioned = false;

    // Update is called once per frame
    void Update()
    {
        if (director == null) return;
        if (director.state != PlayState.Playing) return;

        bool isFastForwarding = Input.GetKey(fastForwardKey);

        if (isFastForwarding != wasFastForwarding)
        {
            SetTimelineSpeed(isFastForwarding ? fastForwardSpeed : normalSpeed);
            wasFastForwarding = isFastForwarding;
        }

        if (Input.GetKeyDown(skipKey))
        {
            SkipCutscene();
        }
    }

    void SetTimelineSpeed(float speed)
    {
        var graph = director.playableGraph;
        if (!graph.IsValid()) return;

        int rootCount = graph.GetRootPlayableCount();

        for (int i = 0; i < rootCount; i++)
        {
            graph.GetRootPlayable(i).SetSpeed(speed);
        }
    }

    void SkipCutscene()
    {
        if (hasTransitioned) return;
        hasTransitioned = true;
        SetTimelineSpeed(normalSpeed);

        if (director != null)
        {
            director.Stop();
        }

        LoadNextScene();
    }

    public void LoadNextScene()
    {
        if (hasTransitioned && SceneManager.GetActiveScene().name != nextSceneName)
        {
            SceneManager.LoadScene(nextSceneName);
            return;
        }

        if (!hasTransitioned)
        {
            hasTransitioned = true;
            SceneManager.LoadScene(nextSceneName);
        }
    }
}
