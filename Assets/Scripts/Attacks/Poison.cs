using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Poison : Attack
{
    [SerializeField] float infectionRate;
    float activeDamage;
    bool used = false;

    public override void balance(float by, int maxDam)
    {
        used = false;
        activeDamage = damage;
        if (Random.Range(0, 2) == 0)
        {
            infectionRate = Mathf.Clamp(infectionRate + by, 1.1f, 2.0f);
        }
        base.balance(by, maxDam);
    }

    public override void use()
    {
        if (!used)
        {
            used = true;
            activeDamage = damage;
        }
        if (Random.Range(0.0f, 1.0f) < accuracy)
        {
            target.takeDamage((int)activeDamage);
            activeDamage *= infectionRate;
        }
    }

    protected override void updateUI()
    {
        base.updateUI();
        damageUI.text =
            "Damages for " + activeDamage.ToString("0")
            + " damage and increases by "
            + ((infectionRate - 1) * 100).ToString("0") + "% each use";
    }
}
