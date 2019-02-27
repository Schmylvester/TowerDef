using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayFrames : MonoBehaviour
{
    public static PlayFrames instance;
    List<ManualUpdate> trackedObjects = new List<ManualUpdate>();
    public uint frame = 0;
    public bool gameOver = false;

    private void Awake()
    {
        instance = this;
    }

    public void addItem(ManualUpdate item)
    {
        trackedObjects.Add(item);
    }

    public void removeItem(ManualUpdate item)
    {
        trackedObjects[trackedObjects.IndexOf(item)] = null;
    }
    private void Update()
    {
        if (!gameOver)
        {
            playFrame(Time.deltaTime);
            frame++;
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
