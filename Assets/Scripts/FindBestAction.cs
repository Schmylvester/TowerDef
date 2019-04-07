using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct BattleInput
{
    public float myHealth;
    public float theirHealth;
}

[System.Serializable]
public struct InOutMap
{
    public float score;
    public BattleInput _in;
    public int[] action_selected;
}

public class FindBestAction : MonoBehaviour
{
    int attackCount = 2;

    //the pool of previous actions from which a good action is selected
    List<InOutMap> allGoodOnes = new List<InOutMap>();
    //the actions from this game
    List<InOutMap> thisGame = new List<InOutMap>();

    public void recordEvent(Fighter a, Fighter b, int action)
    {
        BattleInput input = new BattleInput() { myHealth = a.getScore(), theirHealth = b.getScore() };
        int[] new_action = new int[attackCount];
        new_action[action] = 1;
        InOutMap iomap = new InOutMap() { _in = input, action_selected = new_action };
        thisGame.Add(iomap);
    }

    public void endGame(float score)
    {
        foreach (InOutMap iom in thisGame)
        {
            allGoodOnes.Add(new InOutMap()
            {
                _in = iom._in,
                action_selected = iom.action_selected,
                score = score
            });
        }

        purgeActions();

        thisGame.Clear();
    }

    /// <summary>
    /// used to improve the actions pool for selection
    /// removes old data and bad data to constantly
    /// improve data used
    /// </summary>
    void purgeActions()
    {
        int whileLoopBroken = 0;
        //if there are more than 500 actions in allGoodOnes, remove them until there are only 500
        while (allGoodOnes.Count > 500 && ++whileLoopBroken < 1000)
        {
            //remove the oldest one because it's likely based on outdated attacks
            allGoodOnes.RemoveAt(0);

            //find the lowest scoring one and remove it
            float min = float.MaxValue;
            int mindex = -1;
            for (int i = 0; i < allGoodOnes.Count; i++)
            {
                if (allGoodOnes[i].score < min)
                {
                    min = allGoodOnes[i].score;
                    mindex = i;
                }
            }
            if (mindex != -1)
                allGoodOnes.RemoveAt(mindex);
        }
        if (whileLoopBroken > 1000)
        {
            Debug.LogError("Program got stuck in a while loop");
        }
    }


    public int getBestAction(Fighter a, Fighter b)
    {
        //how many neighbours sampled
        int k = 5;

        //if there isn't enough data to use, return a random
        //also sometimes just return a random for the sake of mutation
        if (allGoodOnes.Count < k || Random.Range(0, 20) > 0)
        {
            return Random.Range(0, attackCount);
        }

        //get current healths of each player
        float aHP = a.getScore();
        float bHP = b.getScore();
        //indices of the previous actions closest to current state
        List<int> closestIndices = new List<int>();
        //get 5 closest
        for (int index = 0; index < k; index++)
        {
            getBestDist(ref closestIndices, aHP, bHP);
        }

        //count each attack
        int[] counts = new int[attackCount];
        foreach (int index in closestIndices)
        {
            for (int i = 0; i < attackCount; i++)
            {
                if (allGoodOnes[index].action_selected[i] == 1)
                {
                    counts[i]++;
                }
            }
        }
        int maxIdx = -1;
        int max = -1;
        for(int i = 0; i < counts.Length; i++)
        {
            if(counts[i] > max)
            {
                max = counts[i];
                maxIdx = i;
            }
        }
        return Mathf.Max(maxIdx);
    }

    void getBestDist(ref List<int> closestIndices, float aHP, float bHP)
    {
        //initialise closest to be as far as possible and an invalid index
        float bestDist = float.MaxValue;
        int bestIdx = -1;

        //search through the previous action list
        for (int prevAct = 0; prevAct < allGoodOnes.Count; prevAct++)
        {
            //skip it if it's already there
            if (!closestIndices.Contains(prevAct))
            {
                //get distance
                float dist
                    = Mathf.Pow(aHP - allGoodOnes[prevAct]._in.myHealth, 2)
                    + Mathf.Pow(bHP - allGoodOnes[prevAct]._in.theirHealth, 2);
                //if it beats the best distance, it's the new best distance
                if (dist < bestDist)
                {
                    bestDist = dist;
                    bestIdx = prevAct;
                }
                //if it's the same, take one at random
                else if (dist == bestDist)
                {
                    bestIdx = Random.Range(0, 2) == 0 ? bestIdx : prevAct;
                }
            }
        }
        closestIndices.Add(bestIdx);
    }
}
