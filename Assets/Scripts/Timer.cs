using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : ManualUpdate
{

    [SerializeField] UnityEngine.UI.Text timeText;
    [SerializeField] float startTime;
    float time;

    private void Start()
    {
        time = startTime;
        PlayFrames.instance.addItem(this);
    }

    public override void update(float rate)
    {
        time -= rate;
        time = Mathf.Max(0, time);
        if(time == 0)
        {
            FeedbackManager.instance.setFeedback(true, "You lose.");
            FeedbackManager.instance.setFeedback(false, "You win.");
            GameStateRecorder.instance.onGameOver(true);
        }
        showTime();
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
}
