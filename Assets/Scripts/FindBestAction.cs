﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct BattleInput
{
    public float myHealth;
    public float theirHealth;
}
[System.Serializable]
public struct BattleOutput
{
    public int myAction;
}

[System.Serializable]
public struct InOutMap
{
    public BattleInput _in;
    public BattleOutput _out;
}

public class FindBestAction : MonoBehaviour
{
    [SerializeField] float scoreThreshold;
    List<InOutMap> allGoodOnes = new List<InOutMap>();
    List<InOutMap> thisGame = new List<InOutMap>();

    public void recordEvent(Fighter a, Fighter b, int action)
    {
        BattleInput input = new BattleInput() { myHealth = a.getScore(), theirHealth = b.getScore() };
        BattleOutput output = new BattleOutput() { myAction = action };
        InOutMap iomap = new InOutMap() { _in = input, _out = output };
        thisGame.Add(iomap);
    }

    public void endGame(float score)
    {
        if (score >= scoreThreshold)
        {
            foreach (InOutMap iom in thisGame)
            {
                allGoodOnes.Add(iom);
            }
        }
        thisGame.Clear();
    }

    public int getBestAction(Fighter a, Fighter b)
    {
        if (allGoodOnes.Count < 10)
        {
            return Random.Range(0, 2);
        }
        float aHP = a.getScore();
        float bHP = b.getScore();
        List<int> closestIndices = new List<int>() { -1, -1, -1, -1, -1 };

        for (int k = 0; k < 5; k++)
        {
            float bestDist = float.MaxValue;
            int bestIdx = -1;
            for (int j = 0; j < allGoodOnes.Count; j++)
            {
                if (!closestIndices.Contains(j))
                {
                    float dist
                        = Mathf.Pow(aHP - allGoodOnes[j]._in.myHealth, 2)
                        + Mathf.Pow(bHP - allGoodOnes[j]._in.theirHealth, 2);
                    if (dist < bestDist)
                    {
                        bestIdx = j;
                    }
                    else if (dist == bestDist)
                    {
                        bestIdx = Random.Range(0, 2) == 0 ? bestIdx : j;
                    }
                }
            }
            closestIndices[k] = bestIdx;
        }

        int[] counts = new int[2];
        foreach(int index in closestIndices)
        {
            counts[allGoodOnes[index]._out.myAction]++;
        }

        if(counts[0] > counts[1])
            return 0;
        return 1;
    }
}
