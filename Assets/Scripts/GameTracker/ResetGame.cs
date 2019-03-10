using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetGame : MonoBehaviour
{
    [SerializeField] Structure[] walls;
    [SerializeField] Resources[] resources;
    [SerializeField] Timer timer;
    [SerializeField] List<Tower> safeTowers;
    [SerializeField] int count = 100;
    [SerializeField] Evolve evolve;
    [SerializeField] int evolveCount;
    float[] scores;
    float checkAverage = float.MinValue;

    private void Start()
    {
        scores = new float[100];
    }

    public void endGame(float defScore)
    {
        if (--count > 0)
        {
            scores[count] = defScore;
            resetGame();
            GameStateRecorder.instance.incrementOutFile();
            Prediction.instance.nextFile();
            PlayFrames.instance.gameOver = false;
        }
        else if (--evolveCount > 0)
        {
            evolve.evolve();
            count = 100;

            float total = 0;
            float max = 0;
            float min = float.MaxValue;
            int wins = 0;
            foreach(float s in scores)
            {
                total += s;
                max = Mathf.Max(max, s);
                if(s > 0)
                    min = Mathf.Min(min, s);
                if (s >= 1)
                    wins++;
            }
            float average = total / 100;
            Debug.Log("Average Score: " + average + " Max Score: " + max + " Min Score: " + min + " Wins: " + wins);
            if(evolveCount % 50 == 0)
            {
                Debug.Log("Checking average...");
                if(average <= checkAverage)
                {
                    Debug.LogError("No good");
                }
                else
                {
                    Debug.Log("Fine");
                }
                checkAverage = average;
            }
            resetGame();
            GameStateRecorder.instance.incrementOutFile();
            Prediction.instance.nextFile();
            PlayFrames.instance.gameOver = false;
        }
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
        foreach (Tower tower in EntityTracker.instance.getTowers())
        {
            if (!safeTowers.Contains(tower))
                Destroy(tower.gameObject);
        }
        foreach (Unit unit in EntityTracker.instance.getUnits())
        {
            Destroy(unit.gameObject);
        }
        EntityTracker.instance.resetGame();
        timer.resetGame();
    }

    public bool randomMatches()
    {
        return count <= 10;
    }
}
