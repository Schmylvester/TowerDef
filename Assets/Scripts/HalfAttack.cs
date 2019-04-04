using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HalfAttack : Attack
{
    public override void use()
    {
        if (Random.Range(0.0f, 1.0f) < accuracy)
        {
            target.takeDamage((int)(target.getHealth() / (100.0f / damage)));
        }
    }

    protected override void updateUI()
    {
        damageUI.text = "Damages enemy for " + damage.ToString("0") + "% of their health";
        accuracyUI.text = "Accuracy - " + (100 * accuracy).ToString("0");
    }
}
