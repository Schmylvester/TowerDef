using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drain : Attack
{
    float drainRate = 1.0f;

    public override void use()
    {
        if (Random.Range(0.0f, 1.0f) < accuracy)
        {
            target.takeDamage((int)damage);
            user.takeDamage((int)-(damage / 2));
        }
    }

    public override void balance(float by)
    {
        drainRate = Mathf.Clamp(drainRate + by, 0.1f, 1.5f);
        base.balance(by);
    }

    protected override void updateUI()
    {
        damageUI.text = "Damages target for " + damage.ToString("0") 
            + " and recovers by " + (damage * drainRate).ToString("0");
        accuracyUI.text = "Accuracy - " + (100 * accuracy).ToString("0");
    }
}
