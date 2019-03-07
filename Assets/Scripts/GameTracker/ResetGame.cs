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
    int alsoCount = 50;
    private void Update()
    {
        if (PlayFrames.instance.gameOver)
        {
            if (--count > 0)
            {
                resetGame();
                GameStateRecorder.instance.incrementOutFile();
                Prediction.instance.nextFile();
                PlayFrames.instance.gameOver = false;
            }
            else if(--alsoCount > 0)
            {
                evolve.evolve();
                count = 100;
            }
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
        return count < 10;
    }
}
