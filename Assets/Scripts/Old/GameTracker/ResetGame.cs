using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetGame : MonoBehaviour
{
    [SerializeField] Structure[] walls;
    [SerializeField] Resources[] resources;
    [SerializeField] Timer timer;
    [SerializeField] List<Tower> safeTowers;
    int evolveCount = 500;
    [SerializeField] GameManager m;
    float checkAverage = float.MinValue;
    static float[] scores;
    static bool[] gamesOver;

    private void Start()
    {
        if (scores == null)
            scores = new float[100];
        if (gamesOver == null)
            gamesOver = new bool[100];
    }

    public void endGame(float defScore, int gameIdx)
    {
        scores[gameIdx] = defScore;
        gamesOver[gameIdx] = true;
        resetGame();
        m.frames.frame = 0;
        if (allGamesOver())
        {
            resetGamesOver();
            if (--evolveCount > 0)
            {
                StartCoroutine(Evolve.instance.evolve());

                float total = 0;
                float max = 0;
                float min = float.MaxValue;
                int wins = 0;
                foreach (float s in scores)
                {
                    total += s;
                    max = Mathf.Max(max, s);
                    if (s > 0)
                        min = Mathf.Min(min, s);
                    if (s >= 1)
                        wins++;
                }
                float average = total / 100;
                Debug.Log("Average Score: " + average + " Max Score: " + max + " Min Score: " + min + " Wins: " + wins);
                if (evolveCount % 50 == 0)
                {
                    Debug.Log("Checking average...");
                    if (average <= checkAverage)
                    {
                        Debug.LogError("No good");
                    }
                    else
                    {
                        Debug.Log("Fine");
                    }
                    checkAverage = average;
                }
            }
        }
    }

    public void resume()
    {
        m.prediction.reload();
        m.gsr.changeColours();
        m.frames.restart();
        timer.restartTime();
        m.gsr.setGameEnded(false);
    }

    public void resetGame()
    {
        foreach (Structure wall in walls)
        {
            wall.resetGame();
        }
        foreach (Resources resource in resources)
        {
            resource.resetGame();
        }
        foreach (Tower tower in m.tracker.getTowers())
        {
            if (!safeTowers.Contains(tower))
                Destroy(tower.gameObject);
        }
        foreach (Unit unit in m.tracker.getUnits())
        {
            Destroy(unit.gameObject);
        }
        m.tracker.resetGame();
    }

    bool allGamesOver()
    {
        foreach (bool over in gamesOver)
            if (!over)
                return false;
        return true;
    }

    void resetGamesOver()
    {
        for (int i = 0; i < gamesOver.Length; i++)
            gamesOver[i] = false;
    }
}
