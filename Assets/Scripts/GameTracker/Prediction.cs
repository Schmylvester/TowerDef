using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prediction : MonoBehaviour
{
    uint defend_file;
    uint attack_file;

    List<IODSetup> defends;
    List<IOASetup> attacks;

    public static Prediction instance;

    private void Awake()
    {
        instance = this;
        System.IO.StreamReader file = 
            new System.IO.StreamReader("Assets/kNNData/Defends/" + defend_file + ".json");
        defends = JsonUtility.FromJson<Defends>(file.ReadToEnd()).defends;
        file.Close();
        file = new System.IO.StreamReader("Assets/kNNData/Attacks/" + attack_file + ".json");
        attacks = JsonUtility.FromJson<Attacks>(file.ReadToEnd()).attacks;
        file.Close();
    }

    float getDist(EntityData a, EntityData b)
    {
        float dist = Mathf.Pow(a.x - b.x, 2);
        dist += Mathf.Pow(a.y - b.y, 2);
        dist += Mathf.Pow(a.health - b.health, 2);
        for (int i = 0; i < a.type.Length; i++)
        {
            dist += Mathf.Pow(a.type[i] - b.type[i], 2);
        }
        return dist;
    }

    float findClosest(EntityData to, List<EntityData> inList)
    {
        float best = 3.0f;
        foreach (EntityData ent in inList)
        {
            float dist = getDist(ent, to);
            if (dist < best)
                best = dist;
        }
        return best;
    }

    float getDistFromCurrentState(InputRecord currentInput, InputRecord previousInput)
    {
        float dist = 0;
        foreach (EntityData tower in currentInput.towers)
        {
            dist += findClosest(tower, previousInput.towers);
        }
        foreach (EntityData unit in currentInput.units)
        {
            dist += findClosest(unit, previousInput.units);
        }
        foreach (EntityData wall in currentInput.walls)
        {
            dist += findClosest(wall, previousInput.walls);
        }
        dist += Mathf.Pow(currentInput.attackerResources - previousInput.attackerResources, 2);
        dist += Mathf.Pow(currentInput.defenderResources - previousInput.defenderResources, 2);
        return dist;
    }

    public void towerPrediction()
    {
        if (defends.Count == 0)
            return;
        //current state of the game
        InputRecord gameState = GameStateRecorder.instance.getGameState();
        
        int bestDistIdx = 0;
        float bestDist = float.MaxValue;
        for (int i = 0; i < defends.Count; i++)
        {
            float dist = getDistFromCurrentState(gameState, defends[i].input);
            dist -= defends[i].score;

            if (dist < bestDist)
            {
                bestDist = dist;
                bestDistIdx = i;
            }
            else if(dist == bestDist)
            {
                bestDistIdx = Random.Range(0, 2) == 0 ? i : bestDistIdx;
            }
        }

        Autoplay.instance.createTower(defends[bestDistIdx].output);
        defends.RemoveAt(bestDistIdx);
    }

    public void unitPrediction()
    {
        //current state of the game
        InputRecord gameState = GameStateRecorder.instance.getGameState();

        int bestDistIdx = 0;
        float bestDist = float.MaxValue;
        for (int i = 0; i < attacks.Count; i++)
        {
            float dist = getDistFromCurrentState(gameState, attacks[i].input);
            dist -= attacks[i].score;
            if (dist < bestDist)
            {
                bestDist = dist;
                bestDistIdx = i;
            }
        }

        Autoplay.instance.createUnit(attacks[bestDistIdx].output);
    }

    public void nextFile()
    {
        attack_file++;
        defend_file++;
    }
}
