using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resources : MonoBehaviour
{
    [SerializeField] int gold;
    [SerializeField] UnityEngine.UI.Text text;

    private void Awake()
    {
        updateGold(0);
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
