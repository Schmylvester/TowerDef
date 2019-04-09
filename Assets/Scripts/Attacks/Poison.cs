using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Poison : Attack
{
    [SerializeField] float infectionRate;
    float activeDamage;

    public override void randomiseInit()
    {
        infectionRate = Random.Range(1.1f, 2.0f);
        base.randomiseInit();
    }

    public override void balance(float by, int maxDam)
    {
        activeDamage = damage;
        if (Random.Range(0, 2) == 0)
        {
            infectionRate = Mathf.Clamp(infectionRate + by, 1.1f, 2.0f);
        }
        base.balance(by, maxDam);
    }

    public override void use()
    {
        if (Random.Range(0.0f, 1.0f) < accuracy)
        {
            target.takeDamage((int)activeDamage);
            activeDamage *= infectionRate;
            updateUI();
        }
    }

    protected override void updateUI()
    {
        base.updateUI();
        damageUI.text =
            "Damages for " + activeDamage.ToString("0")
            + " and increases by "
            + ((infectionRate - 1) * 100).ToString("0") + "% each use (base damage: " + damage + ")";
    }
}
