using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HalfAttack : Attack
{
    public override void randomiseInit()
    {
        base.randomiseInit();
        damage = Random.Range(1, 100.0f);
    }

    public override void balance(float by, int maxDam)
    {
        base.balance(by, maxDam);
        if (damage > 99)
            damage = 99;
    }

    public override void use()
    {
        if (Random.Range(0.0f, 1.0f) < accuracy)
        {
            //damage them for 'damage' % of their health
            target.takeDamage((int)(target.getHealth() * (damage / 100)));
        }
    }

    protected override void updateUI()
    {
        damageUI.text = "Damages enemy for " + damage.ToString("0") + "% of their health";
        accuracyUI.text = "Accuracy - " + (100 * accuracy).ToString("0");
    }
}
