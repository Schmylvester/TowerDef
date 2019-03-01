﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameScorer : MonoBehaviour
{
    public static GameScorer instance;
    [SerializeField] Structure castle;
    [SerializeField] Timer timer;
    [SerializeField] Grid grid;
    float distScore = 0;

    private void Awake()
    {
        instance = this;
    }

    public float getDefScore(bool won)
    {
        if (won)
        {
            return (2 - getAttScore(false));
        }
        else
        {
            return timer.getScore();
        }
    }
    public float getAttScore(bool won)
    {
        if (won)
            return 2 - getDefScore(false);
        else
        {
            float score = 1 - castle.getHealth();
            score += distScore;
            score /= 2;
            return score;
        }
    }

    public void unitDies(float pos)
    {
        float normPos = grid.WorldToCell(new Vector3(pos, 0, 0)).x;
        normPos /= 360;
        normPos += 0.5f;
        distScore = Mathf.Max(normPos, distScore);
    }
}
