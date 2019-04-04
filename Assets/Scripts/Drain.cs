using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drain : Attack
{
    public override void use()
    {
        if (Random.Range(0.0f, 1.0f) < accuracy)
        {
            target.takeDamage((int)damage);
            user.takeDamage((int)-damage);
        }
    }

    protected override void updateUI()
    {
        damageUI.text = "Drains " + damage.ToString("0") + " health from target";
        accuracyUI.text = "Accuracy - " + (100 * accuracy).ToString("0");
    }
}
