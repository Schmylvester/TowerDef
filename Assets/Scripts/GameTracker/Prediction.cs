using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prediction : MonoBehaviour
{
    float winBonus = 0.4f;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            towerPrediction();
        }
        if (Input.GetKeyDown(KeyCode.U))
        {
            unitPrediction();
        }
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

    void towerPrediction()
    {
        //current state of the game
        InputRecord gameState = GameStateRecorder.instance.getGameState();

        //all previous states and actions from previous games
        List<IODSetup> defends = GameStateRecorder.instance.getDefenceData();
        int bestDistIdx = 0;
        float bestDist = float.MaxValue;
        for (int i = 0; i < defends.Count; i++)
        {
            float dist = getDistFromCurrentState(gameState, defends[i].input);
            if (defends[i].didWin)
            {
                dist -= winBonus;
            }
            if (dist < bestDist)
            {
                bestDist = dist;
                bestDistIdx = i;
            }
        }

        Autoplay.instance.createTower(defends[bestDistIdx].output);
    }

    void unitPrediction()
    {
        //current state of the game
        InputRecord gameState = GameStateRecorder.instance.getGameState();

        //all previous states and actions from previous games
        List<IOASetup> attacks = GameStateRecorder.instance.getAttackData();

        int bestDistIdx = 0;
        float bestDist = float.MaxValue;
        for (int i = 0; i < attacks.Count; i++)
        {
            float dist = getDistFromCurrentState(gameState, attacks[i].input);
            if (attacks[i].didWin)
            {
                dist -= winBonus;
            }
            if (dist < bestDist)
            {
                bestDist = dist;
                bestDistIdx = i;
            }
        }

        Autoplay.instance.createUnit(attacks[bestDistIdx].output);
    }
}
