using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Evolve : MonoBehaviour
{
    int fileCount = 100;
    int survivors = 10;
    public static Evolve instance;
    ResetGame[] resets;
    [SerializeField] Timer timer;

    private void Awake()
    {
        if (instance)
            Destroy(gameObject);
        else
            instance = this;
        resets = FindObjectsOfType<ResetGame>();
    }

    public IEnumerator evolve()
    {
        timer.pause();
        yield return new WaitForSeconds(1.0f);
        Defends[] allDefs = new Defends[fileCount];
        for (int i = 0; i < fileCount; i++)
        {
            StreamReader file = new StreamReader("Assets/kNNData/Defends/" + i + ".json");
            allDefs[i] = JsonUtility.FromJson<Defends>(file.ReadToEnd());
            file.Close();
        }
        allDefs = sortDefends(allDefs);
        for(int i = survivors; i < survivors * 1.5f; i++)
        {
            allDefs[i] = combine(allDefs[i - survivors], allDefs[i - (survivors / 2)]);
        }
        for(int i = 0; i < fileCount; i++)
        {
            StreamWriter file = new StreamWriter("Assets/kNNData/Defends/" + i + ".json");
            file.Write(JsonUtility.ToJson(allDefs[i]));
            file.Close();
        }

        //yield return new WaitForSeconds(1.0f);
        foreach (ResetGame r in resets)
            r.resume();
        timer.resume();
        yield return null;
    }

    Defends[] sortDefends(Defends[] defends)
    {
        bool canExit = true;
        do
        {
            canExit = true;
            for (int i = 0; i < defends.Length - 1; i++)
            {
                if (defends[i].score < defends[i + 1].score)
                {
                    Defends temp = defends[i];
                    defends[i] = defends[i + 1];
                    defends[i + 1] = temp;
                    canExit = false;
                }
            }
        } while (!canExit);

        return defends;
    }

    Defends combine(Defends a, Defends b)
    {
        int expectedCount = (a.defends.Count + b.defends.Count) / 2;
        Defends combined = new Defends() { defends = new List<IODSetup>()};
        foreach (IODSetup d in a.defends)
            combined.defends.Add(d);
        foreach (IODSetup d in b.defends)
            combined.defends.Add(d);
        while (combined.defends.Count > expectedCount)
            combined.defends.RemoveAt(Random.Range(0, combined.defends.Count));
        return combined;
    }
}
