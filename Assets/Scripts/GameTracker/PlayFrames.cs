using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayFrames : MonoBehaviour
{
    public static PlayFrames instance;
    List<ManualUpdate> trackedObjects = new List<ManualUpdate>();
    [HideInInspector] public uint frame = 0;
    [HideInInspector] public bool gameOver = false;
    [SerializeField] bool randomGame = false;
    [SerializeField] ResetGame reset;
    [SerializeField] int framesAtATime;

    private void Awake()
    {
        framesAtATime = Mathf.Max(framesAtATime, 1);
        instance = this;
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
                    if (Random.Range(0, 1000) == 0)
                    {
                        if (reset.randomMatches() || Random.Range(0, 3) == 0)
                        {
                            Autoplay.instance.createTower(new EntityData(), true);
                        }
                        else
                        {
                            Prediction.instance.towerPrediction();
                        }
                    }
                    if (Random.Range(0, 1000) == 0)
                    {
                        Autoplay.instance.createUnit(new UnitData(), true);
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
        if (Autoplay.instance.replayRunning())
        {
            Autoplay.instance.replayFrame(frame);
        }
        foreach (ManualUpdate item in trackedObjects)
        {
            if (item)
                item.update(rate);
        }
    }
}
