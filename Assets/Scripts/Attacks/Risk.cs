using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Risk : Attack
{
    [SerializeField] float dieChance;

    public override void randomiseInit()
    {
        damage = Random.Range(10, 100);
        dieChance = Random.Range(0.4f, 0.9f);
        accuracy = Random.Range(0.4f, 0.9f);
    }

    public override void use()
    {
        if (Random.Range(0.0f, 1.0f) < accuracy)
        {
            target.takeDamage((int)damage);
        }
        else if (Random.Range(0.0f, 1.0f) < dieChance)
        {
            user.takeDamage((int)damage);
        }
    }

    public override void balance(float by, int maxDam)
    {
        if (Random.Range(0, 2) == 0)
        {
            dieChance = Mathf.Clamp(dieChance + by, 0.01f, 1.0f);
        }
        base.balance(by, maxDam);
    }

    protected override void updateUI()
    {
        base.updateUI();
        damageUI.text = "If the attack hits it hits the opponent for " + damage + " damage, if it misses it has " + ((int)(100 * dieChance)).ToString() + "% chance of hitting the player.";
    }
}
