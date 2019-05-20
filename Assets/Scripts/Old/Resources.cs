using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resources : ManualUpdate
{
    [SerializeField] int startGold = 0;
    int gold;
    [SerializeField] UnityEngine.UI.Text text = null;
    [SerializeField] float gainRate = 0;
    [SerializeField] short gainAmount = 0;
    float time = 0;
    [SerializeField] PlayFrames frames = null;

    private void Start()
    {
        gold = startGold;
        updateGold(0);
        frames.addItem(this);
    }

    public override void update(float rate)
    {
        time += rate;
        if (time > gainRate)
        {
            time -= gainRate;
            updateGold(gainAmount);
        }
    }

    public bool canAfford(short cost)
    {
        return gold >= cost;
    }

    public void updateGold(short by)
    {
        gold += by;
        gold = Mathf.Max(0, gold);
        text.text = gold.ToString();
    }

    public float normalisedValue()
    {
        return (float)gold / 512;
    }

    public void resetGame()
    {
        gold = startGold;
        updateGold(0);
    }
}
