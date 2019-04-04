using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayFrames : MonoBehaviour
{
    List<ManualUpdate> trackedObjects = new List<ManualUpdate>();
    [HideInInspector] public uint frame = 0;
    [SerializeField] bool randomGame = false;
    [SerializeField] ResetGame reset;
    [SerializeField] GameManager m;
    [SerializeField] GameSpeed speed;
    int defaultRand = 1000;
    int activeRand = 1000;
    bool createdThisGame = false;
    bool createdLastGame = true;
    bool coroutineActive = false;

    private void Awake()
    {
        m.frames = this;
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
            for (int i = 0; i < speed.getFrameCount(); i++)
            {
                if (!m.gsr.getGameEnded())
                {
                    if (Random.Range(0, 100) == 0)
                    {
                        if (!m.prediction.towerPrediction())
                        {
                            m.autoplay.createTower(new EntityData(), true);
                        }
                    }
                    if (Random.Range(0, activeRand) == 0)
                    {
                        createdThisGame = true;
                        activeRand = defaultRand;
                        m.autoplay.createUnit(new UnitData(), true);
                    }
                    else if (activeRand > 1)
                    {
                        activeRand--;
                    }
                    playFrame(speed.getFrameSpeed());
                    frame++;
                }
            }
        }
        else if(!coroutineActive)
        {
            coroutineActive = true;
            StartCoroutine(go());
        }
    }

    IEnumerator go()
    {
        while(!m.gsr.getGameEnded())
        {
            playFrame(speed.getFrameSpeed());
            frame++;
            yield return null;
        }
        coroutineActive = false;
        yield return null;
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

    public void restart()
    {
        createdLastGame = createdThisGame;
        createdThisGame = false;
    }
}
