using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour
{

    [SerializeField] UnityEngine.UI.Text timeText = null;
    [SerializeField] float startTime = 0;
    float time;
    [SerializeField] GameSpeed speed = null;
    [SerializeField] GameStateRecorder[] gsrs = null;
    bool paused = false;

    private void Start()
    {
        time = startTime;
    }

    private void Update()
    {
        if (!paused)
            time -= speed.getSpeed();
        time = Mathf.Max(0, time);
        if (time == 0)
        {
            FeedbackManager.instance.setFeedback(true, "You lose.");
            FeedbackManager.instance.setFeedback(false, "You win.");
            foreach (GameStateRecorder gsr in gsrs)
            {
                if (!gsr.getGameEnded())
                {
                    gsr.onGameOver(true);
                }
            }
            time = startTime;
        }
        showTime();
    }

    public void restartTime()
    {
        time = startTime;
    }
    
    public int frameCount()
    {
        return (int)(startTime / GameSpeed.instance.getFrameSpeed());
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

    public bool ended()
    {
        return time > 0;
    }

    public void pause()
    {
        paused = true;
    }

    public void resume()
    {
        paused = false;
    }
}
