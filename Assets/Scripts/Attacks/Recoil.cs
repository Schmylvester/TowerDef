using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Recoil : Attack
{
    [SerializeField] float recoilRate = 1.0f;

    public override void use()
    {
        if (Random.Range(0.0f, 1.0f) < accuracy)
        {
            target.takeDamage((int)damage);
        }
        //wont kill the user, but will reduce them to 1hp
        //damages the user whether it hits or not
        if (user.getHealth() > (int)(damage * recoilRate))
        {
            user.takeDamage((int)(damage * recoilRate));
        }
        else
        {
            user.takeDamage(user.getHealth() - 1);
        }
    }

    public override void balance(float by, int maxDam)
    {
        if (Random.Range(0, 2) == 0)
        {
            recoilRate = Mathf.Clamp(recoilRate - by, 0.1f, 1.5f);
        }
        base.balance(by, maxDam);
    }

    protected override void updateUI()
    {
        base.updateUI();
        damageUI.text = "Hits opponent for " + damage.ToString("0") 
            + " and damages self for " + (damage * recoilRate).ToString("0");
    }
}
