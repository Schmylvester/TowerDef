using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : ManualUpdate
{

    [SerializeField] UnityEngine.UI.Text timeText;
    [SerializeField] float startTime;
    float time;
    PlayFrames player;

    private void Start()
    {
        time = startTime;
        foreach (PlayFrames frames in FindObjectsOfType<PlayFrames>())
            if (!frames.gameOver)
            {
                player = frames;
                frames.addItem(this);
                break;
            }
    }

    private void Update()
    {
        if (player.gameOver)
            foreach (PlayFrames frames in FindObjectsOfType<PlayFrames>())
                if (!frames.gameOver)
                {
                    player = frames;
                    frames.addItem(this);
                    break;
                }
    }

    public float getScore()
    {
        return 1 - (time / startTime);
    }

    public override void update(float rate)
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
