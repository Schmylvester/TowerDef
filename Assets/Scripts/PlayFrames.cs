using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayFrames : MonoBehaviour
{
    public static PlayFrames instance;
    List<MonoBehaviour> trackedObjects = new List<MonoBehaviour>();
    [SerializeField] Spawner spawner;

    private void Awake()
    {
        instance = this;
    }

    public void addItem(MonoBehaviour item)
    {
        trackedObjects.Add(item);
    }

    public void removeItem(MonoBehaviour item)
    {
        trackedObjects[trackedObjects.IndexOf(item)] = null;
    }
    private void Update()
    {
        playFrame(Time.deltaTime);
    }

    public void playFrame(float rate)
    {
        foreach (MonoBehaviour item in trackedObjects)
        {
            if (item as LineSet)
                (item as LineSet).update(rate);
            else if (item as Unit)
                (item as Unit).update(rate);
            else if (item as Tower)
                (item as Tower).update(rate);
        }

        for (int i = 0; i < trackedObjects.Count; i++)
            if (trackedObjects[i] == null)
            {
                trackedObjects.Remove(trackedObjects[i]);
                i--;
            }
        spawner.update(rate);
    }
}
