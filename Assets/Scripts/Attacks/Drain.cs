using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drain : Attack
{
    [SerializeField] float drainRate = 1.0f;

    public override void randomiseInit()
    {
        base.randomiseInit();
        drainRate = Random.Range(0.1f, 1.5f);
    }

    public override void use()
    {
        if (Random.Range(0.0f, 1.0f) < accuracy)
        {
            target.takeDamage((int)damage);
            user.takeDamage((int)-(damage / 2));
        }
    }

    public override void balance(float by, int maxDam)
    {
        drainRate = Mathf.Clamp(drainRate + by, 0.1f, 1.5f);
        base.balance(by, maxDam);
    }

    //if the game was the wrong length, they should recover less or more health
    public override void lengthBalance(float by, int maxDam)
    {
        drainRate = Mathf.Clamp(drainRate - by, 0.1f, 1.5f);
        base.lengthBalance(by, maxDam);
    }

    protected override void updateUI()
    {
        damageUI.text = "Damages target for " + damage.ToString("0") 
            + " and recovers by " + (damage * drainRate).ToString("0");
        accuracyUI.text = "Accuracy - " + (100 * accuracy).ToString("0");
    }
}
