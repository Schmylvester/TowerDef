using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prediction : MonoBehaviour
{
    uint defend_file;
    uint attack_file;

    Defends defends;
    Attacks attacks;
    [SerializeField] GameManager m;

    private void Awake()
    {
        m.prediction = this;
        string fileOut = "";
        foreach (char c in gameObject.name)
        {
            if (char.IsDigit(c))
                fileOut += c;
        }
        defend_file = uint.Parse(fileOut);
        reload();
    }

    public void reload()
    {
        System.IO.StreamReader file =
            new System.IO.StreamReader("Assets/kNNData/Defends/" + defend_file + ".json");
        defends = JsonUtility.FromJson<Defends>(file.ReadToEnd());
        file.Close();
        file = new System.IO.StreamReader("Assets/kNNData/Attacks/" + attack_file + ".json");
        attacks = JsonUtility.FromJson<Attacks>(file.ReadToEnd());
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

    public bool towerPrediction()
    {
        if (defends.defends.Count == 0)
        {
            return false;
        }
        //current state of the game
        InputRecord gameState = m.gsr.getGameState();

        int bestDistIdx = 0;
        float bestDist = float.MaxValue;
        for (int i = 0; i < defends.defends.Count; i++)
        {
            float dist = getDistFromCurrentState(gameState, defends.defends[i].input);
            if (dist < bestDist)
            {
                bestDist = dist;
                bestDistIdx = i;
            }
            else if (dist == bestDist)
            {
                bestDistIdx = Random.Range(0, 2) == 0 ? i : bestDistIdx;
            }
        }
        if (bestDist < 1.0f)
        {
            m.autoplay.createTower(defends.defends[bestDistIdx].output);
            modifyDefenceOutput(bestDistIdx);
            return true;
        }
        return false;
    }

    public void unitPrediction()
    {
        //current state of the game
        InputRecord gameState = m.gsr.getGameState();

        int bestDistIdx = 0;
        float bestDist = float.MaxValue;
        for (int i = 0; i < attacks.attacks.Count; i++)
        {
            float dist = getDistFromCurrentState(gameState, attacks.attacks[i].input);
            dist -= attacks.score;
            if (dist < bestDist)
            {
                bestDist = dist;
                bestDistIdx = i;
            }
        }

        m.autoplay.createUnit(attacks.attacks[bestDistIdx].output);
    }

    void modifyDefenceOutput(int idx)
    {
        IODSetup d = defends.defends[idx];
        IODSetup newSet = new IODSetup()
        {
            frame = d.frame,
            input = d.input,
            output = new EntityData()
            {
                health = d.output.health,
                type = d.output.type,
                x = d.output.x + (Random.Range(-0.05f, 0.05f)),
                y = d.output.y + (Random.Range(-0.05f, 0.05f))
    }
        };
        defends.defends[idx] = newSet;
    }
}
