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
    static float[] scores;
    float checkAverage = float.MinValue;
    static int gamesOver = 0;

    private void Start()
    {
        if (scores == null)
            scores = new float[100];
    }

    public void endGame(float defScore, int gameIdx)
    {
        scores[gameIdx] = defScore;
        resetGame();
        if (++gamesOver >= 100)
        {
            timer.resetGame();
            gamesOver = 0;
            if (--evolveCount > 0)
            {
                Evolve.instance.evolve();

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
        m.frames.gameOver = false;
        m.gsr.gameEnded = false;
        m.gsr.changeColours();
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
}
