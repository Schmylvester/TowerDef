using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour
{

    [SerializeField] UnityEngine.UI.Text timeText;
    [SerializeField] float startTime;
    float time;
    float rate;

    private void Start()
    {
        time = startTime;
        rate = FindObjectOfType<PlayFrames>().getRate();
    }

    private void Update()
    {
        time -= rate;
        time = Mathf.Max(0, time);
        if (time == 0)
        {
            FeedbackManager.instance.setFeedback(true, "You lose.");
            FeedbackManager.instance.setFeedback(false, "You win.");
            foreach (GameStateRecorder gsr in FindObjectsOfType<GameStateRecorder>())
                gsr.onGameOver(true);
        }
        showTime();
    }

    public float getScore()
    {
        return 1 - (time / startTime);
    }

    void showTime()
    {
        string minutes = ((int)time / 60).ToString();
        string seconds = ((int)time % 60).ToString();
        float m = time - (int)time;
        string miliSeconds = ((int)(m * 100)).ToString();

        if (seconds.Length == 1)
            seconds = "0" + seconds;
        if (miliSeconds.Length == 1)
            miliSeconds = "0" + miliSeconds;

        timeText.text = minutes.ToString() + ":" + seconds + "." + miliSeconds;
    }

    public void resetGame()
    {
        time = startTime;
    }
}
