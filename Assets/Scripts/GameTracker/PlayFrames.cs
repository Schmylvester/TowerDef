using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayFrames : MonoBehaviour
{
    List<ManualUpdate> trackedObjects = new List<ManualUpdate>();
    [HideInInspector] public uint frame = 0;
    [HideInInspector] public bool gameOver = false;
    [SerializeField] bool randomGame = false;
    [SerializeField] ResetGame reset;
    [SerializeField] int framesAtATime;
    [SerializeField] GameManager m;

    private void Awake()
    {
        m.frames = this;
        framesAtATime = Mathf.Max(framesAtATime, 1);
    }

    public void addItem(ManualUpdate item)
    {
        trackedObjects.Add(item);
    }

    public void removeItem(ManualUpdate item)
    {
        if (trackedObjects.IndexOf(item) < trackedObjects.Count && trackedObjects.IndexOf(item) >= 0)
            trackedObjects[trackedObjects.IndexOf(item)] = null;
    }
    private void Update()
    {
        if (randomGame)
        {
            for (int i = 0; i < framesAtATime; i++)
            {
                if (!gameOver)
                {
                    if (Random.Range(0, 100) == 0)
                    {
                        m.prediction.towerPrediction();
                    }
                    else if (reset.randomMatches() || Random.Range(0, 300) == 0)
                    {
                        m.autoplay.createTower(new EntityData(), true);
                    }
                    if (Random.Range(0, 1000) == 0)
                    {
                        m.autoplay.createUnit(new UnitData(), true);
                    }
                    playFrame(0.01f);
                    frame++;
                }
            }
        }
        else
        {
            if (!gameOver)
            {
                playFrame(Time.deltaTime);
                frame++;
            }
        }
    }

    public void playFrame(float rate)
    {
        for (int i = 0; i < trackedObjects.Count; i++)
            if (trackedObjects[i] == null)
            {
                trackedObjects.Remove(trackedObjects[i]);
                i--;
            }
        if (m.autoplay.replayRunning())
        {
            m.autoplay.replayFrame(frame);
        }
        for (int i = 0; i < trackedObjects.Count; i++)
        {
            if (trackedObjects[i])
                trackedObjects[i].update(rate);
        }
    }
}
