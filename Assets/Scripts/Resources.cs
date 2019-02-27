using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resources : MonoBehaviour
{
    [SerializeField] int gold;
    [SerializeField] UnityEngine.UI.Text text;
    [SerializeField] float gainRate;
    [SerializeField] short gainAmount;
    float time = 0;

    private void Awake()
    {
        updateGold(0);
    }

    private void Update()
    {
        time += Time.deltaTime;
        if (time > gainRate)
        {
            time -= gainRate;
            updateGold(gainAmount);
        }
    }

    public int getGold()
    {
        return gold;
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
}
